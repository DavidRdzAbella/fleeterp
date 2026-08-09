using FleetErp.Application.Abstractions;
using FleetErp.Application.Common;
using FleetErp.Application.Contracts;
using FleetErp.Domain.Enums;

namespace FleetErp.Application.Services;

/// <summary>
/// Toda la aritmética de los tableros en un solo lugar y sin dependencias de
/// infraestructura: recibe los hechos del periodo por un puerto y devuelve KPIs,
/// series y rankings. Que sea código puro es lo que permite cubrirlo con pruebas.
/// </summary>
public sealed class AnalyticsService(IAnalyticsDataSource dataSource, IClock clock) : IAnalyticsService
{
    private const int DefaultWindowDays = 30;
    private const int TopDriversOnDashboard = 5;

    public async Task<FleetDashboardDto> GetFleetDashboardAsync(AnalyticsPeriod? period, CancellationToken ct = default)
    {
        var window = Normalize(period);
        var data = await dataSource.LoadAsync(window.FromUtc, window.ToUtc, ct);

        var today = DateOnly.FromDateTime(clock.UtcNow.UtcDateTime);
        var financials = Summarize(data);

        return new FleetDashboardDto(
            Period: window,
            Fleet: BuildFleetStatus(data),
            Activity: BuildActivity(data, today),
            Financials: financials,
            DistanceByDay: SeriesByDay(window, data.Trips, t => t.EffectiveDistance),
            RevenueByDay: SeriesByDay(window, data.Trips, t => t.FreightRevenue),
            ProfitByDay: BuildProfitByDay(window, data),
            CostBreakdown: BuildCostBreakdown(data),
            TopDrivers: BuildRanking(data, DriverRankingCriteria.Distance, TopDriversOnDashboard),
            ActiveTrips: [],
            Alerts: BuildAlerts(data, today));
    }

    public async Task<IReadOnlyList<DriverRankingRowDto>> GetDriverRankingAsync(
        AnalyticsPeriod? period, DriverRankingCriteria criteria, int take, CancellationToken ct = default)
    {
        var window = Normalize(period);
        var data = await dataSource.LoadAsync(window.FromUtc, window.ToUtc, ct);
        return BuildRanking(data, criteria, take <= 0 ? 10 : take);
    }

    public async Task<DriverPerformanceDto> GetDriverPerformanceAsync(Guid driverId, AnalyticsPeriod? period, CancellationToken ct = default)
    {
        var window = Normalize(period);
        var data = await dataSource.LoadAsync(window.FromUtc, window.ToUtc, ct);

        var driver = data.Drivers.FirstOrDefault(d => d.Id == driverId)
                     ?? throw new NotFoundException("el conductor", driverId);

        var trips = data.Trips.Where(t => t.DriverId == driverId).ToList();
        var tripIds = trips.Select(t => t.Id).ToHashSet();

        var fuel = data.FuelLogs.Where(f => f.DriverId == driverId || (f.TripId is not null && tripIds.Contains(f.TripId.Value))).ToList();
        var expenses = data.Expenses.Where(e => e.DriverId == driverId || (e.TripId is not null && tripIds.Contains(e.TripId.Value))).ToList();

        var completed = trips.Where(t => t.Status == TripStatus.Completed).ToList();
        var distance = trips.Sum(t => t.EffectiveDistance);
        var revenue = trips.Sum(t => t.FreightRevenue);
        var driverPay = trips.Sum(t => t.DriverPayAmount);
        var fuelCost = fuel.Sum(f => f.TotalCost);
        var fuelQty = fuel.Sum(f => f.Quantity);
        var otherExpenses = expenses.Sum(e => e.Amount);
        var totalCost = fuelCost + otherExpenses + driverPay;

        return new DriverPerformanceDto(
            DriverId: driver.Id,
            DriverName: driver.FullName,
            Period: window,
            Trips: trips.Count,
            CompletedTrips: completed.Count,
            LateTrips: completed.Count(t => t.IsLate),
            Distance: Round(distance),
            FuelQuantity: Round(fuelQty),
            FuelCost: Round(fuelCost),
            Revenue: Round(revenue),
            DriverPay: Round(driverPay),
            OtherExpenses: Round(otherExpenses),
            TotalCost: Round(totalCost),
            Profit: Round(revenue - totalCost),
            ProfitMargin: Percentage(revenue - totalCost, revenue),
            AverageFuelEfficiency: Ratio(distance, ConsumedFuel(trips, fuel)),
            OnTimeRate: OnTimeRate(completed),
            HoursWorked: Round(trips.Sum(t => t.DriverHours ?? 0m)),
            DistanceByDay: SeriesByDay(window, trips, t => t.EffectiveDistance),
            RevenueByDay: SeriesByDay(window, trips, t => t.FreightRevenue),
            RecentTrips: []);
    }

