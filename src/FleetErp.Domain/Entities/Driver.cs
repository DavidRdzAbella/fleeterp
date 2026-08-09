using FleetErp.Domain.Common;
using FleetErp.Domain.Enums;

namespace FleetErp.Domain.Entities;

/// <summary>
/// Operador. Guarda su esquema de pago propio porque en la práctica conviven
/// choferes por hora, por kilómetro y por porcentaje dentro de la misma empresa.
/// </summary>
public class Driver : TenantEntity, ISoftDeletable
{
    private Driver() { }

    public Driver(string firstName, string lastName, string licenseNumber)
    {
        SetName(firstName, lastName);
        LicenseNumber = licenseNumber.Trim().ToUpperInvariant();
        CustomFields = new CustomFieldValues();
    }

    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}".Trim();

    public string? EmployeeNumber { get; private set; }
    public string LicenseNumber { get; private set; } = string.Empty;
    public string? LicenseType { get; private set; }
    public DateOnly? LicenseExpiry { get; private set; }

    public string? Phone { get; private set; }
    public string? Email { get; private set; }
    public DateOnly? HireDate { get; private set; }

    public DriverPayScheme PayScheme { get; private set; } = DriverPayScheme.PerHour;

    /// <summary>Tarifa según el esquema: $/hora, $/km, $ por viaje o % del flete.</summary>
    public decimal PayRate { get; private set; }

    public DriverStatus Status { get; private set; } = DriverStatus.Active;
    public CustomFieldValues CustomFields { get; private set; } = new();
    public bool IsActive { get; private set; } = true;

    public void SetName(string firstName, string lastName)
    {
        DomainException.Require(!string.IsNullOrWhiteSpace(firstName), "El nombre del conductor es obligatorio.");
        DomainException.Require(!string.IsNullOrWhiteSpace(lastName), "El apellido del conductor es obligatorio.");
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
    }

    public void SetLicense(string licenseNumber, string? licenseType, DateOnly? expiry)
    {
        DomainException.Require(!string.IsNullOrWhiteSpace(licenseNumber), "El número de licencia es obligatorio.");
        LicenseNumber = licenseNumber.Trim().ToUpperInvariant();
        LicenseType = licenseType?.Trim();
        LicenseExpiry = expiry;
    }

    public void SetContact(string? employeeNumber, string? phone, string? email, DateOnly? hireDate)
    {
        EmployeeNumber = employeeNumber?.Trim();
        Phone = phone?.Trim();
        Email = email?.Trim();
        HireDate = hireDate;
    }

    public void SetCompensation(DriverPayScheme scheme, decimal rate)
    {
        DomainException.Require(rate >= 0, "La tarifa del conductor no puede ser negativa.");
        DomainException.Require(scheme != DriverPayScheme.PercentageOfRevenue || rate <= 100,
            "El porcentaje sobre el flete no puede ser mayor a 100.");
        PayScheme = scheme;
        PayRate = rate;
    }

    /// <summary>Licencia vencida o por vencer respecto al umbral configurado por la empresa.</summary>
    public bool LicenseExpiresWithin(DateOnly today, int days) =>
        LicenseExpiry is not null && LicenseExpiry.Value <= today.AddDays(days);

    public void MarkOnTrip()
    {
        DomainException.Require(Status == DriverStatus.Active,
            $"El conductor {FullName} no está disponible (estado actual: {Status}).");
        Status = DriverStatus.OnTrip;
    }

    public void ReleaseFromTrip()
    {
        if (Status == DriverStatus.OnTrip) Status = DriverStatus.Active;
    }

    public void SetOnLeave() => Status = DriverStatus.OnLeave;

    public void ReturnFromLeave()
    {
        if (Status == DriverStatus.OnLeave) Status = DriverStatus.Active;
    }

    public void Deactivate()
    {
        DomainException.Require(Status != DriverStatus.OnTrip, "No se puede dar de baja un conductor que está en viaje.");
        IsActive = false;
        Status = DriverStatus.Inactive;
    }

    public void Activate()
    {
        IsActive = true;
        Status = DriverStatus.Active;
    }
}
