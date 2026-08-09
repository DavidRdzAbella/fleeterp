using FleetErp.Application.Abstractions;
using FleetErp.Domain.Common;
using FleetErp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FleetErp.Infrastructure.Persistence;

/// <summary>
/// Contexto de datos. Concentra tres responsabilidades transversales que de otro
/// modo se repetirían en cada consulta: aislamiento por empresa, auditoría
/// automática y estampado del identificador de empresa al insertar.
/// </summary>
public sealed class FleetDbContext(
    DbContextOptions<FleetDbContext> options,
    ICurrentTenant currentTenant,
    ICurrentUser currentUser,
    IClock clock) : DbContext(options)
{
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<VehicleType> VehicleTypes => Set<VehicleType>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<Driver> Drivers => Set<Driver>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Trip> Trips => Set<Trip>();
    public DbSet<FuelLog> FuelLogs => Set<FuelLog>();
    public DbSet<ExpenseCategory> ExpenseCategories => Set<ExpenseCategory>();
    public DbSet<Expense> Expenses => Set<Expense>();
    public DbSet<MaintenanceOrder> MaintenanceOrders => Set<MaintenanceOrder>();
    public DbSet<CustomFieldDefinition> CustomFieldDefinitions => Set<CustomFieldDefinition>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FleetDbContext).Assembly);
        ApplyTenantFilters(modelBuilder);
        base.OnModelCreating(modelBuilder);
    }

    /// <summary>
    /// Filtro global por empresa en toda entidad multi-empresa. Es la garantía de
    /// que una consulta mal escrita no puede devolver datos de otro cliente:
    /// el aislamiento no depende de que cada servicio se acuerde de filtrar.
    /// </summary>
    private void ApplyTenantFilters(ModelBuilder modelBuilder)
    {
        var apply = typeof(FleetDbContext).GetMethod(nameof(ApplyTenantFilter),
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!;

        foreach (var entityType in modelBuilder.Model.GetEntityTypes()
                     .Where(t => typeof(ITenantScoped).IsAssignableFrom(t.ClrType)))
        {
            apply.MakeGenericMethod(entityType.ClrType).Invoke(this, [modelBuilder]);
        }
    }

    /// <summary>
    /// El filtro referencia propiedades del propio contexto: EF sustituye esa
    /// referencia por la instancia viva en cada consulta, de modo que el modelo
    /// se puede seguir cacheando aunque la empresa cambie en cada petición.
    /// </summary>
    private void ApplyTenantFilter<TEntity>(ModelBuilder modelBuilder) where TEntity : class, ITenantScoped =>
        modelBuilder.Entity<TEntity>()
            .HasQueryFilter(e => TenantFilterDisabled || e.TenantId == CurrentTenantId);

    /// <summary>Expuesto para que el filtro global lo lea como parámetro en cada consulta.</summary>
    public Guid CurrentTenantId => currentTenant.IsResolved ? currentTenant.TenantId : Guid.Empty;

    public bool TenantFilterDisabled => currentTenant.FilterDisabled;

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        StampTenantAndAudit();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        StampTenantAndAudit();
        return base.SaveChanges();
    }

    private void StampTenantAndAudit()
    {
        var now = clock.UtcNow;
        var actor = currentUser.Email ?? "sistema";

        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is ITenantScoped scoped && entry.State == EntityState.Added && scoped.TenantId == Guid.Empty)
                scoped.TenantId = CurrentTenantId;

            if (entry.Entity is not IAuditable auditable) continue;

            switch (entry.State)
            {
                case EntityState.Added:
                    auditable.CreatedAtUtc = now;
                    auditable.CreatedBy = actor;
                    break;
                case EntityState.Modified:
                    auditable.UpdatedAtUtc = now;
                    auditable.UpdatedBy = actor;
                    break;
            }
        }
    }
}