    public async Task<FinanceReportDto> GetFinanceReportAsync(AnalyticsPeriod? period, CancellationToken ct = default)
    {
        var window = Normalize(period);
        var data = await dataSource.LoadAsync(window.FromUtc, window.ToUtc, ct);

        var costByDay = BuildCostByDay(window, data);

        return new FinanceReportDto(
            Period: window,
            Summary: Summarize(data),
            ExpensesByCategory: data.Expenses
                .GroupBy(e => e.CategoryName)
                .Select(g => new NamedValueDto(g.Key, Round(g.Sum(e => e.Amount))))
                .OrderByDescending(x => x.Value)
                .ToList(),
            CostByVehicle: BuildCostByVehicle(data),
            RevenueByCustomer: data.Trips
                .Where(t => t.Status != TripStatus.Cancelled)
                .GroupBy(t => t.CustomerName ?? "Sin cliente asignado")
                .Select(g => new NamedValueDto(g.Key, Round(g.Sum(t => t.FreightRevenue))))
                .OrderByDescending(x => x.Value)
                .ToList(),
            RevenueByDay: SeriesByDay(window, data.Trips, t => t.FreightRevenue),
            CostByDay: costByDay,
            Payroll: BuildPayroll(data));
    }

    // ---- Construcción de bloques -----------------------------------------

    private static FleetStatusDto BuildFleetStatus(AnalyticsDataSet data)
    {
        var vehicles = data.Vehicles.Where(v => v.IsActive).ToList();
        var drivers = data.Drivers.Where(d => d.IsActive).ToList();

        var onTrip = vehicles.Count(v => v.Status == VehicleStatus.OnTrip);
        var operational = vehicles.Count(v => v.Status is VehicleStatus.Available or VehicleStatus.OnTrip);

        return new FleetStatusDto(
            TotalVehicles: vehicles.Count,
            Available: vehicles.Count(v => v.Status == VehicleStatus.Available),
            OnTrip: onTrip,
            InMaintenance: vehicles.Count(v => v.Status == VehicleStatus.InMaintenance),
            OutOfService: vehicles.Count(v => v.Status == VehicleStatus.OutOfService),
            MotorizedUnits: vehicles.Count(v => v.Category == VehicleCategory.Motorized),
            TowedUnits: vehicles.Count(v => v.Category == VehicleCategory.Towed),
            TotalDrivers: drivers.Count,
            DriversActive: drivers.Count(d => d.Status == DriverStatus.Active),
            DriversOnTrip: drivers.Count(d => d.Status == DriverStatus.OnTrip),
            UtilizationRate: Percentage(onTrip, operational));
    }

