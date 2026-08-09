using FleetErp.Domain.Common;

namespace FleetErp.Domain.Entities;

/// <summary>
/// Empresa transportista. Es la unidad de aislamiento del sistema: todo dato
/// operativo cuelga de un tenant, y una misma instalación atiende a varias.
/// </summary>
public class Tenant : BaseEntity, IAuditable, ISoftDeletable
{
    private Tenant() { }

    public Tenant(string name, string slug, TenantSettings? settings = null)
    {
        Rename(name);
        SetSlug(slug);
        Settings = settings ?? TenantSettings.Default();
    }

    public string Name { get; private set; } = string.Empty;

    /// <summary>Identificador corto y estable (subdominio / cabecera X-Tenant).</summary>
    public string Slug { get; private set; } = string.Empty;

    public string? TaxId { get; private set; }
    public string? ContactEmail { get; private set; }
    public string? Phone { get; private set; }

    /// <summary>Parametrización que evita tocar código al implantar en otra empresa.</summary>
    public TenantSettings Settings { get; private set; } = TenantSettings.Default();

    public bool IsActive { get; private set; } = true;

    public DateTimeOffset CreatedAtUtc { get; set; }
    public string? CreatedBy { get; set; }
    public DateTimeOffset? UpdatedAtUtc { get; set; }
    public string? UpdatedBy { get; set; }

    public void Rename(string name)
    {
        DomainException.Require(!string.IsNullOrWhiteSpace(name), "El nombre de la empresa es obligatorio.");
        Name = name.Trim();
    }

    public void SetSlug(string slug)
    {
        DomainException.Require(!string.IsNullOrWhiteSpace(slug), "El identificador (slug) de la empresa es obligatorio.");
        Slug = slug.Trim().ToLowerInvariant();
    }

    public void SetContact(string? taxId, string? email, string? phone)
    {
        TaxId = taxId?.Trim();
        ContactEmail = email?.Trim();
        Phone = phone?.Trim();
    }

    public void UpdateSettings(TenantSettings settings) =>
        Settings = settings ?? throw new DomainException("La configuración de la empresa es obligatoria.");

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}
