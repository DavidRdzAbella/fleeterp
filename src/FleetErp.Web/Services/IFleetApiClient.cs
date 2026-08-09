using FleetErp.Web.Models;

namespace FleetErp.Web.Services;

/// <summary>
/// Único punto de contacto del portal con el backend. Los controladores dependen
/// de esta interfaz y no de <c>HttpClient</c>, de modo que el portal es probable
/// con un doble y no queda atado al transporte.
/// </summary>
public interface IFleetApiClient
{
    Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken ct = default);

    // Tableros
    Task<FleetDashboardModel> GetFleetDashboardAsync(DateTimeOffset? fromUtc, DateTimeOffset? toUtc, CancellationToken ct = default);
    Task<IReadOnlyList<DriverRankingRowModel>> GetDriverRankingAsync(DriverRankingCriteria criteria, int take, DateTimeOffset? fromUtc, DateTimeOffset? toUtc, CancellationToken ct = default);
    Task<DriverPerformanceModel> GetDriverPerformanceAsync(Guid driverId, DateTimeOffset? fromUtc, DateTimeOffset? toUtc, CancellationToken ct = default);
    Task<FinanceReportModel> GetFinanceReportAsync(DateTimeOffset? fromUtc, DateTimeOffset? toUtc, CancellationToken ct = default);

    // Viajes
    Task<PagedResult<TripListItemModel>> SearchTripsAsync(string? search, TripStatus? status, Guid? driverId, Guid? vehicleId, int page, int pageSize, CancellationToken ct = default);
    Task<IReadOnlyList<TripListItemModel>> GetActiveTripsAsync(int max, CancellationToken ct = default);
    Task<TripDetailModel?> GetTripAsync(Guid id, CancellationToken ct = default);
    Task<Guid> CreateTripAsync(CreateTripRequest request, CancellationToken ct = default);
    Task UpdateTripAsync(Guid id, UpdateTripRequest request, CancellationToken ct = default);
    Task DispatchTripAsync(Guid id, DispatchTripRequest request, CancellationToken ct = default);
    Task CompleteTripAsync(Guid id, CompleteTripRequest request, CancellationToken ct = default);
    Task CancelTripAsync(Guid id, CancelTripRequest request, CancellationToken ct = default);

    // Unidades
    Task<PagedResult<VehicleModel>> SearchVehiclesAsync(string? search, VehicleStatus? status, Guid? typeId, int page, int pageSize, CancellationToken ct = default);
    Task<VehicleModel?> GetVehicleAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<LookupItemModel>> GetVehicleLookupAsync(VehicleCategory? category, CancellationToken ct = default);
    Task<Guid> CreateVehicleAsync(CreateVehicleRequest request, CancellationToken ct = default);
    Task UpdateVehicleAsync(Guid id, UpdateVehicleRequest request, CancellationToken ct = default);
    Task ChangeVehicleStatusAsync(Guid id, VehicleStatus status, CancellationToken ct = default);

    // Conductores
    Task<PagedResult<DriverModel>> SearchDriversAsync(string? search, DriverStatus? status, int page, int pageSize, CancellationToken ct = default);
    Task<DriverModel?> GetDriverAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<LookupItemModel>> GetDriverLookupAsync(CancellationToken ct = default);
    Task<Guid> CreateDriverAsync(UpsertDriverRequest request, CancellationToken ct = default);
    Task UpdateDriverAsync(Guid id, UpsertDriverRequest request, CancellationToken ct = default);

    // Clientes
    Task<PagedResult<CustomerModel>> SearchCustomersAsync(string? search, int page, int pageSize, CancellationToken ct = default);
    Task<CustomerModel?> GetCustomerAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<LookupItemModel>> GetCustomerLookupAsync(CancellationToken ct = default);
    Task<Guid> CreateCustomerAsync(UpsertCustomerRequest request, CancellationToken ct = default);
    Task UpdateCustomerAsync(Guid id, UpsertCustomerRequest request, CancellationToken ct = default);

    Task SetCustomerActiveAsync(Guid id, bool active, CancellationToken ct = default);
    Task SetDriverActiveAsync(Guid id, bool active, CancellationToken ct = default);
    Task SetVehicleActiveAsync(Guid id, bool active, CancellationToken ct = default);

    // Combustible y gastos
    Task<PagedResult<FuelLogModel>> SearchFuelLogsAsync(Guid? vehicleId, Guid? tripId, int page, int pageSize, CancellationToken ct = default);
    Task<FuelLogModel?> GetFuelLogAsync(Guid id, CancellationToken ct = default);
    Task<Guid> CreateFuelLogAsync(CreateFuelLogRequest request, CancellationToken ct = default);
    Task UpdateFuelLogAsync(Guid id, UpdateFuelLogRequest request, CancellationToken ct = default);
    Task DeleteFuelLogAsync(Guid id, CancellationToken ct = default);

    Task<PagedResult<ExpenseModel>> SearchExpensesAsync(Guid? categoryId, Guid? tripId, DateTimeOffset? fromUtc, DateTimeOffset? toUtc, int page, int pageSize, CancellationToken ct = default);
    Task<ExpenseModel?> GetExpenseAsync(Guid id, CancellationToken ct = default);
    Task<Guid> CreateExpenseAsync(CreateExpenseRequest request, CancellationToken ct = default);
    Task UpdateExpenseAsync(Guid id, UpdateExpenseRequest request, CancellationToken ct = default);
    Task DeleteExpenseAsync(Guid id, CancellationToken ct = default);

    // Mantenimiento
    Task<PagedResult<MaintenanceOrderModel>> SearchMaintenanceAsync(Guid? vehicleId, MaintenanceStatus? status, int page, int pageSize, CancellationToken ct = default);
    Task<MaintenanceOrderModel?> GetMaintenanceAsync(Guid id, CancellationToken ct = default);
    Task<Guid> CreateMaintenanceAsync(CreateMaintenanceOrderRequest request, CancellationToken ct = default);
    Task StartMaintenanceAsync(Guid id, CancellationToken ct = default);
    Task CloseMaintenanceAsync(Guid id, CloseMaintenanceOrderRequest request, CancellationToken ct = default);

    // Usuarios
    Task<PagedResult<UserModel>> SearchUsersAsync(string? search, int page, int pageSize, CancellationToken ct = default);
    Task<UserModel?> GetUserAsync(Guid id, CancellationToken ct = default);
    Task<Guid> CreateUserAsync(CreateUserRequest request, CancellationToken ct = default);
    Task UpdateUserAsync(Guid id, UpdateUserRequest request, CancellationToken ct = default);
    Task ChangeUserPasswordAsync(Guid id, ChangePasswordRequest request, CancellationToken ct = default);
    Task SetUserActiveAsync(Guid id, bool active, CancellationToken ct = default);

    // Parametrización
    Task<TenantModel> GetTenantAsync(CancellationToken ct = default);
    Task UpdateTenantAsync(UpdateTenantRequest request, CancellationToken ct = default);
    Task UpdateTenantSettingsAsync(TenantSettingsModel settings, CancellationToken ct = default);
    Task<IReadOnlyList<VehicleTypeModel>> GetVehicleTypesAsync(bool includeInactive, CancellationToken ct = default);
    Task<Guid> CreateVehicleTypeAsync(UpsertVehicleTypeRequest request, CancellationToken ct = default);
    Task UpdateVehicleTypeAsync(Guid id, UpsertVehicleTypeRequest request, CancellationToken ct = default);
    Task SetVehicleTypeActiveAsync(Guid id, bool active, CancellationToken ct = default);

    Task<IReadOnlyList<ExpenseCategoryModel>> GetExpenseCategoriesAsync(bool includeInactive, CancellationToken ct = default);
    Task<Guid> CreateExpenseCategoryAsync(UpsertExpenseCategoryRequest request, CancellationToken ct = default);
    Task UpdateExpenseCategoryAsync(Guid id, UpsertExpenseCategoryRequest request, CancellationToken ct = default);
    Task SetExpenseCategoryActiveAsync(Guid id, bool active, CancellationToken ct = default);

    Task<IReadOnlyList<CustomFieldDefinitionModel>> GetCustomFieldsAsync(CustomFieldTarget? target, CancellationToken ct = default);
    Task<Guid> CreateCustomFieldAsync(UpsertCustomFieldDefinitionRequest request, CancellationToken ct = default);
    Task UpdateCustomFieldAsync(Guid id, UpsertCustomFieldDefinitionRequest request, CancellationToken ct = default);
    Task SetCustomFieldActiveAsync(Guid id, bool active, CancellationToken ct = default);
}
