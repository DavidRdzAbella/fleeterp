using FleetErp.Application.Abstractions;
using FleetErp.Application.Common;
using FleetErp.Application.Contracts;
using FleetErp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FleetErp.Infrastructure.Persistence.Queries;

public sealed class TripQueries(FleetDbContext context) : ITripQueries
{
    public async Task<PagedResult<TripListItemDto>> SearchAsync(TripFilter filter, PageQuery page, CancellationToken ct = default)
    {
        var query = Filtered(filter);
        var total = await query.CountAsync(ct);

        var trips = await Hydrate(query)
            .OrderByDescending(t => t.ScheduledDepartureUtc)
            .ThenByDescending(t => t.Folio)
            .Skip(page.Skip)
            .Take(page.SafePageSize)
            .ToListAsync(ct);

        return new PagedResult<TripListItemDto>(
            trips.Select(t => t.ToListItem()).ToList(), page.SafePage, page.SafePageSize, total);
    }

    public async Task<TripDetailDto?> GetDetailAsync(Guid id, CancellationToken ct = default)
    {
        var trip = await Hydrate(context.Trips.AsNoTracking())
            .Include(t => t.FuelLogs).ThenInclude(f => f.Vehicle)
            .Include(t => t.Expenses).ThenInclude(e => e.Category)
            .FirstOrDefaultAsync(t => t.Id == id, ct);

        return trip?.ToDetail();
    }

    public async Task<IReadOnlyList<TripListItemDto>> GetActiveAsync(int max, CancellationToken ct = default)
    {
        var trips = await Hydrate(context.Trips.AsNoTracking())
            .Where(t => t.Status == Domain.Enums.TripStatus.InProgress)
            .OrderBy(t => t.ActualDepartureUtc)
            .Take(max <= 0 ? 10 : max)
            .ToListAsync(ct);

        return trips.Select(t => t.ToListItem()).ToList();
    }

    /// <summary>
    /// Los totales del viaje (costo y utilidad) los calcula el dominio a partir de
    /// sus cargas y gastos, así que ambas colecciones deben venir cargadas o los
    /// importes saldrían en cero.
    /// </summary>
    private static IQueryable<Trip> Hydrate(IQueryable<Trip> query) => query
        .Include(t => t.Driver)
        .Include(t => t.Vehicle)
        .Include(t => t.Trailer)
        .Include(t => t.Customer)
        .Include(t => t.FuelLogs)
        .Include(t => t.Expenses);

    private IQueryable<Trip> Filtered(TripFilter filter)
    {
        var query = context.Trips.AsNoTracking();

        if (filter.Status is not null) query = query.Where(t => t.Status == filter.Status);
        if (filter.DriverId is not null) query = query.Where(t => t.DriverId == filter.DriverId);
        if (filter.VehicleId is not null) query = query.Where(t => t.VehicleId == filter.VehicleId || t.TrailerId == filter.VehicleId);
        if (filter.CustomerId is not null) query = query.Where(t => t.CustomerId == filter.CustomerId);
        if (filter.FromUtc is not null) query = query.Where(t => t.ScheduledDepartureUtc >= filter.FromUtc);
        if (filter.ToUtc is not null) query = query.Where(t => t.ScheduledDepartureUtc <= filter.ToUtc);

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var term = filter.Search.Trim().ToLower();
            query = query.Where(t =>
                t.Folio.ToLower().Contains(term) ||
                t.Origin.ToLower().Contains(term) ||
                t.Destination.ToLower().Contains(term));
        }

        return query;
    }
}
