using System.Linq.Expressions;
using FleetErp.Domain.Common;

namespace FleetErp.Domain.Abstractions;

/// <summary>
/// Puerto de persistencia genérico. El dominio y la aplicación dependen de esta
/// abstracción; que detrás haya EF Core sobre PostgreSQL es un detalle de la capa
/// externa (inversión de dependencias).
/// </summary>
public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<T>> ListAsync(CancellationToken ct = default);
    Task<IReadOnlyList<T>> ListAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
    Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
    Task AddAsync(T entity, CancellationToken ct = default);
    void Update(T entity);
    void Remove(T entity);

    /// <summary>
    /// Escotilla para consultas de lectura compuestas (tableros y reportes).
    /// Devuelve <c>IQueryable</c> a propósito: obligar a un método de repositorio
    /// por cada agregación del dashboard sería peor diseño que exponerla acotada.
    /// </summary>
    IQueryable<T> Query();
}