    private static TripActivityDto BuildActivity(AnalyticsDataSet data, DateOnly today)
    {
        var trips = data.Trips;
        var completed = trips.Where(t => t.Status == TripStatus.Completed).ToList();
        var distance = trips.Where(t => t.Status != TripStatus.Cancelled).Sum(t => t.EffectiveDistance);
        var counted = trips.Count(t => t.Status != TripStatus.Cancelled);

        return new TripActivityDto(
            Planned: trips.Count(t => t.Status == TripStatus.Planned),
            InProgress: trips.Count(t => t.Status == TripStatus.InProgress),
            CompletedInPeriod: completed.Count,
            Cancelled: trips.Count(t => t.Status == TripStatus.Cancelled),
            DeparturesToday: trips.Count(t => t.ActualDepartureUtc is not null &&
                                              DateOnly.FromDateTime(t.ActualDepartureUtc.Value.UtcDateTime) == today),
            ArrivalsToday: trips.Count(t => t.ActualArrivalUtc is not null &&
                                            DateOnly.FromDateTime(t.ActualArrivalUtc.Value.UtcDateTime) == today),
            LateArrivals: completed.Count(t => t.IsLate),
            OnTimeRate: OnTimeRate(completed),
            TotalDistance: Round(distance),
            AverageDistancePerTrip: counted == 0 ? 0m : Round(distance / counted));
    }

    private static FinancialSummaryDto Summarize(AnalyticsDataSet data)
    {
        var trips = data.Trips.Where(t => t.Status != TripStatus.Cancelled).ToList();

        var revenue = trips.Sum(t => t.FreightRevenue);
        var driverPay = trips.Sum(t => t.DriverPayAmount);
        var fuelCost = data.FuelLogs.Sum(f => f.TotalCost);
        var fuelQty = data.FuelLogs.Sum(f => f.Quantity);
        var otherExpenses = data.Expenses.Sum(e => e.Amount);
        var maintenance = data.Maintenance.Sum(m => m.Cost);
        var totalCost = fuelCost + otherExpenses + driverPay + maintenance;
        var distance = trips.Sum(t => t.EffectiveDistance);

        return new FinancialSummaryDto(
            Revenue: Round(revenue),
            FuelCost: Round(fuelCost),
            FuelQuantity: Round(fuelQty),
            OtherExpenses: Round(otherExpenses),
            DriverPay: Round(driverPay),
            MaintenanceCost: Round(maintenance),
            TotalCost: Round(totalCost),
            Profit: Round(revenue - totalCost),
            ProfitMargin: Percentage(revenue - totalCost, revenue),
            RevenuePerDistanceUnit: Ratio(revenue, distance),
            CostPerDistanceUnit: Ratio(totalCost, distance),
            AverageFuelEfficiency: Ratio(distance, ConsumedFuel(trips, data.FuelLogs)));
    }

    private static IReadOnlyList<NamedValueDto> BuildCostBreakdown(AnalyticsDataSet data)
    {
        var trips = data.Trips.Where(t => t.Status != TripStatus.Cancelled).ToList();
        var breakdown = new List<NamedValueDto>
        {
            new("Combustible", Round(data.FuelLogs.Sum(f => f.TotalCost))),
            new("Pago a operadores", Round(trips.Sum(t => t.DriverPayAmount))),
            new("Mantenimiento", Round(data.Maintenance.Sum(m => m.Cost)))
        };

        breakdown.AddRange(data.Expenses
            .GroupBy(e => e.CategoryName)
            .Select(g => new NamedValueDto(g.Key, Round(g.Sum(e => e.Amount)))));

        return breakdown.Where(x => x.Value > 0).OrderByDescending(x => x.Value).ToList();
    }

    private static IReadOnlyList<NamedValueDto> BuildCostByVehicle(AnalyticsDataSet data)
    {
        var labels = data.Vehicles.ToDictionary(v => v.Id, v => v.Label);
        var costs = new Dictionary<Guid, decimal>();

        void Accumulate(Guid? vehicleId, decimal amount)
        {
            if (vehicleId is null) return;
            costs[vehicleId.Value] = costs.GetValueOrDefault(vehicleId.Value) + amount;
        }

        foreach (var f in data.FuelLogs) Accumulate(f.VehicleId, f.TotalCost);
        foreach (var m in data.Maintenance) Accumulate(m.VehicleId, m.Cost);

        // Un gasto puede venir amarrado al viaje y no a la unidad: se reasigna
        // a la unidad que hizo ese viaje para que el costo por camión sea completo.
        var tripVehicle = data.Trips.ToDictionary(t => t.Id, t => t.VehicleId);
        foreach (var e in data.Expenses)
        {
            var vehicleId = e.VehicleId ?? (e.TripId is not null && tripVehicle.TryGetValue(e.TripId.Value, out var v) ? v : null);
            Accumulate(vehicleId, e.Amount);
        }
        foreach (var t in data.Trips.Where(t => t.Status != TripStatus.Cancelled))
            Accumulate(t.VehicleId, t.DriverPayAmount);

        return costs
            .Select(kv => new NamedValueDto(labels.GetValueOrDefault(kv.Key, "Unidad desconocida"), Round(kv.Value)))
            .OrderByDescending(x => x.Value)
            .ToList();
    }

