namespace FleetErp.Domain.Common;

/// <summary>
/// Baja lógica: los catálogos nunca se borran físicamente porque hay
/// movimientos históricos que los referencian.
/// </summary>
public interface ISoftDeletable
{
    bool IsActive { get; }
    void Deactivate();
    void Activate();
}
