using FleetErp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FleetErp.Infrastructure.Persistence.Configurations;

public sealed class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.ToTable("vehicles");
        builder.HasKey(v => v.Id);

        builder.Property(v => v.EconomicNumber).HasMaxLength(30).IsRequired();
        builder.Property(v => v.PlateNumber).HasMaxLength(20).IsRequired();
        builder.Property(v => v.Brand).HasMaxLength(60);
        builder.Property(v => v.Model).HasMaxLength(60);
        builder.Property(v => v.Vin).HasMaxLength(40);

        builder.Property(v => v.CargoCapacity).HasPrecision(18, 3);
        builder.Property(v => v.TankCapacity).HasPrecision(18, 3);
        builder.Property(v => v.CurrentOdometer).HasPrecision(18, 2);

        builder.ConfigureCustomFields(v => v.CustomFields);

        builder.HasOne(v => v.VehicleType)
            .WithMany()
            .HasForeignKey(v => v.VehicleTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(v => new { v.TenantId, v.EconomicNumber }).IsUnique();
        builder.HasIndex(v => new { v.TenantId, v.PlateNumber }).IsUnique();
        builder.HasIndex(v => new { v.TenantId, v.Status });
    }
}

public sealed class DriverConfiguration : IEntityTypeConfiguration<Driver>
{
    public void Configure(EntityTypeBuilder<Driver> builder)
    {
        builder.ToTable("drivers");
        builder.HasKey(d => d.Id);

        builder.Property(d => d.FirstName).HasMaxLength(60).IsRequired();
        builder.Property(d => d.LastName).HasMaxLength(60).IsRequired();
        builder.Property(d => d.EmployeeNumber).HasMaxLength(30);
        builder.Property(d => d.LicenseNumber).HasMaxLength(40).IsRequired();
        builder.Property(d => d.LicenseType).HasMaxLength(30);
        builder.Property(d => d.Phone).HasMaxLength(30);
        builder.Property(d => d.Email).HasMaxLength(150);
        builder.Property(d => d.PayRate).HasPrecision(18, 2);

        builder.Ignore(d => d.FullName);
        builder.ConfigureCustomFields(d => d.CustomFields);

        builder.HasIndex(d => new { d.TenantId, d.LicenseNumber }).IsUnique();
        builder.HasIndex(d => new { d.TenantId, d.Status });
    }
}

public sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("customers");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name).HasMaxLength(150).IsRequired();
        builder.Property(c => c.TaxId).HasMaxLength(30);
        builder.Property(c => c.ContactName).HasMaxLength(120);
        builder.Property(c => c.Phone).HasMaxLength(30);
        builder.Property(c => c.Email).HasMaxLength(150);
        builder.Property(c => c.Address).HasMaxLength(300);

        builder.ConfigureCustomFields(c => c.CustomFields);
        builder.HasIndex(c => new { c.TenantId, c.Name });
    }
}

public sealed class TripConfiguration : IEntityTypeConfiguration<Trip>
{
    public void Configure(EntityTypeBuilder<Trip> builder)
    {
        builder.ToTable("trips");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Folio).HasMaxLength(30).IsRequired();
        builder.Property(t => t.Origin).HasMaxLength(150).IsRequired();
        builder.Property(t => t.Destination).HasMaxLength(150).IsRequired();
        builder.Property(t => t.CargoDescription).HasMaxLength(300);
        builder.Property(t => t.Notes).HasMaxLength(1000);
        builder.Property(t => t.CancellationReason).HasMaxLength(300);

        builder.Property(t => t.PlannedDistance).HasPrecision(18, 2);
        builder.Property(t => t.OdometerStart).HasPrecision(18, 2);
        builder.Property(t => t.OdometerEnd).HasPrecision(18, 2);
        builder.Property(t => t.InitialFuel).HasPrecision(18, 3);
        builder.Property(t => t.FinalFuel).HasPrecision(18, 3);
        builder.Property(t => t.CargoWeight).HasPrecision(18, 3);
        builder.Property(t => t.FreightRevenue).HasPrecision(18, 2);
        builder.Property(t => t.DriverPayRate).HasPrecision(18, 2);
        builder.Property(t => t.DriverHours).HasPrecision(10, 2);
        builder.Property(t => t.DriverPayAmount).HasPrecision(18, 2);

