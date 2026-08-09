using FleetErp.Application.Abstractions;
using FleetErp.Application.Contracts;
using FleetErp.Application.Services;
using FleetErp.Domain.Enums;
using FluentAssertions;

namespace FleetErp.UnitTests.Application;

/// <summary>
/// El tablero es lo que el cliente va a mirar todos los días, así que su
/// aritmética se prueba con datos armados a mano y resultados calculados aparte.
/// Como el servicio recibe los hechos por un puerto, no hace falta base de datos.
/// </summary>
public class AnalyticsServiceTests
{
    private static readonly DateTimeOffset Now = new(2026, 3, 15, 12, 0, 0, TimeSpan.Zero);
    private static readonly Guid Ulises = Guid.NewGuid();
    private static readonly Guid Juanito = Guid.NewGuid();
    private static readonly Guid Tracto = Guid.NewGuid();

    private static AnalyticsPeriod Period => new(new DateTimeOffset(2026, 3, 1, 0, 0, 0, TimeSpan.Zero), Now);

    private static IAnalyticsService Build(AnalyticsDataSet data) =>
        new AnalyticsService(new StubDataSource(data), new StubClock(Now));

    // ---- Datos de prueba --------------------------------------------------

    private static TripFact CompletedTrip(
        Guid tripId, Guid driverId, string driverName, decimal distance, decimal revenue, decimal driverPay,
        int day, decimal initialFuel = 400m, decimal? finalFuel = 100m, bool late = false)
    {
        var departure = new DateTimeOffset(2026, 3, day, 6, 0, 0, TimeSpan.Zero);
        return new TripFact(
            tripId, $"VJ-{day:D3}", TripStatus.Completed,
            driverId, driverName, Tracto, "T-101 · AB-123-CD", null, null,
            departure, departure.AddHours(12), departure, departure.AddHours(late ? 15 : 11),
            distance, distance, initialFuel, finalFuel,
            revenue, driverPay, 11m, DriverPayScheme.PerHour, late);
    }

    private static AnalyticsDataSet DataSet(
        IEnumerable<TripFact>? trips = null,
        IEnumerable<FuelFact>? fuel = null,
        IEnumerable<ExpenseFact>? expenses = null,
        IEnumerable<MaintenanceFact>? maintenance = null,
        IEnumerable<VehicleFact>? vehicles = null,
        IEnumerable<DriverFact>? drivers = null) =>
        new(trips?.ToList() ?? [],
            expenses?.ToList() ?? [],
            fuel?.ToList() ?? [],
            maintenance?.ToList() ?? [],
            vehicles?.ToList() ?? [],
            drivers?.ToList() ?? []);

    // ---- Resumen económico ------------------------------------------------

    [Fact]
    public async Task El_costo_total_suma_combustible_gastos_nomina_y_taller()
    {
        var tripId = Guid.NewGuid();
        var data = DataSet(
            trips: [CompletedTrip(tripId, Ulises, "Ulises Mendoza", distance: 900m, revenue: 30_000m, driverPay: 2_500m, day: 10)],
            fuel: [new FuelFact(Guid.NewGuid(), Tracto, "T-101", tripId, Ulises, Now.AddDays(-5), 250m, 6_500m)],
            expenses: [new ExpenseFact(Guid.NewGuid(), Guid.NewGuid(), "Casetas y peajes", true, tripId, Tracto, Ulises, Now.AddDays(-5), 1_800m)],
            maintenance: [new MaintenanceFact(Guid.NewGuid(), Tracto, MaintenanceStatus.Closed, Now.AddDays(-9), Now.AddDays(-8), 5_000m)]);

        var dashboard = await Build(data).GetFleetDashboardAsync(Period);

        dashboard.Financials.FuelCost.Should().Be(6_500m);
        dashboard.Financials.OtherExpenses.Should().Be(1_800m);
        dashboard.Financials.DriverPay.Should().Be(2_500m);
        dashboard.Financials.MaintenanceCost.Should().Be(5_000m);
        dashboard.Financials.TotalCost.Should().Be(15_800m);
        dashboard.Financials.Revenue.Should().Be(30_000m);
        dashboard.Financials.Profit.Should().Be(14_200m);
    }

