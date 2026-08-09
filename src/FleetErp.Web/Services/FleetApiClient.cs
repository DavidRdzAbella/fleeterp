using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FleetErp.Web.Models;

namespace FleetErp.Web.Services;

/// <summary>
/// Cliente tipado sobre <c>HttpClient</c>. Toda la traducción HTTP ↔ modelo vive
/// aquí: los controladores nunca ven códigos de estado ni JSON.
/// </summary>
public sealed class FleetApiClient(HttpClient http) : IFleetApiClient
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web)
    {
        Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
    };

    // ---- Sesión -----------------------------------------------------------

    public Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken ct = default) =>
        PostAsync<LoginRequest, LoginResponse>("api/auth/login", request, ct);

    // ---- Tableros ---------------------------------------------------------

    public Task<FleetDashboardModel> GetFleetDashboardAsync(DateTimeOffset? fromUtc, DateTimeOffset? toUtc, CancellationToken ct = default) =>
        GetRequiredAsync<FleetDashboardModel>(Url("api/analytics/fleet-dashboard", Period(fromUtc, toUtc)), ct);

    public async Task<IReadOnlyList<DriverRankingRowModel>> GetDriverRankingAsync(
        DriverRankingCriteria criteria, int take, DateTimeOffset? fromUtc, DateTimeOffset? toUtc, CancellationToken ct = default)
    {
        var query = Period(fromUtc, toUtc);
        query["criteria"] = criteria.ToString();
        query["take"] = take.ToString();
        return await GetRequiredAsync<List<DriverRankingRowModel>>(Url("api/analytics/driver-ranking", query), ct);
    }

    public Task<DriverPerformanceModel> GetDriverPerformanceAsync(Guid driverId, DateTimeOffset? fromUtc, DateTimeOffset? toUtc, CancellationToken ct = default) =>
        GetRequiredAsync<DriverPerformanceModel>(Url($"api/drivers/{driverId}/performance", Period(fromUtc, toUtc)), ct);

    public Task<FinanceReportModel> GetFinanceReportAsync(DateTimeOffset? fromUtc, DateTimeOffset? toUtc, CancellationToken ct = default) =>
        GetRequiredAsync<FinanceReportModel>(Url("api/analytics/finance", Period(fromUtc, toUtc)), ct);

    // ---- Viajes -----------------------------------------------------------

    public Task<PagedResult<TripListItemModel>> SearchTripsAsync(
        string? search, TripStatus? status, Guid? driverId, Guid? vehicleId, int page, int pageSize, CancellationToken ct = default)
    {
        var query = Paging(page, pageSize);
        Add(query, "search", search);
        Add(query, "status", status?.ToString());
        Add(query, "driverId", driverId?.ToString());
        Add(query, "vehicleId", vehicleId?.ToString());
        return GetRequiredAsync<PagedResult<TripListItemModel>>(Url("api/trips", query), ct);
    }

    public async Task<IReadOnlyList<TripListItemModel>> GetActiveTripsAsync(int max, CancellationToken ct = default) =>
        await GetRequiredAsync<List<TripListItemModel>>($"api/trips/active?max={max}", ct);

    public Task<TripDetailModel?> GetTripAsync(Guid id, CancellationToken ct = default) =>
        GetOptionalAsync<TripDetailModel>($"api/trips/{id}", ct);

    public Task<Guid> CreateTripAsync(CreateTripRequest request, CancellationToken ct = default) =>
        PostForIdAsync("api/trips", request, ct);

    public Task UpdateTripAsync(Guid id, UpdateTripRequest request, CancellationToken ct = default) =>
        SendAsync(HttpMethod.Put, $"api/trips/{id}", request, ct);

    public Task DispatchTripAsync(Guid id, DispatchTripRequest request, CancellationToken ct = default) =>
        SendAsync(HttpMethod.Post, $"api/trips/{id}/dispatch", request, ct);

    public Task CompleteTripAsync(Guid id, CompleteTripRequest request, CancellationToken ct = default) =>
        SendAsync(HttpMethod.Post, $"api/trips/{id}/complete", request, ct);

    public Task CancelTripAsync(Guid id, CancelTripRequest request, CancellationToken ct = default) =>
        SendAsync(HttpMethod.Post, $"api/trips/{id}/cancel", request, ct);

    // ---- Unidades ---------------------------------------------------------

    public Task<PagedResult<VehicleModel>> SearchVehiclesAsync(
        string? search, VehicleStatus? status, Guid? typeId, int page, int pageSize, CancellationToken ct = default)
    {
        var query = Paging(page, pageSize);
        Add(query, "search", search);
        Add(query, "status", status?.ToString());
        Add(query, "vehicleTypeId", typeId?.ToString());
        return GetRequiredAsync<PagedResult<VehicleModel>>(Url("api/vehicles", query), ct);
    }

    public Task<VehicleModel?> GetVehicleAsync(Guid id, CancellationToken ct = default) =>
        GetOptionalAsync<VehicleModel>($"api/vehicles/{id}", ct);

    public async Task<IReadOnlyList<LookupItemModel>> GetVehicleLookupAsync(VehicleCategory? category, CancellationToken ct = default)
    {
        var suffix = category is null ? string.Empty : $"?category={category}";
        return await GetRequiredAsync<List<LookupItemModel>>($"api/vehicles/lookup{suffix}", ct);
    }

    public Task<Guid> CreateVehicleAsync(CreateVehicleRequest request, CancellationToken ct = default) =>
        PostForIdAsync("api/vehicles", request, ct);

    public Task UpdateVehicleAsync(Guid id, UpdateVehicleRequest request, CancellationToken ct = default) =>
        SendAsync(HttpMethod.Put, $"api/vehicles/{id}", request, ct);

    public Task ChangeVehicleStatusAsync(Guid id, VehicleStatus status, CancellationToken ct = default) =>
        SendAsync<object>(HttpMethod.Post, $"api/vehicles/{id}/status?status={status}", null, ct);

    // ---- Conductores ------------------------------------------------------

    public Task<PagedResult<DriverModel>> SearchDriversAsync(
        string? search, DriverStatus? status, int page, int pageSize, CancellationToken ct = default)
    {
        var query = Paging(page, pageSize);
        Add(query, "search", search);
        Add(query, "status", status?.ToString());
        return GetRequiredAsync<PagedResult<DriverModel>>(Url("api/drivers", query), ct);
    }

    public Task<DriverModel?> GetDriverAsync(Guid id, CancellationToken ct = default) =>
        GetOptionalAsync<DriverModel>($"api/drivers/{id}", ct);

    public async Task<IReadOnlyList<LookupItemModel>> GetDriverLookupAsync(CancellationToken ct = default) =>
        await GetRequiredAsync<List<LookupItemModel>>("api/drivers/lookup", ct);

    public Task<Guid> CreateDriverAsync(UpsertDriverRequest request, CancellationToken ct = default) =>
        PostForIdAsync("api/drivers", request, ct);

    public Task UpdateDriverAsync(Guid id, UpsertDriverRequest request, CancellationToken ct = default) =>
        SendAsync(HttpMethod.Put, $"api/drivers/{id}", request, ct);

    // ---- Clientes ---------------------------------------------------------

    public Task<PagedResult<CustomerModel>> SearchCustomersAsync(string? search, int page, int pageSize, CancellationToken ct = default)
    {
        var query = Paging(page, pageSize);
        Add(query, "search", search);
        return GetRequiredAsync<PagedResult<CustomerModel>>(Url("api/customers", query), ct);
    }

    public Task<CustomerModel?> GetCustomerAsync(Guid id, CancellationToken ct = default) =>
        GetOptionalAsync<CustomerModel>($"api/customers/{id}", ct);

    public async Task<IReadOnlyList<LookupItemModel>> GetCustomerLookupAsync(CancellationToken ct = default) =>
        await GetRequiredAsync<List<LookupItemModel>>("api/customers/lookup", ct);

    public Task<Guid> CreateCustomerAsync(UpsertCustomerRequest request, CancellationToken ct = default) =>
        PostForIdAsync("api/customers", request, ct);

    public Task UpdateCustomerAsync(Guid id, UpsertCustomerRequest request, CancellationToken ct = default) =>
        SendAsync(HttpMethod.Put, $"api/customers/{id}", request, ct);

    public Task SetCustomerActiveAsync(Guid id, bool active, CancellationToken ct = default) =>
        SendAsync<object>(HttpMethod.Post, $"api/customers/{id}/active?active={active}", null, ct);

    public Task SetDriverActiveAsync(Guid id, bool active, CancellationToken ct = default) =>
        SendAsync<object>(HttpMethod.Post, $"api/drivers/{id}/active?active={active}", null, ct);

    public Task SetVehicleActiveAsync(Guid id, bool active, CancellationToken ct = default) =>
        SendAsync<object>(HttpMethod.Post, $"api/vehicles/{id}/active?active={active}", null, ct);

    // ---- Combustible y gastos --------------------------------------------

    public Task<PagedResult<FuelLogModel>> SearchFuelLogsAsync(Guid? vehicleId, Guid? tripId, int page, int pageSize, CancellationToken ct = default)
    {
        var query = Paging(page, pageSize);
        Add(query, "vehicleId", vehicleId?.ToString());
        Add(query, "tripId", tripId?.ToString());
        return GetRequiredAsync<PagedResult<FuelLogModel>>(Url("api/fuel-logs", query), ct);
    }

    public Task<FuelLogModel?> GetFuelLogAsync(Guid id, CancellationToken ct = default) =>
        GetOptionalAsync<FuelLogModel>($"api/fuel-logs/{id}", ct);

    public Task<Guid> CreateFuelLogAsync(CreateFuelLogRequest request, CancellationToken ct = default) =>
        PostForIdAsync("api/fuel-logs", request, ct);

    public Task UpdateFuelLogAsync(Guid id, UpdateFuelLogRequest request, CancellationToken ct = default) =>
        SendAsync(HttpMethod.Put, $"api/fuel-logs/{id}", request, ct);

    public Task DeleteFuelLogAsync(Guid id, CancellationToken ct = default) =>
        SendAsync<object>(HttpMethod.Delete, $"api/fuel-logs/{id}", null, ct);

    public Task<PagedResult<ExpenseModel>> SearchExpensesAsync(
        Guid? categoryId, Guid? tripId, DateTimeOffset? fromUtc, DateTimeOffset? toUtc, int page, int pageSize, CancellationToken ct = default)
    {
        var query = Paging(page, pageSize);
        Add(query, "categoryId", categoryId?.ToString());
        Add(query, "tripId", tripId?.ToString());
        Add(query, "fromUtc", fromUtc?.ToString("O"));
        Add(query, "toUtc", toUtc?.ToString("O"));
        return GetRequiredAsync<PagedResult<ExpenseModel>>(Url("api/expenses", query), ct);
    }

    public Task<ExpenseModel?> GetExpenseAsync(Guid id, CancellationToken ct = default) =>
        GetOptionalAsync<ExpenseModel>($"api/expenses/{id}", ct);

    public Task<Guid> CreateExpenseAsync(CreateExpenseRequest request, CancellationToken ct = default) =>
        PostForIdAsync("api/expenses", request, ct);

    public Task UpdateExpenseAsync(Guid id, UpdateExpenseRequest request, CancellationToken ct = default) =>
        SendAsync(HttpMethod.Put, $"api/expenses/{id}", request, ct);

    public Task DeleteExpenseAsync(Guid id, CancellationToken ct = default) =>
        SendAsync<object>(HttpMethod.Delete, $"api/expenses/{id}", null, ct);

    // ---- Mantenimiento ----------------------------------------------------

    public Task<PagedResult<MaintenanceOrderModel>> SearchMaintenanceAsync(
        Guid? vehicleId, MaintenanceStatus? status, int page, int pageSize, CancellationToken ct = default)
    {
        var query = Paging(page, pageSize);
        Add(query, "vehicleId", vehicleId?.ToString());
        Add(query, "status", status?.ToString());
        return GetRequiredAsync<PagedResult<MaintenanceOrderModel>>(Url("api/maintenance", query), ct);
    }

    public Task<MaintenanceOrderModel?> GetMaintenanceAsync(Guid id, CancellationToken ct = default) =>
        GetOptionalAsync<MaintenanceOrderModel>($"api/maintenance/{id}", ct);

    public Task<Guid> CreateMaintenanceAsync(CreateMaintenanceOrderRequest request, CancellationToken ct = default) =>
        PostForIdAsync("api/maintenance", request, ct);

    public Task StartMaintenanceAsync(Guid id, CancellationToken ct = default) =>
        SendAsync<object>(HttpMethod.Post, $"api/maintenance/{id}/start", null, ct);

    public Task CloseMaintenanceAsync(Guid id, CloseMaintenanceOrderRequest request, CancellationToken ct = default) =>
        SendAsync(HttpMethod.Post, $"api/maintenance/{id}/close", request, ct);

    // ---- Usuarios ---------------------------------------------------------

    public Task<PagedResult<UserModel>> SearchUsersAsync(string? search, int page, int pageSize, CancellationToken ct = default)
    {
        var query = Paging(page, pageSize);
        Add(query, "search", search);
        return GetRequiredAsync<PagedResult<UserModel>>(Url("api/users", query), ct);
    }

    public Task<UserModel?> GetUserAsync(Guid id, CancellationToken ct = default) =>
        GetOptionalAsync<UserModel>($"api/users/{id}", ct);

    public Task<Guid> CreateUserAsync(CreateUserRequest request, CancellationToken ct = default) =>
        PostForIdAsync("api/users", request, ct);

    public Task UpdateUserAsync(Guid id, UpdateUserRequest request, CancellationToken ct = default) =>
        SendAsync(HttpMethod.Put, $"api/users/{id}", request, ct);

    public Task ChangeUserPasswordAsync(Guid id, ChangePasswordRequest request, CancellationToken ct = default) =>
        SendAsync(HttpMethod.Post, $"api/users/{id}/password", request, ct);

    public Task SetUserActiveAsync(Guid id, bool active, CancellationToken ct = default) =>
        SendAsync<object>(HttpMethod.Post, $"api/users/{id}/active?active={active}", null, ct);

    // ---- Parametrización --------------------------------------------------

    public Task<TenantModel> GetTenantAsync(CancellationToken ct = default) =>
        GetRequiredAsync<TenantModel>("api/tenant", ct);

    public Task UpdateTenantAsync(UpdateTenantRequest request, CancellationToken ct = default) =>
        SendAsync(HttpMethod.Put, "api/tenant", request, ct);

    public Task UpdateTenantSettingsAsync(TenantSettingsModel settings, CancellationToken ct = default) =>
        SendAsync(HttpMethod.Put, "api/tenant/settings", settings, ct);

    public async Task<IReadOnlyList<VehicleTypeModel>> GetVehicleTypesAsync(bool includeInactive, CancellationToken ct = default) =>
        await GetRequiredAsync<List<VehicleTypeModel>>($"api/catalogs/vehicle-types?includeInactive={includeInactive}", ct);

    public Task<Guid> CreateVehicleTypeAsync(UpsertVehicleTypeRequest request, CancellationToken ct = default) =>
        PostForIdAsync("api/catalogs/vehicle-types", request, ct);

    public Task UpdateVehicleTypeAsync(Guid id, UpsertVehicleTypeRequest request, CancellationToken ct = default) =>
        SendAsync(HttpMethod.Put, $"api/catalogs/vehicle-types/{id}", request, ct);

    public Task SetVehicleTypeActiveAsync(Guid id, bool active, CancellationToken ct = default) =>
        SendAsync<object>(HttpMethod.Post, $"api/catalogs/vehicle-types/{id}/active?active={active}", null, ct);

    public async Task<IReadOnlyList<ExpenseCategoryModel>> GetExpenseCategoriesAsync(bool includeInactive, CancellationToken ct = default) =>
        await GetRequiredAsync<List<ExpenseCategoryModel>>($"api/catalogs/expense-categories?includeInactive={includeInactive}", ct);

    public Task<Guid> CreateExpenseCategoryAsync(UpsertExpenseCategoryRequest request, CancellationToken ct = default) =>
        PostForIdAsync("api/catalogs/expense-categories", request, ct);

    public Task UpdateExpenseCategoryAsync(Guid id, UpsertExpenseCategoryRequest request, CancellationToken ct = default) =>
        SendAsync(HttpMethod.Put, $"api/catalogs/expense-categories/{id}", request, ct);

    public Task SetExpenseCategoryActiveAsync(Guid id, bool active, CancellationToken ct = default) =>
        SendAsync<object>(HttpMethod.Post, $"api/catalogs/expense-categories/{id}/active?active={active}", null, ct);

    public async Task<IReadOnlyList<CustomFieldDefinitionModel>> GetCustomFieldsAsync(CustomFieldTarget? target, CancellationToken ct = default)
    {
        var suffix = target is null ? string.Empty : $"?target={target}";
        return await GetRequiredAsync<List<CustomFieldDefinitionModel>>($"api/catalogs/custom-fields{suffix}", ct);
    }

    public Task<Guid> CreateCustomFieldAsync(UpsertCustomFieldDefinitionRequest request, CancellationToken ct = default) =>
        PostForIdAsync("api/catalogs/custom-fields", request, ct);

    public Task UpdateCustomFieldAsync(Guid id, UpsertCustomFieldDefinitionRequest request, CancellationToken ct = default) =>
        SendAsync(HttpMethod.Put, $"api/catalogs/custom-fields/{id}", request, ct);

    public Task SetCustomFieldActiveAsync(Guid id, bool active, CancellationToken ct = default) =>
        SendAsync<object>(HttpMethod.Post, $"api/catalogs/custom-fields/{id}/active?active={active}", null, ct);

    // ---- Plomería ---------------------------------------------------------

    private async Task<T> GetRequiredAsync<T>(string url, CancellationToken ct)
    {
        using var response = await http.GetAsync(url, ct);
        await EnsureSuccessAsync(response, ct);
        return await response.Content.ReadFromJsonAsync<T>(Json, ct)
               ?? throw new ApiException(HttpStatusCode.InternalServerError, "Respuesta vacía del servidor.", null);
    }

    private async Task<T?> GetOptionalAsync<T>(string url, CancellationToken ct) where T : class
    {
        using var response = await http.GetAsync(url, ct);
        if (response.StatusCode == HttpStatusCode.NotFound) return null;
        await EnsureSuccessAsync(response, ct);
        return await response.Content.ReadFromJsonAsync<T>(Json, ct);
    }

    private async Task<TResponse> PostAsync<TRequest, TResponse>(string url, TRequest body, CancellationToken ct)
    {
        using var response = await http.PostAsJsonAsync(url, body, Json, ct);
        await EnsureSuccessAsync(response, ct);
        return await response.Content.ReadFromJsonAsync<TResponse>(Json, ct)
               ?? throw new ApiException(HttpStatusCode.InternalServerError, "Respuesta vacía del servidor.", null);
    }

    private async Task<Guid> PostForIdAsync<TRequest>(string url, TRequest body, CancellationToken ct)
    {
        using var response = await http.PostAsJsonAsync(url, body, Json, ct);
        await EnsureSuccessAsync(response, ct);
        var created = await response.Content.ReadFromJsonAsync<CreatedResource>(Json, ct);
        return created?.Id ?? Guid.Empty;
    }

    private async Task SendAsync<TRequest>(HttpMethod method, string url, TRequest? body, CancellationToken ct)
    {
        using var request = new HttpRequestMessage(method, url);
        if (body is not null) request.Content = JsonContent.Create(body, options: Json);

        using var response = await http.SendAsync(request, ct);
        await EnsureSuccessAsync(response, ct);
    }

    /// <summary>Convierte el <c>ProblemDetails</c> de la API en una excepción con mensaje presentable.</summary>
    private static async Task EnsureSuccessAsync(HttpResponseMessage response, CancellationToken ct)
    {
        if (response.IsSuccessStatusCode) return;

        ProblemResponse? problem = null;
        try
        {
            problem = await response.Content.ReadFromJsonAsync<ProblemResponse>(Json, ct);
        }
        catch (Exception)
        {
            // Un error de infraestructura puede no traer cuerpo JSON; se usa el fallback.
        }

        throw new ApiException(
            response.StatusCode,
            problem?.Title ?? "No fue posible completar la operación.",
            problem?.Detail,
            problem?.Errors);
    }

    private static Dictionary<string, string?> Paging(int page, int pageSize) =>
        new() { ["page"] = page.ToString(), ["pageSize"] = pageSize.ToString() };

    private static Dictionary<string, string?> Period(DateTimeOffset? fromUtc, DateTimeOffset? toUtc)
    {
        var query = new Dictionary<string, string?>();
        Add(query, "fromUtc", fromUtc?.ToString("O"));
        Add(query, "toUtc", toUtc?.ToString("O"));
        return query;
    }

    private static void Add(IDictionary<string, string?> query, string key, string? value)
    {
        if (!string.IsNullOrWhiteSpace(value)) query[key] = value;
    }

    private static string Url(string path, IDictionary<string, string?> query) =>
        query.Count == 0
            ? path
            : $"{path}?{string.Join('&', query.Where(kv => kv.Value is not null)
                .Select(kv => $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value!)}"))}";

    private sealed record CreatedResource(Guid Id);

    private sealed record ProblemResponse(string? Title, string? Detail, int? Status, Dictionary<string, string[]>? Errors);
}
