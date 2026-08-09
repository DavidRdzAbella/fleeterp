namespace FleetErp.Application.Contracts;

/// <summary>Rango de análisis. Si no se envía, los servicios usan los últimos 30 días.</summary>
public sealed record AnalyticsPeriod(DateTimeOffset FromUtc, DateTimeOffset ToUtc)
{
    /// <summary>
    /// Ventana móvil que cierra al final del día en curso, no en el instante
    /// actual: de otro modo los viajes ya programados para hoy por la tarde
    /// desaparecerían del tablero de la mañana.
    /// </summary>
    public static AnalyticsPeriod LastDays(DateTimeOffset nowUtc, int days) =>
        new(nowUtc.UtcDateTime.Date.AddDays(-days + 1), EndOfDay(nowUtc));

    public static AnalyticsPeriod CurrentWeek(DateTimeOffset nowUtc)
    {
        var offset = ((int)nowUtc.DayOfWeek + 6) % 7; // semana de lunes a domingo
        return new AnalyticsPeriod(nowUtc.UtcDateTime.Date.AddDays(-offset), EndOfDay(nowUtc));
    }

    private static DateTimeOffset EndOfDay(DateTimeOffset nowUtc) =>
        new(nowUtc.UtcDateTime.Date.AddDays(1).AddTicks(-1), TimeSpan.Zero);

    public int DayCount => Math.Max(1, (int)(ToUtc.UtcDateTime.Date - FromUtc.UtcDateTime.Date).TotalDays + 1);
}

public sealed record TimeSeriesPointDto(DateOnly Date, decimal Value);

public sealed record NamedValueDto(string Label, decimal Value);

/// <summary>
/// Tablero de flotilla: la vista "cómo va toda la flotilla, las entradas y las
/// salidas" que pidió el cliente, resuelta en una sola llamada para que la
/// pantalla no encadene peticiones.
/// </summary>
public sealed record FleetDashboardDto(
    AnalyticsPeriod Period,
    FleetStatusDto Fleet,
    TripActivityDto Activity,
    FinancialSummaryDto Financials,
    IReadOnlyList<TimeSeriesPointDto> DistanceByDay,
    IReadOnlyList<TimeSeriesPointDto> RevenueByDay,
    IReadOnlyList<TimeSeriesPointDto> ProfitByDay,
    IReadOnlyList<NamedValueDto> CostBreakdown,
    IReadOnlyList<DriverRankingRowDto> TopDrivers,
    IReadOnlyList<TripListItemDto> ActiveTrips,
    IReadOnlyList<AlertDto> Alerts);

public sealed record FleetStatusDto(
    int TotalVehicles, int Available, int OnTrip, int InMaintenance, int OutOfService,
    int MotorizedUnits, int TowedUnits,
    int TotalDrivers, int DriversActive, int DriversOnTrip,
    decimal UtilizationRate);

public sealed record TripActivityDto(
    int Planned, int InProgress, int CompletedInPeriod, int Cancelled,
    int DeparturesToday, int ArrivalsToday, int LateArrivals,
    decimal OnTimeRate, decimal TotalDistance, decimal AverageDistancePerTrip);

/// <summary>Resultado económico del periodo. Costo = combustible + gastos de ruta + nómina de operadores.</summary>
public sealed record FinancialSummaryDto(
    decimal Revenue,
    decimal FuelCost,
    decimal FuelQuantity,
    decimal OtherExpenses,
    decimal DriverPay,
    decimal MaintenanceCost,
    decimal TotalCost,
    decimal Profit,
    decimal ProfitMargin,
    decimal RevenuePerDistanceUnit,
    decimal CostPerDistanceUnit,
    decimal AverageFuelEfficiency);

public sealed record AlertDto(string Severity, string Title, string Detail, string? Link = null);

/// <summary>Fila del ranking de conductores ("top 1, 2, 3").</summary>
public sealed record DriverRankingRowDto(
    int Rank, Guid DriverId, string DriverName,
    int Trips, decimal Distance, decimal Revenue, decimal FuelCost,
    decimal DriverPay, decimal Profit, decimal AverageFuelEfficiency, decimal OnTimeRate);

/// <summary>
/// Detalle de un conductor: "Ulises recorrió 5,000 km en la semana, gastó X de
/// combustible, vendió Y y le dejó Z a la empresa".
/// </summary>
public sealed record DriverPerformanceDto(
    Guid DriverId, string DriverName, AnalyticsPeriod Period,
    int Trips, int CompletedTrips, int LateTrips,
    decimal Distance, decimal FuelQuantity, decimal FuelCost,
    decimal Revenue, decimal DriverPay, decimal OtherExpenses,
    decimal TotalCost, decimal Profit, decimal ProfitMargin,
    decimal AverageFuelEfficiency, decimal OnTimeRate, decimal HoursWorked,
    IReadOnlyList<TimeSeriesPointDto> DistanceByDay,
    IReadOnlyList<TimeSeriesPointDto> RevenueByDay,
    IReadOnlyList<TripListItemDto> RecentTrips);

/// <summary>Pantalla de gastos y ganancias, con el desglose que pidió el cliente.</summary>
public sealed record FinanceReportDto(
    AnalyticsPeriod Period,
    FinancialSummaryDto Summary,
    IReadOnlyList<NamedValueDto> ExpensesByCategory,
    IReadOnlyList<NamedValueDto> CostByVehicle,
    IReadOnlyList<NamedValueDto> RevenueByCustomer,
    IReadOnlyList<TimeSeriesPointDto> RevenueByDay,
    IReadOnlyList<TimeSeriesPointDto> CostByDay,
    IReadOnlyList<DriverPayrollRowDto> Payroll);

/// <summary>Lo que se le pagó a cada operador en el periodo y por qué esquema.</summary>
public sealed record DriverPayrollRowDto(
    Guid DriverId, string DriverName, int Trips,
    decimal Hours, decimal Distance, decimal Amount, string Scheme);
