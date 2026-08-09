namespace FleetErp.Domain.Common;

/// <summary>
/// Base de toda entidad operativa: pertenece a una empresa y se audita.
/// Concentrar esto aquí evita repetir seis propiedades en cada entidad y permite
/// que el <c>DbContext</c> aplique filtro de tenant y auditoría de forma uniforme.
/// </summary>
public abstract class TenantEntity : BaseEntity, ITenantScoped, IAuditable
{
    public Guid TenantId { get; set; }

    public DateTimeOffset CreatedAtUtc { get; set; }
    public string? CreatedBy { get; set; }
    public DateTimeOffset? UpdatedAtUtc { get; set; }
    public string? UpdatedBy { get; set; }
}