    private static IReadOnlyList<DriverPayrollRowDto> BuildPayroll(AnalyticsDataSet data) =>
        data.Trips
            .Where(t => t.Status != TripStatus.Cancelled)
            .GroupBy(t => new { t.DriverId, t.DriverName })
            .Select(g => new DriverPayrollRowDto(
                g.Key.DriverId,
                g.Key.DriverName,
                g.Count(),
                Round(g.Sum(t => t.DriverHours ?? 0m)),
                Round(g.Sum(t => t.EffectiveDistance)),
                Round(g.Sum(t => t.DriverPayAmount)),
                DescribeScheme(g.Select(t => t.DriverPayScheme).Distinct().ToList())))
            .OrderByDescending(r => r.Amount)
            .ToList();

    private static string DescribeScheme(IReadOnlyList<DriverPayScheme> schemes) => schemes.Count switch
    {
        0 => "—",
        1 => SchemeLabel(schemes[0]),
        _ => "Mixto"
    };

    private static string SchemeLabel(DriverPayScheme scheme) => scheme switch
    {
        DriverPayScheme.PerHour => "Por hora",
        DriverPayScheme.PerKilometer => "Por distancia",
        DriverPayScheme.FixedPerTrip => "Fijo por viaje",
        DriverPayScheme.PercentageOfRevenue => "% del flete",
        _ => scheme.ToString()
    };

    private static IReadOnlyList<DriverRankingRowDto> BuildRanking(AnalyticsDataSet data, DriverRankingCriteria criteria, int take)
    {
        var tripVehicleless = data.Trips.Where(t => t.Status != TripStatus.Cancelled).ToList();
        var tripToDriver = tripVehicleless.ToDictionary(t => t.Id, t => t.DriverId);

        decimal FuelCostFor(Guid driverId) => data.FuelLogs
            .Where(f => f.DriverId == driverId || (f.TripId is not null && tripToDriver.GetValueOrDefault(f.TripId.Value) == driverId))
            .Sum(f => f.TotalCost);

        decimal FuelQtyFor(Guid driverId) => data.FuelLogs
            .Where(f => f.DriverId == driverId || (f.TripId is not null && tripToDriver.GetValueOrDefault(f.TripId.Value) == driverId))
            .Sum(f => f.Quantity);

        decimal ExpensesFor(Guid driverId) => data.Expenses
            .Where(e => e.DriverId == driverId || (e.TripId is not null && tripToDriver.GetValueOrDefault(e.TripId.Value) == driverId))
            .Sum(e => e.Amount);

        var rows = tripVehicleless
            .GroupBy(t => new { t.DriverId, t.DriverName })
            .Select(g =>
            {
                var trips = g.ToList();
                var distance = trips.Sum(t => t.EffectiveDistance);
                var revenue = trips.Sum(t => t.FreightRevenue);
                var pay = trips.Sum(t => t.DriverPayAmount);
                var fuelCost = FuelCostFor(g.Key.DriverId);
                var consumed = ConsumedFuel(trips, data.FuelLogs.Where(f =>
                    f.DriverId == g.Key.DriverId || (f.TripId is not null && tripToDriver.GetValueOrDefault(f.TripId.Value) == g.Key.DriverId)).ToList());
                var completed = trips.Where(t => t.Status == TripStatus.Completed).ToList();

                return new
                {
                    g.Key.DriverId,
                    g.Key.DriverName,
                    Trips = trips.Count,
                    Distance = Round(distance),
                    Revenue = Round(revenue),
                    FuelCost = Round(fuelCost),
                    Pay = Round(pay),
                    Profit = Round(revenue - (fuelCost + ExpensesFor(g.Key.DriverId) + pay)),
                    Efficiency = Ratio(distance, consumed),
                    OnTime = OnTimeRate(completed),
                    FuelQty = FuelQtyFor(g.Key.DriverId)
                };
            })
            .ToList();

        var ordered = criteria switch
        {
            DriverRankingCriteria.Revenue => rows.OrderByDescending(r => r.Revenue),
            DriverRankingCriteria.Profit => rows.OrderByDescending(r => r.Profit),
            DriverRankingCriteria.Trips => rows.OrderByDescending(r => r.Trips),
            DriverRankingCriteria.FuelEfficiency => rows.OrderByDescending(r => r.Efficiency),
            _ => rows.OrderByDescending(r => r.Distance)
        };

        return ordered
            .Take(take)
            .Select((r, i) => new DriverRankingRowDto(
                i + 1, r.DriverId, r.DriverName, r.Trips, r.Distance, r.Revenue,
                r.FuelCost, r.Pay, r.Profit, r.Efficiency, r.OnTime))
            .ToList();
    }

