using System.Linq.Expressions;
using FleetErp.Domain.Abstractions;
using FleetErp.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace FleetErp.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementación única del puerto de persistencia. No hay un repositorio por
/// entidad porque no habría nada distinto que escribir en cada uno; lo específico
/// de cada agregado vive en el dominio o en el lado de lectura.
/// </summary>
public class EfRepository<T>(FleetDbContext context) : IRepository<T> where T : BaseEntity
{
    protected FleetDbContext Context { get; } = context;
    protected DbSet<T> Set => Context.Set<T>();

    public Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        Set.FirstOrDefaultAsync(e => e.Id == id, ct);

    public async Task<IReadOnlyList<T>> ListAsync(CancellationToken ct = default) =>
        await Set.AsNoTracking().ToListAsync(ct);

    public async Task<IReadOnlyList<T>> ListAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default) =>
        await Set.Where(predicate).ToListAsync(ct);

    public Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default) =>
        Set.AnyAsync(predicate, ct);

    public Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default) =>
        Set.CountAsync(predicate, ct);

    public async Task AddAsync(T entity, CancellationToken ct = default) => await Set.AddAsync(entity, ct);

    public void Update(T entity) => Set.Update(entity);

    public void Remove(T entity) => Set.Remove(entity);

    public IQueryable<T> Query() => Set.AsQueryable();
}
