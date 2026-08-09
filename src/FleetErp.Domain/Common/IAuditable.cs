namespace FleetErp.Domain.Common;

/// <summary>
/// Marca entidades a las que la infraestructura estampa auditoría automáticamente.
/// </summary>
public interface IAuditable
{
    DateTimeOffset CreatedAtUtc { get; set; }
    string? CreatedBy { get; set; }
    DateTimeOffset? UpdatedAtUtc { get; set; }
    string? UpdatedBy { get; set; }
}