        // Cálculos derivados: se resuelven en memoria a partir de las columnas
        // capturadas, no se almacenan, para que no puedan quedar desincronizados.
        builder.Ignore(t => t.ActualDistance);
        builder.Ignore(t => t.EffectiveDistance);
        builder.Ignore(t => t.FuelPurchased);
        builder.Ignore(t => t.FuelConsumed);
        builder.Ignore(t => t.FuelEfficiency);
        builder.Ignore(t => t.Duration);
        builder.Ignore(t => t.FuelCost);
        builder.Ignore(t => t.OtherExpensesCost);
        builder.Ignore(t => t.TotalCost);
        builder.Ignore(t => t.Profit);
        builder.Ignore(t => t.ProfitMargin);
        builder.Ignore(t => t.IsLate);

        builder.ConfigureCustomFields(t => t.CustomFields);

        builder.HasOne(t => t.Driver).WithMany().HasForeignKey(t => t.DriverId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(t => t.Vehicle).WithMany().HasForeignKey(t => t.VehicleId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(t => t.Trailer).WithMany().HasForeignKey(t => t.TrailerId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(t => t.Customer).WithMany().HasForeignKey(t => t.CustomerId).OnDelete(DeleteBehavior.Restrict);

        builder.Metadata.FindNavigation(nameof(Trip.FuelLogs))!.SetPropertyAccessMode(PropertyAccessMode.Field);
        builder.Metadata.FindNavigation(nameof(Trip.Expenses))!.SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(t => new { t.TenantId, t.Folio }).IsUnique();
        builder.HasIndex(t => new { t.TenantId, t.Status });
        builder.HasIndex(t => new { t.TenantId, t.ScheduledDepartureUtc });
        builder.HasIndex(t => new { t.TenantId, t.DriverId });
    }
}

public sealed class FuelLogConfiguration : IEntityTypeConfiguration<FuelLog>
{
    public void Configure(EntityTypeBuilder<FuelLog> builder)
    {
        builder.ToTable("fuel_logs");
        builder.HasKey(f => f.Id);

        builder.Property(f => f.Quantity).HasPrecision(18, 3);
        builder.Property(f => f.PricePerUnit).HasPrecision(18, 4);
        builder.Property(f => f.TotalCost).HasPrecision(18, 2);
        builder.Property(f => f.OdometerReading).HasPrecision(18, 2);
        builder.Property(f => f.Station).HasMaxLength(120);
        builder.Property(f => f.ReferenceNumber).HasMaxLength(60);

        builder.HasOne(f => f.Vehicle).WithMany().HasForeignKey(f => f.VehicleId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(f => f.Driver).WithMany().HasForeignKey(f => f.DriverId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(f => f.Trip).WithMany(t => t.FuelLogs).HasForeignKey(f => f.TripId).OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(f => new { f.TenantId, f.LoadedAtUtc });
        builder.HasIndex(f => new { f.TenantId, f.VehicleId });
    }
}

public sealed class ExpenseConfiguration : IEntityTypeConfiguration<Expense>
{
    public void Configure(EntityTypeBuilder<Expense> builder)
    {
        builder.ToTable("expenses");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Amount).HasPrecision(18, 2);
        builder.Property(e => e.Description).HasMaxLength(250).IsRequired();
        builder.Property(e => e.ReferenceNumber).HasMaxLength(60);

        builder.HasOne(e => e.Category).WithMany().HasForeignKey(e => e.CategoryId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.Vehicle).WithMany().HasForeignKey(e => e.VehicleId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.Driver).WithMany().HasForeignKey(e => e.DriverId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.Trip).WithMany(t => t.Expenses).HasForeignKey(e => e.TripId).OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => new { e.TenantId, e.IncurredAtUtc });
        builder.HasIndex(e => new { e.TenantId, e.CategoryId });
    }
}

public sealed class MaintenanceOrderConfiguration : IEntityTypeConfiguration<MaintenanceOrder>
{
    public void Configure(EntityTypeBuilder<MaintenanceOrder> builder)
    {
        builder.ToTable("maintenance_orders");
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Folio).HasMaxLength(30).IsRequired();
        builder.Property(m => m.Description).HasMaxLength(500).IsRequired();
        builder.Property(m => m.Workshop).HasMaxLength(150);
        builder.Property(m => m.Cost).HasPrecision(18, 2);
        builder.Property(m => m.OdometerAtService).HasPrecision(18, 2);

        builder.HasOne(m => m.Vehicle).WithMany().HasForeignKey(m => m.VehicleId).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(m => new { m.TenantId, m.Folio }).IsUnique();
        builder.HasIndex(m => new { m.TenantId, m.Status });
    }
}
