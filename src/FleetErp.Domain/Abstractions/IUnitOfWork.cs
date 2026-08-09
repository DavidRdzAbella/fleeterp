using FleetErp.Domain.Common;
using FleetErp.Domain.Entities;

namespace FleetErp.Domain.Abstractions;

/// <summary>
/// Transacción de negocio: despachar un viaje toca el viaje, la unidad y el
/// conductor, y las tres cosas deben confirmarse juntas o ninguna.
/// </summary>
public interface IUnitOfWork
{
    IRepository<Tenant> Tenants { get; }
    IRepository<AppUser> Users { get; }
    IRepository<VehicleType> VehicleTypes { get; }
    IRepository<Vehicle> Vehicles { get; }
    IRepository<Driver> Drivers { get; }
    IRepository<Customer> Customers { get; }
    IRepository<Trip> Trips { get; }
    IRepository<FuelLog> FuelLogs { get; }
    IRepository<ExpenseCategory> ExpenseCategories { get; }
    IRepository<Expense> Expenses { get; }
    IRepository<MaintenanceOrder> MaintenanceOrders { get; }
    IRepository<CustomFieldDefinition> CustomFieldDefinitions { get; }

    IRepository<T> Repository<T>() where T : BaseEntity;

    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