    [Fact]
    public async Task Los_viajes_cancelados_no_cuentan_como_venta()
    {
        var cancelled = CompletedTrip(Guid.NewGuid(), Ulises, "Ulises Mendoza", 900m, 30_000m, 2_500m, 10)
            with { Status = TripStatus.Cancelled };

        var dashboard = await Build(DataSet(trips: [cancelled])).GetFleetDashboardAsync(Period);

        dashboard.Financials.Revenue.Should().Be(0m);
        dashboard.Activity.Cancelled.Should().Be(1);
        dashboard.Activity.TotalDistance.Should().Be(0m);
    }

    [Fact]
    public async Task El_rendimiento_promedio_solo_considera_viajes_cerrados_con_tanque_final()
    {
        var closed = Guid.NewGuid();
        var running = Guid.NewGuid();

        var data = DataSet(
            trips:
            [
                // 900 km quemando 400 + 200 − 100 = 500 → 1.8 km/L
                CompletedTrip(closed, Ulises, "Ulises Mendoza", 900m, 30_000m, 0m, 10, initialFuel: 400m, finalFuel: 100m),
                CompletedTrip(running, Juanito, "Juanito Pérez", 500m, 15_000m, 0m, 12) with
                {
                    Status = TripStatus.InProgress, FinalFuel = null
                }
            ],
            fuel: [new FuelFact(Guid.NewGuid(), Tracto, "T-101", closed, Ulises, Now.AddDays(-5), 200m, 5_000m)]);

        var dashboard = await Build(data).GetFleetDashboardAsync(Period);

        // La distancia del periodo sí incluye al viaje en ruta; el rendimiento no.
        dashboard.Activity.TotalDistance.Should().Be(1_400m);
        dashboard.Financials.AverageFuelEfficiency.Should().Be(Math.Round(1_400m / 500m, 2));
    }

    // ---- Actividad --------------------------------------------------------

    [Fact]
    public async Task La_puntualidad_es_el_porcentaje_de_llegadas_a_tiempo()
    {
        var data = DataSet(trips:
        [
            CompletedTrip(Guid.NewGuid(), Ulises, "Ulises Mendoza", 900m, 30_000m, 0m, 10),
            CompletedTrip(Guid.NewGuid(), Ulises, "Ulises Mendoza", 900m, 30_000m, 0m, 11),
            CompletedTrip(Guid.NewGuid(), Ulises, "Ulises Mendoza", 900m, 30_000m, 0m, 12, late: true),
            CompletedTrip(Guid.NewGuid(), Ulises, "Ulises Mendoza", 900m, 30_000m, 0m, 13, late: true)
        ]);

        var dashboard = await Build(data).GetFleetDashboardAsync(Period);

        dashboard.Activity.LateArrivals.Should().Be(2);
        dashboard.Activity.OnTimeRate.Should().Be(50m);
    }

    [Fact]
    public async Task Las_salidas_y_llegadas_de_hoy_se_cuentan_por_separado()
    {
        var departedToday = CompletedTrip(Guid.NewGuid(), Ulises, "Ulises", 400m, 12_000m, 0m, 15) with
        {
            Status = TripStatus.InProgress,
            ActualDepartureUtc = Now.AddHours(-4),
            ActualArrivalUtc = null
        };

        var arrivedToday = CompletedTrip(Guid.NewGuid(), Juanito, "Juanito", 400m, 12_000m, 0m, 14) with
        {
            ActualArrivalUtc = Now.AddHours(-1)
        };

        var dashboard = await Build(DataSet(trips: [departedToday, arrivedToday])).GetFleetDashboardAsync(Period);

        dashboard.Activity.DeparturesToday.Should().Be(1);
        dashboard.Activity.ArrivalsToday.Should().Be(1);
        dashboard.Activity.InProgress.Should().Be(1);
    }