    private static IReadOnlyList<AlertDto> BuildAlerts(AnalyticsDataSet data, DateOnly today)
    {
        var alerts = new List<AlertDto>();

        foreach (var d in data.Drivers.Where(d => d.IsActive && d.LicenseExpiry is not null).OrderBy(d => d.LicenseExpiry))
        {
            var days = d.LicenseExpiry!.Value.DayNumber - today.DayNumber;
            if (days < 0)
                alerts.Add(new AlertDto("danger", "Licencia vencida", $"{d.FullName} — venció el {d.LicenseExpiry:dd/MM/yyyy}."));
            else if (days <= 30)
                alerts.Add(new AlertDto("warning", "Licencia por vencer", $"{d.FullName} — vence en {days} día(s)."));
        }

        foreach (var v in data.Vehicles.Where(v => v.IsActive && v.InsuranceExpiry is not null).OrderBy(v => v.InsuranceExpiry))
        {
            var days = v.InsuranceExpiry!.Value.DayNumber - today.DayNumber;
            if (days < 0)
                alerts.Add(new AlertDto("danger", "Seguro vencido", $"Unidad {v.Label} — venció el {v.InsuranceExpiry:dd/MM/yyyy}."));
            else if (days <= 30)
                alerts.Add(new AlertDto("warning", "Seguro por vencer", $"Unidad {v.Label} — vence en {days} día(s)."));
        }

        var openOrders = data.Maintenance.Count(m => m.Status != MaintenanceStatus.Closed);
        if (openOrders > 0)
            alerts.Add(new AlertDto("info", "Mantenimiento abierto", $"{openOrders} orden(es) de servicio sin cerrar."));

        var unprofitable = data.Trips.Count(t => t.Status == TripStatus.Completed && t.FreightRevenue > 0 &&
                                                 t.FreightRevenue <= t.DriverPayAmount);
        if (unprofitable > 0)
            alerts.Add(new AlertDto("warning", "Viajes sin margen",
                $"{unprofitable} viaje(s) donde el pago al operador se comió el flete."));

        return alerts.Take(12).ToList();
    }

    // ---- Utilidades de cálculo -------------------------------------------

    private static IReadOnlyList<TimeSeriesPointDto> SeriesByDay(
        AnalyticsPeriod window, IEnumerable<TripFact> trips, Func<TripFact, decimal> selector)
    {
        var totals = trips
            .Where(t => t.Status != TripStatus.Cancelled)
            .GroupBy(t => t.BucketDate)
            .ToDictionary(g => g.Key, g => g.Sum(selector));

        return EachDay(window).Select(d => new TimeSeriesPointDto(d, Round(totals.GetValueOrDefault(d)))).ToList();
    }

