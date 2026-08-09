using System.Collections.Concurrent;
using FleetErp.Domain.Abstractions;
using FleetErp.Domain.Common;
using FleetErp.Domain.Entities;

namespace FleetErp.Infrastructure.Persistence.Repositories;

/// <summary>
/// Unidad de trabajo sobre el <see cref="FleetDbContext"/>. Al compartir el mismo
/// contexto, todos los repositorios participan de la misma transacción implícita:
/// un despacho de viaje confirma viaje, unidad y conductor de una sola vez.
/// </summary>
public sealed class UnitOfWork(FleetDbContext context) : IUnitOfWork
{
    private readonly ConcurrentDictionary<Type, object> _repositories = new();

    public IRepository<Tenant> Tenants => Repository<Tenant>();
    public IRepository<AppUser> Users => Repository<AppUser>();
    public IRepository<VehicleType> VehicleTypes => Repository<VehicleType>();
    public IRepository<Vehicle> Vehicles => Repository<Vehicle>();
    public IRepository<Driver> Drivers => Repository<Driver>();
    public IRepository<Customer> Customers => Repository<Customer>();
    public IRepository<Trip> Trips => Repository<Trip>();
    public IRepository<FuelLog> FuelLogs => Repository<FuelLog>();
    public IRepository<ExpenseCategory> ExpenseCategories => Repository<ExpenseCategory>();
    public IRepository<Expense> Expenses => Repository<Expense>();
    public IRepository<MaintenanceOrder> MaintenanceOrders => Repository<MaintenanceOrder>();
    public IRepository<CustomFieldDefinition> CustomFieldDefinitions => Repository<CustomFieldDefinition>();

    public IRepository<T> Repository<T>() where T : BaseEntity =>
        (IRepository<T>)_repositories.GetOrAdd(typeof(T), _ => new EfRepository<T>(context));

    public Task<int> SaveChangesAsync(CancellationToken ct = default) => context.SaveChangesAsync(ct);
}
