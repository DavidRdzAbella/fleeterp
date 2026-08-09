using FleetErp.Domain.Common;
using FleetErp.Domain.Enums;

namespace FleetErp.Domain.Entities;

/// <summary>
/// Catálogo de tipos de unidad, editable por cada empresa: "Tractocamión",
/// "Caja seca 53'", "Pipa", "Rabón". Al ser catálogo y no enum, una empresa nueva
/// da de alta su propia nomenclatura sin tocar el código.
/// </summary>
public class VehicleType : TenantEntity, ISoftDeletable
{
    private VehicleType() { }

    public VehicleType(string code, string name, VehicleCategory category)
    {
        Update(code, name, category);
    }

    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;

    /// <summary>Motriz o de arrastre: define si puede engancharse a un viaje como remolque.</summary>
    public VehicleCategory Category { get; private set; }

    public bool IsActive { get; private set; } = true;

    public void Update(string code, string name, VehicleCategory category)
    {
        DomainException.Require(!string.IsNullOrWhiteSpace(code), "El código del tipo de unidad es obligatorio.");
        DomainException.Require(!string.IsNullOrWhiteSpace(name), "El nombre del tipo de unidad es obligatorio.");
        Code = code.Trim().ToUpperInvariant();
        Name = name.Trim();
        Category = category;
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}
