using FleetErp.Domain.Enums;

namespace FleetErp.Application.Contracts;

/// <summary>Fila del tablero de viajes: lo mínimo para operar sin abrir el detalle.</summary>
public sealed record TripListItemDto(
    Guid Id, string Folio, TripStatus Status,
    string DriverName, string VehicleLabel, string? CustomerName,
    string Origin, string Destination,
    DateTimeOffset ScheduledDepartureUtc, DateTimeOffset? ActualDepartureUtc, DateTimeOffset? ActualArrivalUtc,
    decimal PlannedDistance, decimal ActualDistance,
    decimal CargoWeight, WeightUnit CargoWeightUnit,
    decimal FreightRevenue, decimal TotalCost, decimal Profit,
    bool IsLate);

public sealed record TripDetailDto(
    Guid Id, string Folio, TripStatus Status,
    Guid DriverId, string DriverName,
    Guid VehicleId, string VehicleLabel,
    Guid? TrailerId, string? TrailerLabel,
    Guid? CustomerId, string? CustomerName,
    string Origin, string Destination, decimal PlannedDistance,
    DateTimeOffset ScheduledDepartureUtc, DateTimeOffset? ScheduledArrivalUtc,
    DateTimeOffset? ActualDepartureUtc, DateTimeOffset? ActualArrivalUtc,
    decimal? OdometerStart, decimal? OdometerEnd, decimal ActualDistance,
    decimal InitialFuel, decimal? FinalFuel, bool RefuelPlanned,
    decimal FuelPurchased, decimal? FuelConsumed, decimal? FuelEfficiency,
    decimal CargoWeight, WeightUnit CargoWeightUnit, string? CargoDescription,
    decimal FreightRevenue, DriverPayScheme DriverPayScheme, decimal DriverPayRate,
    decimal? DriverHours, decimal DriverPayAmount,
    decimal FuelCost, decimal OtherExpensesCost, decimal TotalCost, decimal Profit, decimal ProfitMargin,
    bool IsLate, string? Notes, string? CancellationReason,
    IReadOnlyDictionary<string, string?> CustomFields,
    IReadOnlyList<FuelLogDto> FuelLogs,
    IReadOnlyList<ExpenseDto> Expenses);

/// <summary>
/// Alta de viaje. Reúne exactamente lo que el cliente describió en el audio:
/// conductor, unidad, destino, kilómetros por recorrer, combustible inicial,
/// si va a cargar gasolina y cuánta carga lleva.
/// </summary>
public sealed record CreateTripRequest(
    Guid DriverId, Guid VehicleId, Guid? TrailerId, Guid? CustomerId,
    string Origin, string Destination, decimal PlannedDistance,
    DateTimeOffset ScheduledDepartureUtc, DateTimeOffset? ScheduledArrivalUtc,
    decimal InitialFuel, bool RefuelPlanned,
    decimal CargoWeight, WeightUnit CargoWeightUnit, string? CargoDescription,
    decimal FreightRevenue, DriverPayScheme? DriverPayScheme, decimal? DriverPayRate,
    string? Notes, Dictionary<string, string?>? CustomFields);

public sealed record UpdateTripRequest(
    Guid DriverId, Guid VehicleId, Guid? TrailerId, Guid? CustomerId,
    string Origin, string Destination, decimal PlannedDistance,
    DateTimeOffset ScheduledDepartureUtc, DateTimeOffset? ScheduledArrivalUtc,
    decimal InitialFuel, bool RefuelPlanned,
    decimal CargoWeight, WeightUnit CargoWeightUnit, string? CargoDescription,
    decimal FreightRevenue, DriverPayScheme DriverPayScheme, decimal DriverPayRate,
    string? Notes, Dictionary<string, string?>? CustomFields);

/// <summary>Salida a ruta.</summary>
public sealed record DispatchTripRequest(DateTimeOffset DepartureUtc, decimal OdometerStart, decimal? InitialFuel);

/// <summary>Llegada.</summary>
public sealed record CompleteTripRequest(DateTimeOffset ArrivalUtc, decimal OdometerEnd, decimal? FinalFuel, decimal? DriverHours);

public sealed record CancelTripRequest(string Reason);

public sealed record TripFilter(
    string? Search, TripStatus? Status, Guid? DriverId, Guid? VehicleId, Guid? CustomerId,
    DateTimeOffset? FromUtc, DateTimeOffset? ToUtc);

// ---- Combustible y gastos -------------------------------------------------

public sealed record FuelLogDto(
    Guid Id, Guid VehicleId, string VehicleLabel, Guid? TripId, string? TripFolio,
    Guid? DriverId, string? DriverName,
    DateTimeOffset LoadedAtUtc, decimal Quantity, decimal PricePerUnit, decimal TotalCost,
    decimal? OdometerReading, string? Station, string? ReferenceNumber);

public sealed record CreateFuelLogRequest(
    Guid VehicleId, Guid? TripId, Guid? DriverId,
    DateTimeOffset LoadedAtUtc, decimal Quantity, decimal PricePerUnit,
    decimal? OdometerReading, string? Station, string? ReferenceNumber);

/// <summary>
/// Corrección de una carga ya capturada. Se declara aparte del alta aunque hoy
/// tenga los mismos campos: son dos operaciones con permisos y bitácora propios,
/// y la de corrección va a divergir en cuanto se exija motivo del ajuste.
/// </summary>
public sealed record UpdateFuelLogRequest(
    Guid VehicleId, Guid? TripId, Guid? DriverId,
    DateTimeOffset LoadedAtUtc, decimal Quantity, decimal PricePerUnit,
    decimal? OdometerReading, string? Station, string? ReferenceNumber);

public sealed record ExpenseDto(
    Guid Id, Guid CategoryId, string CategoryName,
    Guid? TripId, string? TripFolio, Guid? VehicleId, string? VehicleLabel,
    Guid? DriverId, string? DriverName,
    DateTimeOffset IncurredAtUtc, decimal Amount, string Description, string? ReferenceNumber);

public sealed record CreateExpenseRequest(
    Guid CategoryId, Guid? TripId, Guid? VehicleId, Guid? DriverId,
    DateTimeOffset IncurredAtUtc, decimal Amount, string Description, string? ReferenceNumber);

public sealed record UpdateExpenseRequest(
    Guid CategoryId, Guid? TripId, Guid? VehicleId, Guid? DriverId,
    DateTimeOffset IncurredAtUtc, decimal Amount, string Description, string? ReferenceNumber);

public sealed record ExpenseFilter(
    Guid? CategoryId, Guid? TripId, Guid? VehicleId, Guid? DriverId,
    DateTimeOffset? FromUtc, DateTimeOffset? ToUtc);
