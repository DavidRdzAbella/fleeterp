using FleetErp.Domain.Enums;

namespace FleetErp.Application.Abstractions;

/// <summary>
/// Entrega los hechos crudos del periodo ya aplanados. La agregación (KPIs,
/// rankings, series) se hace en la capa de aplicación sobre estos registros, lo
/// que permite probar toda la matemática del tablero sin base de datos.
/// </summary>
/// <remarks>
/// Traer los movimientos del periodo a memoria es adecuado al volumen de una
/// flotilla pequeña o mediana, que es el caso de uso del MVP. Si una empresa
/// crece, la sustitución natural es una implementación que agregue en SQL
/// (o vistas materializadas) detrás de este mismo puerto, sin tocar el tablero.
/// </remarks>
public interface IAnalyticsDataSource
{
    Task<AnalyticsDataSet> LoadAsync(DateTimeOffset fromUtc, DateTimeOffset toUtc, CancellationToken ct = default);
}

public sealed record AnalyticsDataSet(
    IReadOnlyList<TripFact> Trips,
    IReadOnlyList<ExpenseFact> Expenses,
    IReadOnlyList<FuelFact> FuelLogs,
    IReadOnlyList<MaintenanceFact> Maintenance,
    IReadOnlyList<VehicleFact> Vehicles,
    IReadOnlyList<DriverFact> Drivers)
{
    public static AnalyticsDataSet Empty { get; } = new([], [], [], [], [], []);
}

public sealed record TripFact(
    Guid Id, string Folio, TripStatus Status,
    Guid DriverId, string DriverName,
    Guid VehicleId, string VehicleLabel,
    Guid? CustomerId, string? CustomerName,
    DateTimeOffset ScheduledDepartureUtc, DateTimeOffset? ScheduledArrivalUtc,
    DateTimeOffset? ActualDepartureUtc, DateTimeOffset? ActualArrivalUtc,
    decimal PlannedDistance, decimal ActualDistance,
    decimal InitialFuel, decimal? FinalFuel,
    decimal FreightRevenue, decimal DriverPayAmount, decimal? DriverHours,
    DriverPayScheme DriverPayScheme,
    bool IsLate)
{
    public decimal EffectiveDistance => ActualDistance > 0 ? ActualDistance : PlannedDistance;

    /// <summary>Fecha con la que el viaje entra a las series: la salida real, o la programada si aún no sale.</summary>
    public DateOnly BucketDate => DateOnly.FromDateTime((ActualDepartureUtc ?? ScheduledDepartureUtc).UtcDateTime);
}

public sealed record ExpenseFact(
    Guid Id, Guid CategoryId, string CategoryName, bool IsTripRelated,
    Guid? TripId, Guid? VehicleId, Guid? DriverId,
    DateTimeOffset IncurredAtUtc, decimal Amount);

public sealed record FuelFact(
    Guid Id, Guid VehicleId, string VehicleLabel, Guid? TripId, Guid? DriverId,
    DateTimeOffset LoadedAtUtc, decimal Quantity, decimal TotalCost);

public sealed record MaintenanceFact(
    Guid Id, Guid VehicleId, MaintenanceStatus Status, DateTimeOffset OpenedAtUtc,
    DateTimeOffset? ClosedAtUtc, decimal Cost);

public sealed record VehicleFact(
    Guid Id, string Label, VehicleStatus Status, VehicleCategory Category,
    bool IsActive, DateOnly? InsuranceExpiry);

public sealed record DriverFact(
    Guid Id, string FullName, DriverStatus Status, bool IsActive, DateOnly? LicenseExpiry);
