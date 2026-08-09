using FleetErp.Domain.Enums;

namespace FleetErp.Application.Contracts;

// ---- Unidades -------------------------------------------------------------

public sealed record VehicleDto(
    Guid Id, string EconomicNumber, string PlateNumber,
    Guid VehicleTypeId, string VehicleTypeName, VehicleCategory Category,
    string? Brand, string? Model, int? Year, string? Vin,
    decimal? CargoCapacity, decimal? TankCapacity,
    decimal CurrentOdometer, VehicleStatus Status,
    DateOnly? InsuranceExpiry, DateOnly? CirculationCardExpiry,
    IReadOnlyDictionary<string, string?> CustomFields, bool IsActive);

public sealed record CreateVehicleRequest(
    string EconomicNumber, string PlateNumber, Guid VehicleTypeId,
    string? Brand, string? Model, int? Year, string? Vin,
    decimal? CargoCapacity, decimal? TankCapacity, decimal InitialOdometer,
    DateOnly? InsuranceExpiry, DateOnly? CirculationCardExpiry,
    Dictionary<string, string?>? CustomFields);

public sealed record UpdateVehicleRequest(
    string EconomicNumber, string PlateNumber, Guid VehicleTypeId,
    string? Brand, string? Model, int? Year, string? Vin,
    decimal? CargoCapacity, decimal? TankCapacity,
    DateOnly? InsuranceExpiry, DateOnly? CirculationCardExpiry,
    Dictionary<string, string?>? CustomFields);

public sealed record VehicleFilter(string? Search, Guid? VehicleTypeId, VehicleStatus? Status, bool? IsActive);

// ---- Conductores ----------------------------------------------------------

public sealed record DriverDto(
    Guid Id, string FirstName, string LastName, string FullName,
    string? EmployeeNumber, string LicenseNumber, string? LicenseType, DateOnly? LicenseExpiry,
    bool LicenseExpiringSoon,
    string? Phone, string? Email, DateOnly? HireDate,
    DriverPayScheme PayScheme, decimal PayRate, DriverStatus Status,
    IReadOnlyDictionary<string, string?> CustomFields, bool IsActive);

public sealed record UpsertDriverRequest(
    string FirstName, string LastName, string? EmployeeNumber,
    string LicenseNumber, string? LicenseType, DateOnly? LicenseExpiry,
    string? Phone, string? Email, DateOnly? HireDate,
    DriverPayScheme PayScheme, decimal PayRate,
    Dictionary<string, string?>? CustomFields);

public sealed record DriverFilter(string? Search, DriverStatus? Status, bool? IsActive);

// ---- Clientes -------------------------------------------------------------

public sealed record CustomerDto(
    Guid Id, string Name, string? TaxId, string? ContactName,
    string? Phone, string? Email, string? Address,
    IReadOnlyDictionary<string, string?> CustomFields, bool IsActive);

public sealed record UpsertCustomerRequest(
    string Name, string? TaxId, string? ContactName,
    string? Phone, string? Email, string? Address,
    Dictionary<string, string?>? CustomFields);

// ---- Mantenimiento --------------------------------------------------------

public sealed record MaintenanceOrderDto(
    Guid Id, string Folio, Guid VehicleId, string VehicleLabel,
    MaintenanceKind Kind, MaintenanceStatus Status,
    DateTimeOffset OpenedAtUtc, DateTimeOffset? ClosedAtUtc,
    string Description, string? Workshop, decimal Cost, decimal? OdometerAtService);

public sealed record CreateMaintenanceOrderRequest(
    Guid VehicleId, MaintenanceKind Kind, DateTimeOffset OpenedAtUtc, string Description);

public sealed record CloseMaintenanceOrderRequest(
    DateTimeOffset ClosedAtUtc, decimal Cost, string? Workshop, decimal? OdometerAtService);
