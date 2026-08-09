using FleetErp.Application.Abstractions;
using FleetErp.Application.Common;
using FleetErp.Application.Contracts;
using FleetErp.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace FleetErp.Infrastructure.Persistence.Queries;

public sealed class ExpenseQueries(FleetDbContext context) : IExpenseQueries
{
    public async Task<PagedResult<ExpenseDto>> SearchAsync(ExpenseFilter filter, PageQuery page, CancellationToken ct = default)
    {
        var query = context.Expenses.AsNoTracking()
            .Include(e => e.Category)
            .Include(e => e.Vehicle)
            .Include(e => e.Driver)
            .Include(e => e.Trip)
            .AsQueryable();

        if (filter.CategoryId is not null) query = query.Where(e => e.CategoryId == filter.CategoryId);
        if (filter.TripId is not null) query = query.Where(e => e.TripId == filter.TripId);
        if (filter.VehicleId is not null) query = query.Where(e => e.VehicleId == filter.VehicleId);
        if (filter.DriverId is not null) query = query.Where(e => e.DriverId == filter.DriverId);
        if (filter.FromUtc is not null) query = query.Where(e => e.IncurredAtUtc >= filter.FromUtc);
        if (filter.ToUtc is not null) query = query.Where(e => e.IncurredAtUtc <= filter.ToUtc);

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(e => e.IncurredAtUtc)
            .Skip(page.Skip).Take(page.SafePageSize)
            .ToListAsync(ct);

        return new PagedResult<ExpenseDto>(items.Select(e => e.ToDto()).ToList(), page.SafePage, page.SafePageSize, total);
    }

    public async Task<ExpenseDto?> GetAsync(Guid id, CancellationToken ct = default)
    {
        var expense = await context.Expenses.AsNoTracking()
            .Include(e => e.Category)
            .Include(e => e.Vehicle)
            .Include(e => e.Driver)
            .Include(e => e.Trip)
            .FirstOrDefaultAsync(e => e.Id == id, ct);

        return expense?.ToDto();
    }
}

public sealed class FuelLogQueries(FleetDbContext context) : IFuelLogQueries
{
    public async Task<PagedResult<FuelLogDto>> SearchAsync(
        Guid? vehicleId, Guid? tripId, DateTimeOffset? fromUtc, DateTimeOffset? toUtc,
        PageQuery page, CancellationToken ct = default)
    {
        var query = context.FuelLogs.AsNoTracking()
            .Include(f => f.Vehicle)
            .Include(f => f.Driver)
            .Include(f => f.Trip)
            .AsQueryable();

        if (vehicleId is not null) query = query.Where(f => f.VehicleId == vehicleId);
        if (tripId is not null) query = query.Where(f => f.TripId == tripId);
        if (fromUtc is not null) query = query.Where(f => f.LoadedAtUtc >= fromUtc);
        if (toUtc is not null) query = query.Where(f => f.LoadedAtUtc <= toUtc);

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(f => f.LoadedAtUtc)
            .Skip(page.Skip).Take(page.SafePageSize)
            .ToListAsync(ct);

        return new PagedResult<FuelLogDto>(items.Select(f => f.ToDto()).ToList(), page.SafePage, page.SafePageSize, total);
    }

    public async Task<FuelLogDto?> GetAsync(Guid id, CancellationToken ct = default)
    {
        var log = await context.FuelLogs.AsNoTracking()
            .Include(f => f.Vehicle)
            .Include(f => f.Driver)
            .Include(f => f.Trip)
            .FirstOrDefaultAsync(f => f.Id == id, ct);

        return log?.ToDto();
    }
}

public sealed class MaintenanceQueries(FleetDbContext context) : IMaintenanceQueries
{
    public async Task<PagedResult<MaintenanceOrderDto>> SearchAsync(
        Guid? vehicleId, MaintenanceStatus? status, PageQuery page, CancellationToken ct = default)
    {
        var query = context.MaintenanceOrders.AsNoTracking().Include(m => m.Vehicle).AsQueryable();

        if (vehicleId is not null) query = query.Where(m => m.VehicleId == vehicleId);
        if (status is not null) query = query.Where(m => m.Status == status);

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(m => m.OpenedAtUtc)
            .Skip(page.Skip).Take(page.SafePageSize)
            .ToListAsync(ct);

        return new PagedResult<MaintenanceOrderDto>(items.Select(m => m.ToDto()).ToList(), page.SafePage, page.SafePageSize, total);
    }

    public async Task<MaintenanceOrderDto?> GetAsync(Guid id, CancellationToken ct = default)
    {
        var order = await context.MaintenanceOrders.AsNoTracking().Include(m => m.Vehicle)
            .FirstOrDefaultAsync(m => m.Id == id, ct);
        return order?.ToDto();
    }
}
