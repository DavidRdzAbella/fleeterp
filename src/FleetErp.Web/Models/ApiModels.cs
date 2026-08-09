namespace FleetErp.Web.Models;

/// <summary>
/// Espejo de los contratos de la API. El portal define sus propios tipos en lugar
/// de referenciar los proyectos del backend: eso es lo que permite desplegarlos
/// por separado y versionarlos a distinto ritmo. El precio es esta duplicación
/// deliberada, y el contrato que las mantiene alineadas es el JSON de la API.
/// </summary>
public enum TripStatus { Planned, InProgress, Completed, Cancelled }

public enum VehicleStatus { Available, OnTrip, InMaintenance, OutOfService }

public enum VehicleCategory { Motorized, Towed }

public enum DriverStatus { Active, OnTrip, OnLeave, Inactive }

public enum DriverPayScheme { PerHour, PerKilometer, FixedPerTrip, PercentageOfRevenue }

public enum WeightUnit { Kilogram, Tonne, Pound }

public enum DistanceUnit { Kilometer, Mile }

public enum VolumeUnit { Liter, Gallon }

public enum MaintenanceKind { Preventive, Corrective }

public enum MaintenanceStatus { Open, InProgress, Closed }

public enum UserRole { Administrator, Dispatcher, Viewer }

public enum CustomFieldType { Text, Number, Date, Boolean, Select }

public enum CustomFieldTarget { Trip, Vehicle, Driver, Customer }

public enum DriverRankingCriteria { Distance, Revenue, Profit, Trips, FuelEfficiency }

// ---- Sesión ---------------------------------------------------------------

public sealed record LoginRequest(string TenantSlug, string Email, string Password);

public sealed record LoginResponse(
    string Token, DateTimeOffset ExpiresAtUtc, Guid UserId, string FullName, string Email,
    UserRole Role, Guid TenantId, string TenantName, string TenantSlug, TenantSettingsModel Settings);

public sealed record TenantSettingsModel(
    string CurrencyCode, string CurrencySymbol, string TimeZoneId, string Locale,
    DistanceUnit DistanceUnit, VolumeUnit VolumeUnit, WeightUnit WeightUnit,
    DriverPayScheme DefaultDriverPayScheme, decimal DefaultDriverPayRate, decimal DefaultFuelPricePerUnit,
    string TripFolioPrefix, string BrandPrimaryColor, string? LogoUrl,
    int LicenseExpiryAlertDays, decimal MinAcceptableFuelEfficiency)
{
    public static TenantSettingsModel Fallback() => new(
        "MXN", "$", "America/Mexico_City", "es-MX",
        DistanceUnit.Kilometer, VolumeUnit.Liter, WeightUnit.Kilogram,
        DriverPayScheme.PerHour, 0m, 0m, "VJ", "#0E7C66", null, 30, 2m);
}

public sealed record TenantModel(
    Guid Id, string Name, string Slug, string? TaxId, string? ContactEmail, string? Phone,
    bool IsActive, TenantSettingsModel Settings);

public sealed record UpdateTenantRequest(string Name, string? TaxId, string? ContactEmail, string? Phone);

// ---- Usuarios -------------------------------------------------------------

public sealed record UserModel(
    Guid Id, string Email, string FullName, UserRole Role,
    DateTimeOffset? LastLoginUtc, DateTimeOffset CreatedAtUtc, bool IsActive);

public sealed record CreateUserRequest(string Email, string FullName, UserRole Role, string Password);

public sealed record UpdateUserRequest(string Email, string FullName, UserRole Role);

public sealed record ChangePasswordRequest(string Password);

// ---- Paginación -----------------------------------------------------------

public sealed record PagedResult<T>(IReadOnlyList<T> Items, int Page, int PageSize, int TotalCount)
{
    public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPrevious => Page > 1;
    public bool HasNext => Page < TotalPages;

    public static PagedResult<T> Empty() => new([], 1, 20, 0);
}

public sealed record LookupItemModel(Guid Id, string Label, string? Detail);

// ---- Catálogos ------------------------------------------------------------

public sealed record VehicleTypeModel(Guid Id, string Code, string Name, VehicleCategory Category, bool IsActive);

public sealed record UpsertVehicleTypeRequest(string Code, string Name, VehicleCategory Category);

public sealed record ExpenseCategoryModel(Guid Id, string Code, string Name, bool IsTripRelated, bool IsActive);

public sealed record UpsertExpenseCategoryRequest(string Code, string Name, bool IsTripRelated);

public sealed record CustomFieldDefinitionModel(
    Guid Id, CustomFieldTarget Target, string Key, string Label, CustomFieldType Type,
    bool IsRequired, IReadOnlyList<string> Options, int DisplayOrder, bool IsActive);

public sealed record UpsertCustomFieldDefinitionRequest(
    CustomFieldTarget Target, string Key, string Label, CustomFieldType Type,
    bool IsRequired, string? Options, int DisplayOrder);

// ---- Flotilla -------------------------------------------------------------

public sealed record VehicleModel(
    Guid Id, string EconomicNumber, string PlateNumber,
    Guid VehicleTypeId, string VehicleTypeName, VehicleCategory Category,
    string? Brand, string? Model, int? Year, string? Vin,
    decimal? CargoCapacity, decimal? TankCapacity, decimal CurrentOdometer, VehicleStatus Status,
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

public sealed record DriverModel(
    Guid Id, string FirstName, string LastName, string FullName,
    string? EmployeeNumber, string LicenseNumber, string? LicenseType, DateOnly? LicenseExpiry,
    bool LicenseExpiringSoon, string? Phone, string? Email, DateOnly? HireDate,
    DriverPayScheme PayScheme, decimal PayRate, DriverStatus Status,
    IReadOnlyDictionary<string, string?> CustomFields, bool IsActive);

public sealed record UpsertDriverRequest(
    string FirstName, string LastName, string? EmployeeNumber,
    string LicenseNumber, string? LicenseType, DateOnly? LicenseExpiry,
    string? Phone, string? Email, DateOnly? HireDate,
    DriverPayScheme PayScheme, decimal PayRate, Dictionary<string, string?>? CustomFields);

public sealed record CustomerModel(
    Guid Id, string Name, string? TaxId, string? ContactName, string? Phone, string? Email, string? Address,
    IReadOnlyDictionary<string, string?> CustomFields, bool IsActive);

public sealed record UpsertCustomerRequest(
    string Name, string? TaxId, string? ContactName, string? Phone, string? Email, string? Address,
    Dictionary<string, string?>? CustomFields);

public sealed record MaintenanceOrderModel(
    Guid Id, string Folio, Guid VehicleId, string VehicleLabel,
    MaintenanceKind Kind, MaintenanceStatus Status,
    DateTimeOffset OpenedAtUtc, DateTimeOffset? ClosedAtUtc,
    string Description, string? Workshop, decimal Cost, decimal? OdometerAtService);

public sealed record CreateMaintenanceOrderRequest(
    Guid VehicleId, MaintenanceKind Kind, DateTimeOffset OpenedAtUtc, string Description);

public sealed record CloseMaintenanceOrderRequest(
    DateTimeOffset ClosedAtUtc, decimal Cost, string? Workshop, decimal? OdometerAtService);