    private static IReadOnlyList<TimeSeriesPointDto> BuildProfitByDay(AnalyticsPeriod window, AnalyticsDataSet data)
    {
        var revenue = data.Trips.Where(t => t.Status != TripStatus.Cancelled)
            .GroupBy(t => t.BucketDate).ToDictionary(g => g.Key, g => g.Sum(t => t.FreightRevenue));
        var cost = CostPerDay(data);

        return EachDay(window)
            .Select(d => new TimeSeriesPointDto(d, Round(revenue.GetValueOrDefault(d) - cost.GetValueOrDefault(d))))
            .ToList();
    }

    private static IReadOnlyList<TimeSeriesPointDto> BuildCostByDay(AnalyticsPeriod window, AnalyticsDataSet data)
    {
        var cost = CostPerDay(data);
        return EachDay(window).Select(d => new TimeSeriesPointDto(d, Round(cost.GetValueOrDefault(d)))).ToList();
    }

    private static Dictionary<DateOnly, decimal> CostPerDay(AnalyticsDataSet data)
    {
        var cost = new Dictionary<DateOnly, decimal>();

        void Add(DateOnly day, decimal amount) => cost[day] = cost.GetValueOrDefault(day) + amount;

        foreach (var f in data.FuelLogs) Add(DateOnly.FromDateTime(f.LoadedAtUtc.UtcDateTime), f.TotalCost);
        foreach (var e in data.Expenses) Add(DateOnly.FromDateTime(e.IncurredAtUtc.UtcDateTime), e.Amount);
        foreach (var m in data.Maintenance.Where(m => m.ClosedAtUtc is not null))
            Add(DateOnly.FromDateTime(m.ClosedAtUtc!.Value.UtcDateTime), m.Cost);
        foreach (var t in data.Trips.Where(t => t.Status != TripStatus.Cancelled))
            Add(t.BucketDate, t.DriverPayAmount);

        return cost;
    }

    private static IEnumerable<DateOnly> EachDay(AnalyticsPeriod window)
    {
        var start = DateOnly.FromDateTime(window.FromUtc.UtcDateTime);
        var end = DateOnly.FromDateTime(window.ToUtc.UtcDateTime);
        for (var d = start; d <= end; d = d.AddDays(1)) yield return d;
    }

    /// <summary>
    /// Combustible realmente quemado: solo cuentan los viajes cerrados, porque un
    /// viaje en ruta todavía no reporta el nivel final del tanque.
    /// </summary>
    private static decimal ConsumedFuel(IEnumerable<TripFact> trips, IReadOnlyCollection<FuelFact> fuelLogs)
    {
        decimal consumed = 0m;
        foreach (var t in trips.Where(t => t.Status == TripStatus.Completed && t.FinalFuel is not null))
        {
            var purchased = fuelLogs.Where(f => f.TripId == t.Id).Sum(f => f.Quantity);
            consumed += Math.Max(0m, t.InitialFuel + purchased - t.FinalFuel!.Value);
        }
        return consumed;
    }

    private static decimal OnTimeRate(IReadOnlyCollection<TripFact> completedTrips)
    {
        var measurable = completedTrips.Where(t => t.ScheduledArrivalUtc is not null).ToList();
        return measurable.Count == 0 ? 100m : Percentage(measurable.Count(t => !t.IsLate), measurable.Count);
    }

    private static decimal Percentage(decimal part, decimal whole) =>
        whole == 0 ? 0m : Math.Round(part / whole * 100m, 2, MidpointRounding.AwayFromZero);

    private static decimal Ratio(decimal numerator, decimal denominator) =>
        denominator == 0 ? 0m : Math.Round(numerator / denominator, 2, MidpointRounding.AwayFromZero);

    private static decimal Round(decimal value) => Math.Round(value, 2, MidpointRounding.AwayFromZero);

    private AnalyticsPeriod Normalize(AnalyticsPeriod? period)
    {
        if (period is null) return AnalyticsPeriod.LastDays(clock.UtcNow, DefaultWindowDays);
        return period.FromUtc <= period.ToUtc ? period : new AnalyticsPeriod(period.ToUtc, period.FromUtc);
    }
}
