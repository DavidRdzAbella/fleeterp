using FleetErp.Application.Abstractions;
using FleetErp.Application.Common;
using FleetErp.Application.Contracts;
using FleetErp.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace FleetErp.Infrastructure.Persistence.Queries;

public sealed class VehicleQueries(FleetDbContext context) : IVehicleQueries
{
    public async Task<PagedResult<VehicleDto>> SearchAsync(VehicleFilter filter, PageQuery page, CancellationToken ct = default)
    {
        var query = context.Vehicles.AsNoTracking().Include(v => v.VehicleType).AsQueryable();

        if (filter.VehicleTypeId is not null) query = query.Where(v => v.VehicleTypeId == filter.VehicleTypeId);
        if (filter.Status is not null) query = query.Where(v => v.Status == filter.Status);
        if (filter.IsActive is not null) query = query.Where(v => v.IsActive == filter.IsActive);

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var term = filter.Search.Trim().ToLower();
            query = query.Where(v =>
                v.EconomicNumber.ToLower().Contains(term) ||
                v.PlateNumber.ToLower().Contains(term) ||
                (v.Brand != null && v.Brand.ToLower().Contains(term)) ||
                (v.Model != null && v.Model.ToLower().Contains(term)));
        }

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderBy(v => v.EconomicNumber)
            .Skip(page.Skip).Take(page.SafePageSize)
            .ToListAsync(ct);

        return new PagedResult<VehicleDto>(items.Select(v => v.ToDto()).ToList(), page.SafePage, page.SafePageSize, total);
    }

    public async Task<VehicleDto?> GetAsync(Guid id, CancellationToken ct = default)
    {
        var vehicle = await context.Vehicles.AsNoTracking()
            .Include(v => v.VehicleType)
            .FirstOrDefaultAsync(v => v.Id == id, ct);
        return vehicle?.ToDto();
    }

    public async Task<IReadOnlyList<LookupItemDto>> LookupAsync(VehicleCategory? category, CancellationToken ct = default)
    {
        var query = context.Vehicles.AsNoTracking().Include(v => v.VehicleType)
            .Where(v => v.IsActive);

        if (category is not null) query = query.Where(v => v.VehicleType!.Category == category);

        var items = await query.OrderBy(v => v.EconomicNumber).ToListAsync(ct);

        return items
            .Select(v => new LookupItemDto(v.Id, $"{v.EconomicNumber} · {v.PlateNumber}",
                                           $"{v.VehicleType?.Name} · {StatusLabel(v.Status)}"))
            .ToList();
    }

    private static string StatusLabel(VehicleStatus status) => status switch
    {
        VehicleStatus.Available => "Disponible",
        VehicleStatus.OnTrip => "En viaje",
        VehicleStatus.InMaintenance => "En taller",
        VehicleStatus.OutOfService => "Fuera de servicio",
        _ => status.ToString()
    };
}

public sealed class DriverQueries(FleetDbContext context, IClock clock) : IDriverQueries
{
    private const int DefaultLicenseAlertDays = 30;

    public async Task<PagedResult<DriverDto>> SearchAsync(DriverFilter filter, PageQuery page, CancellationToken ct = default)
    {
        var query = context.Drivers.AsNoTracking().AsQueryable();

        if (filter.Status is not null) query = query.Where(d => d.Status == filter.Status);
        if (filter.IsActive is not null) query = query.Where(d => d.IsActive == filter.IsActive);

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var term = filter.Search.Trim().ToLower();
            query = query.Where(d =>
                d.FirstName.ToLower().Contains(term) ||
                d.LastName.ToLower().Contains(term) ||
                d.LicenseNumber.ToLower().Contains(term) ||
                (d.EmployeeNumber != null && d.EmployeeNumber.ToLower().Contains(term)));
        }

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderBy(d => d.FirstName).ThenBy(d => d.LastName)
            .Skip(page.Skip).Take(page.SafePageSize)
            .ToListAsync(ct);

        return new PagedResult<DriverDto>(
            items.Select(d => d.ToDto(clock.Today, DefaultLicenseAlertDays)).ToList(),
            page.SafePage, page.SafePageSize, total);
    }

    public async Task<DriverDto?> GetAsync(Guid id, CancellationToken ct = default)
    {
        var driver = await context.Drivers.AsNoTracking().FirstOrDefaultAsync(d => d.Id == id, ct);
        return driver?.ToDto(clock.Today, DefaultLicenseAlertDays);
    }

    public async Task<IReadOnlyList<LookupItemDto>> LookupAsync(CancellationToken ct = default)
    {
        var items = await context.Drivers.AsNoTracking()
            .Where(d => d.IsActive)
            .OrderBy(d => d.FirstName).ThenBy(d => d.LastName)
            .ToListAsync(ct);

        return items.Select(d => new LookupItemDto(d.Id, d.FullName, d.LicenseNumber)).ToList();
    }
}

public sealed class CustomerQueries(FleetDbContext context) : ICustomerQueries
{
    public async Task<PagedResult<CustomerDto>> SearchAsync(string? search, bool? isActive, PageQuery page, CancellationToken ct = default)
    {
        var query = context.Customers.AsNoTracking().AsQueryable();

        if (isActive is not null) query = query.Where(c => c.IsActive == isActive);
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(c => c.Name.ToLower().Contains(term) ||
                                     (c.TaxId != null && c.TaxId.ToLower().Contains(term)));
        }

        var total = await query.CountAsync(ct);
        var items = await query.OrderBy(c => c.Name).Skip(page.Skip).Take(page.SafePageSize).ToListAsync(ct);

        return new PagedResult<CustomerDto>(items.Select(c => c.ToDto()).ToList(), page.SafePage, page.SafePageSize, total);
    }

    public async Task<CustomerDto?> GetAsync(Guid id, CancellationToken ct = default)
    {
        var customer = await context.Customers.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id, ct);
        return customer?.ToDto();
    }

    public async Task<IReadOnlyList<LookupItemDto>> LookupAsync(CancellationToken ct = default)
    {
        var items = await context.Customers.AsNoTracking().Where(c => c.IsActive).OrderBy(c => c.Name).ToListAsync(ct);
        return items.Select(c => new LookupItemDto(c.Id, c.Name, c.TaxId)).ToList();
    }
}
