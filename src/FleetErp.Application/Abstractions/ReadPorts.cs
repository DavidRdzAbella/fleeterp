using FleetErp.Application.Common;
using FleetErp.Application.Contracts;
using FleetErp.Domain.Enums;

namespace FleetErp.Application.Abstractions;

/// <summary>
/// Lado de lectura (CQRS ligero). Los listados necesitan joins y proyecciones que
/// no tiene sentido reconstruir en memoria, así que se declaran como puertos aquí
/// y se implementan con EF Core en infraestructura. La capa de aplicación sigue
/// sin conocer al ORM.
/// </summary>
public interface ITripQueries
{
    Task<PagedResult<TripListItemDto>> SearchAsync(TripFilter filter, PageQuery page, CancellationToken ct = default);
    Task<TripDetailDto?> GetDetailAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<TripListItemDto>> GetActiveAsync(int max, CancellationToken ct = default);
}

public interface IVehicleQueries
{
    Task<PagedResult<VehicleDto>> SearchAsync(VehicleFilter filter, PageQuery page, CancellationToken ct = default);
    Task<VehicleDto?> GetAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<LookupItemDto>> LookupAsync(VehicleCategory? category, CancellationToken ct = default);
}

public interface IDriverQueries
{
    Task<PagedResult<DriverDto>> SearchAsync(DriverFilter filter, PageQuery page, CancellationToken ct = default);
    Task<DriverDto?> GetAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<LookupItemDto>> LookupAsync(CancellationToken ct = default);
}

public interface ICustomerQueries
{
    Task<PagedResult<CustomerDto>> SearchAsync(string? search, bool? isActive, PageQuery page, CancellationToken ct = default);
    Task<CustomerDto?> GetAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<LookupItemDto>> LookupAsync(CancellationToken ct = default);
}

public interface IExpenseQueries
{
    Task<PagedResult<ExpenseDto>> SearchAsync(ExpenseFilter filter, PageQuery page, CancellationToken ct = default);
    Task<ExpenseDto?> GetAsync(Guid id, CancellationToken ct = default);
}

public interface IFuelLogQueries
{
    Task<PagedResult<FuelLogDto>> SearchAsync(Guid? vehicleId, Guid? tripId, DateTimeOffset? fromUtc, DateTimeOffset? toUtc, PageQuery page, CancellationToken ct = default);
    Task<FuelLogDto?> GetAsync(Guid id, CancellationToken ct = default);
}

public interface IUserQueries
{
    Task<PagedResult<UserDto>> SearchAsync(UserFilter filter, PageQuery page, CancellationToken ct = default);
    Task<UserDto?> GetAsync(Guid id, CancellationToken ct = default);
}

public interface IMaintenanceQueries
{
    Task<PagedResult<MaintenanceOrderDto>> SearchAsync(Guid? vehicleId, MaintenanceStatus? status, PageQuery page, CancellationToken ct = default);
    Task<MaintenanceOrderDto?> GetAsync(Guid id, CancellationToken ct = default);
}
