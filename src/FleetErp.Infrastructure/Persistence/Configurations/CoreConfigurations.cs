using FleetErp.Domain.Common;
using FleetErp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FleetErp.Infrastructure.Persistence.Configurations;

/// <summary>
/// Piezas de mapeo reutilizadas por varias entidades. Tenerlas en un solo sitio
/// evita que la precisión del dinero o el formato de los campos configurables
/// se definan distinto en cada tabla.
/// </summary>
internal static class MappingConventions
{
    public const string JsonColumnType = "jsonb";

    public static readonly ValueConverter<CustomFieldValues, string> CustomFieldConverter =
        new(v => JsonHelper.Serialize(v), v => JsonHelper.Deserialize(v));

    public static readonly ValueComparer<CustomFieldValues> CustomFieldComparer =
        new((a, b) => JsonHelper.Serialize(a) == JsonHelper.Serialize(b),
            v => JsonHelper.Serialize(v).GetHashCode(),
            v => JsonHelper.Deserialize(JsonHelper.Serialize(v)));

    /// <summary>Columna JSON con los campos que cada empresa agregó por su cuenta.</summary>
    public static void ConfigureCustomFields<T>(this EntityTypeBuilder<T> builder,
        System.Linq.Expressions.Expression<Func<T, CustomFieldValues>> selector) where T : class
    {
        builder.Property(selector)
            .HasConversion(CustomFieldConverter)
            .Metadata.SetValueComparer(CustomFieldComparer);

        builder.Property(selector).HasColumnName("custom_fields").HasColumnType(JsonColumnType);
    }

    /// <summary>Índice único acotado a la empresa: dos clientes pueden repetir la misma placa.</summary>
    public static void HasTenantUniqueIndex<T>(this EntityTypeBuilder<T> builder,
        System.Linq.Expressions.Expression<Func<T, object?>> selector, string name) where T : class =>
        builder.HasIndex(selector).HasDatabaseName(name).IsUnique();
}

public sealed class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable("tenants");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Name).HasMaxLength(150).IsRequired();
        builder.Property(t => t.Slug).HasMaxLength(60).IsRequired();
        builder.HasIndex(t => t.Slug).IsUnique();
        builder.Property(t => t.TaxId).HasMaxLength(30);
        builder.Property(t => t.ContactEmail).HasMaxLength(150);
        builder.Property(t => t.Phone).HasMaxLength(30);

        // La parametrización viaja como documento: agregar un ajuste nuevo no
        // rompe el esquema ni obliga a migrar a los clientes ya instalados.
        builder.Property(t => t.Settings)
            .HasConversion(v => JsonHelper.SerializeSettings(v), v => JsonHelper.DeserializeSettings(v))
            .HasColumnName("settings")
            .HasColumnType(MappingConventions.JsonColumnType)
            .Metadata.SetValueComparer(new ValueComparer<TenantSettings>(
                (a, b) => JsonHelper.SerializeSettings(a!) == JsonHelper.SerializeSettings(b!),
                v => JsonHelper.SerializeSettings(v).GetHashCode(),
                v => JsonHelper.DeserializeSettings(JsonHelper.SerializeSettings(v))));
    }
}

public sealed class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.ToTable("users");
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Email).HasMaxLength(150).IsRequired();
        builder.Property(u => u.FullName).HasMaxLength(120).IsRequired();
        builder.Property(u => u.PasswordHash).HasMaxLength(300).IsRequired();
        builder.HasIndex(u => new { u.TenantId, u.Email }).IsUnique();
    }
}

public sealed class VehicleTypeConfiguration : IEntityTypeConfiguration<VehicleType>
{
    public void Configure(EntityTypeBuilder<VehicleType> builder)
    {
        builder.ToTable("vehicle_types");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Code).HasMaxLength(20).IsRequired();
        builder.Property(t => t.Name).HasMaxLength(80).IsRequired();
        builder.HasIndex(t => new { t.TenantId, t.Code }).IsUnique();
    }
}

public sealed class ExpenseCategoryConfiguration : IEntityTypeConfiguration<ExpenseCategory>
{
    public void Configure(EntityTypeBuilder<ExpenseCategory> builder)
    {
        builder.ToTable("expense_categories");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Code).HasMaxLength(20).IsRequired();
        builder.Property(c => c.Name).HasMaxLength(80).IsRequired();
        builder.HasIndex(c => new { c.TenantId, c.Code }).IsUnique();
    }
}

public sealed class CustomFieldDefinitionConfiguration : IEntityTypeConfiguration<CustomFieldDefinition>
{
    public void Configure(EntityTypeBuilder<CustomFieldDefinition> builder)
    {
        builder.ToTable("custom_field_definitions");
        builder.HasKey(f => f.Id);
        builder.Property(f => f.Key).HasMaxLength(40).IsRequired();
        builder.Property(f => f.Label).HasMaxLength(80).IsRequired();
        builder.Property(f => f.Options).HasMaxLength(1000);
        builder.HasIndex(f => new { f.TenantId, f.Target, f.Key }).IsUnique();
    }
}
