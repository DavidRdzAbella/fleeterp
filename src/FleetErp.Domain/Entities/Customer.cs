using FleetErp.Domain.Common;

namespace FleetErp.Domain.Entities;

/// <summary>
/// Cliente al que se le factura el flete. Es lo que permite responder
/// "cuánto vendió" por viaje, por conductor y por cuenta.
/// </summary>
public class Customer : TenantEntity, ISoftDeletable
{
    private Customer() { }

    public Customer(string name)
    {
        Rename(name);
        CustomFields = new CustomFieldValues();
    }

    public string Name { get; private set; } = string.Empty;
    public string? TaxId { get; private set; }
    public string? ContactName { get; private set; }
    public string? Phone { get; private set; }
    public string? Email { get; private set; }
    public string? Address { get; private set; }

    public CustomFieldValues CustomFields { get; private set; } = new();
    public bool IsActive { get; private set; } = true;

    public void Rename(string name)
    {
        DomainException.Require(!string.IsNullOrWhiteSpace(name), "El nombre del cliente es obligatorio.");
        Name = name.Trim();
    }

    public void SetContact(string? taxId, string? contactName, string? phone, string? email, string? address)
    {
        TaxId = taxId?.Trim();
        ContactName = contactName?.Trim();
        Phone = phone?.Trim();
        Email = email?.Trim();
        Address = address?.Trim();
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}
