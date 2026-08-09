using FleetErp.Application.Abstractions;
using FleetErp.Application.Common;
using FleetErp.Application.Contracts;
using Microsoft.EntityFrameworkCore;

namespace FleetErp.Infrastructure.Persistence.Queries;

public sealed class UserQueries(FleetDbContext context) : IUserQueries
{
    public async Task<PagedResult<UserDto>> SearchAsync(UserFilter filter, PageQuery page, CancellationToken ct = default)
    {
        var query = context.Users.AsNoTracking().AsQueryable();

        if (filter.Role is not null) query = query.Where(u => u.Role == filter.Role);
        if (filter.IsActive is not null) query = query.Where(u => u.IsActive == filter.IsActive);

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var term = filter.Search.Trim().ToLower();
            query = query.Where(u => u.FullName.ToLower().Contains(term) || u.Email.ToLower().Contains(term));
        }

        var total = await query.CountAsync(ct);

        // El hash de contraseña nunca sale del servidor: la proyección solo toma
        // los campos que la pantalla necesita.
        var items = await query
            .OrderBy(u => u.FullName)
            .Skip(page.Skip).Take(page.SafePageSize)
            .Select(u => new UserDto(u.Id, u.Email, u.FullName, u.Role, u.LastLoginUtc, u.CreatedAtUtc, u.IsActive))
            .ToListAsync(ct);

        return new PagedResult<UserDto>(items, page.SafePage, page.SafePageSize, total);
    }

    public Task<UserDto?> GetAsync(Guid id, CancellationToken ct = default) =>
        context.Users.AsNoTracking()
            .Where(u => u.Id == id)
            .Select(u => new UserDto(u.Id, u.Email, u.FullName, u.Role, u.LastLoginUtc, u.CreatedAtUtc, u.IsActive))
            .FirstOrDefaultAsync(ct);
}