    [Fact]
    public async Task El_estado_de_la_flotilla_ignora_unidades_dadas_de_baja()
    {
        var data = DataSet(vehicles:
        [
            new VehicleFact(Guid.NewGuid(), "T-101", VehicleStatus.Available, VehicleCategory.Motorized, true, null),
            new VehicleFact(Guid.NewGuid(), "T-102", VehicleStatus.OnTrip, VehicleCategory.Motorized, true, null),
            new VehicleFact(Guid.NewGuid(), "C-301", VehicleStatus.InMaintenance, VehicleCategory.Towed, true, null),
            new VehicleFact(Guid.NewGuid(), "T-999", VehicleStatus.Available, VehicleCategory.Motorized, false, null)
        ]);

        var dashboard = await Build(data).GetFleetDashboardAsync(Period);

        dashboard.Fleet.TotalVehicles.Should().Be(3);
        dashboard.Fleet.MotorizedUnits.Should().Be(2);
        dashboard.Fleet.TowedUnits.Should().Be(1);
        // Utilización = en viaje / (disponibles + en viaje); el taller no cuenta.
        dashboard.Fleet.UtilizationRate.Should().Be(50m);
    }

    // ---- Ranking y desempeño ----------------------------------------------

    [Fact]
    public async Task El_ranking_ordena_por_el_criterio_pedido()
    {
        var data = DataSet(trips:
        [
            CompletedTrip(Guid.NewGuid(), Ulises, "Ulises Mendoza", distance: 900m, revenue: 20_000m, driverPay: 0m, day: 10),
            CompletedTrip(Guid.NewGuid(), Juanito, "Juanito Pérez", distance: 400m, revenue: 45_000m, driverPay: 0m, day: 11)
        ]);

        var service = Build(data);

        var byDistance = await service.GetDriverRankingAsync(Period, DriverRankingCriteria.Distance, 10);
        var byRevenue = await service.GetDriverRankingAsync(Period, DriverRankingCriteria.Revenue, 10);

        byDistance[0].DriverName.Should().Be("Ulises Mendoza");
        byDistance[0].Rank.Should().Be(1);
        byRevenue[0].DriverName.Should().Be("Juanito Pérez");
        byRevenue[0].Rank.Should().Be(1);
    }

    [Fact]
    public async Task El_desempeno_de_un_conductor_atribuye_los_costos_de_sus_viajes()
    {
        var tripId = Guid.NewGuid();
        var data = DataSet(
            trips: [CompletedTrip(tripId, Ulises, "Ulises Mendoza", 900m, 30_000m, driverPay: 2_500m, day: 10)],
            // El gasto viene amarrado al viaje, no al conductor: debe atribuírsele igual.
            fuel: [new FuelFact(Guid.NewGuid(), Tracto, "T-101", tripId, null, Now.AddDays(-5), 250m, 6_500m)],
            expenses: [new ExpenseFact(Guid.NewGuid(), Guid.NewGuid(), "Casetas", true, tripId, null, null, Now.AddDays(-5), 1_500m)],
            drivers: [new DriverFact(Ulises, "Ulises Mendoza", DriverStatus.Active, true, null)]);

        var performance = await Build(data).GetDriverPerformanceAsync(Ulises, Period);

        performance.Distance.Should().Be(900m);
        performance.Revenue.Should().Be(30_000m);
        performance.FuelCost.Should().Be(6_500m);
        performance.OtherExpenses.Should().Be(1_500m);
        performance.DriverPay.Should().Be(2_500m);
        performance.TotalCost.Should().Be(10_500m);
        performance.Profit.Should().Be(19_500m);
    }

    [Fact]
    public async Task Pedir_el_desempeno_de_un_conductor_inexistente_falla_claro()
    {
        var act = async () => await Build(AnalyticsDataSet.Empty).GetDriverPerformanceAsync(Guid.NewGuid(), Period);

        await act.Should().ThrowAsync<FleetErp.Application.Common.NotFoundException>();
    }

    // ---- Series y reportes ------------------------------------------------

    [Fact]
    public async Task Las_series_traen_un_punto_por_dia_incluidos_los_dias_sin_movimiento()
    {
        var period = new AnalyticsPeriod(new DateTimeOffset(2026, 3, 10, 0, 0, 0, TimeSpan.Zero), Now);
        var data = DataSet(trips: [CompletedTrip(Guid.NewGuid(), Ulises, "Ulises", 900m, 30_000m, 0m, day: 12)]);

        var dashboard = await Build(data).GetFleetDashboardAsync(period);

        dashboard.DistanceByDay.Should().HaveCount(6); // del 10 al 15
        dashboard.DistanceByDay.Single(p => p.Date == new DateOnly(2026, 3, 12)).Value.Should().Be(900m);
        dashboard.DistanceByDay.Single(p => p.Date == new DateOnly(2026, 3, 11)).Value.Should().Be(0m);
    }

    [Fact]
    public async Task La_nomina_del_reporte_agrupa_por_conductor()
    {
        var data = DataSet(trips:
        [
            CompletedTrip(Guid.NewGuid(), Ulises, "Ulises Mendoza", 900m, 30_000m, driverPay: 1_200m, day: 10),
            CompletedTrip(Guid.NewGuid(), Ulises, "Ulises Mendoza", 800m, 26_000m, driverPay: 1_100m, day: 11),
            CompletedTrip(Guid.NewGuid(), Juanito, "Juanito Pérez", 400m, 12_000m, driverPay: 600m, day: 12)
        ]);

        var report = await Build(data).GetFinanceReportAsync(Period);

        report.Payroll.Should().HaveCount(2);
        report.Payroll[0].DriverName.Should().Be("Ulises Mendoza");
        report.Payroll[0].Trips.Should().Be(2);
        report.Payroll[0].Amount.Should().Be(2_300m);
        report.Payroll[0].Scheme.Should().Be("Por hora");
    }

    [Fact]
    public async Task Sin_movimientos_el_tablero_responde_en_ceros_y_no_truena()
    {
        var dashboard = await Build(AnalyticsDataSet.Empty).GetFleetDashboardAsync(Period);

        dashboard.Financials.Revenue.Should().Be(0m);
        dashboard.Financials.ProfitMargin.Should().Be(0m);
        dashboard.Activity.OnTimeRate.Should().Be(100m);
        dashboard.TopDrivers.Should().BeEmpty();
        dashboard.CostBreakdown.Should().BeEmpty();
    }

    [Fact]
    public async Task Se_avisa_de_licencias_y_seguros_por_vencer()
    {
        var today = DateOnly.FromDateTime(Now.UtcDateTime);
        var data = DataSet(
            vehicles: [new VehicleFact(Tracto, "T-101", VehicleStatus.Available, VehicleCategory.Motorized, true, today.AddDays(10))],
            drivers: [new DriverFact(Ulises, "Ulises Mendoza", DriverStatus.Active, true, today.AddDays(-2))]);

        var dashboard = await Build(data).GetFleetDashboardAsync(Period);

        dashboard.Alerts.Should().Contain(a => a.Title == "Licencia vencida" && a.Severity == "danger");
        dashboard.Alerts.Should().Contain(a => a.Title == "Seguro por vencer" && a.Severity == "warning");
    }

    // ---- Dobles de prueba -------------------------------------------------

    private sealed class StubDataSource(AnalyticsDataSet data) : IAnalyticsDataSource
    {
        public Task<AnalyticsDataSet> LoadAsync(DateTimeOffset fromUtc, DateTimeOffset toUtc, CancellationToken ct = default) =>
            Task.FromResult(data);
    }

    private sealed class StubClock(DateTimeOffset now) : IClock
    {
        public DateTimeOffset UtcNow => now;
        public DateOnly Today => DateOnly.FromDateTime(now.UtcDateTime);
    }
}
