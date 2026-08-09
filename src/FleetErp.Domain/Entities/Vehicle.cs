using FleetErp.Domain.Common;
using FleetErp.Domain.Enums;

namespace FleetErp.Domain.Entities;

/// <summary>
/// Unidad del inventario de flotilla. Una sola entidad cubre tractocamiones y
/// cajas/remolques: lo que las diferencia es el <see cref="VehicleType"/> y su
/// categoría, no una jerarquía de clases. Así el inventario que pidió el cliente
/// ("los tractocamiones, las cajas y todo ese show") es una sola pantalla.
/// </summary>
public class Vehicle : TenantEntity, ISoftDeletable
{
    private Vehicle() { }

    public Vehicle(string economicNumber, string plateNumber, Guid vehicleTypeId)
    {
        SetIdentification(economicNumber, plateNumber);
        VehicleTypeId = vehicleTypeId;
        CustomFields = new CustomFieldValues();
    }

    /// <summary>Número económico interno; es como la empresa realmente llama a la unidad.</summary>
    public string EconomicNumber { get; private set; } = string.Empty;
    public string PlateNumber { get; private set; } = string.Empty;

    public Guid VehicleTypeId { get; private set; }
    public VehicleType? VehicleType { get; private set; }

    public string? Brand { get; private set; }
    public string? Model { get; private set; }
    public int? Year { get; private set; }
    public string? Vin { get; private set; }

    /// <summary>Capacidad de carga en la unidad de peso configurada por la empresa.</summary>
    public decimal? CargoCapacity { get; private set; }
    public decimal? TankCapacity { get; private set; }

    public decimal CurrentOdometer { get; private set; }
    public VehicleStatus Status { get; private set; } = VehicleStatus.Available;

    public DateOnly? InsuranceExpiry { get; private set; }
    public DateOnly? CirculationCardExpiry { get; private set; }

    public CustomFieldValues CustomFields { get; private set; } = new();
    public bool IsActive { get; private set; } = true;

    public void SetIdentification(string economicNumber, string plateNumber)
    {
        DomainException.Require(!string.IsNullOrWhiteSpace(economicNumber), "El número económico es obligatorio.");
        DomainException.Require(!string.IsNullOrWhiteSpace(plateNumber), "La placa es obligatoria.");
        EconomicNumber = economicNumber.Trim().ToUpperInvariant();
        PlateNumber = plateNumber.Trim().ToUpperInvariant();
    }

    public void SetSpecs(string? brand, string? model, int? year, string? vin, decimal? cargoCapacity, decimal? tankCapacity)
    {
        DomainException.Require(year is null or (>= 1950 and <= 2100), "El año del vehículo no es válido.");
        DomainException.Require(cargoCapacity is null or >= 0, "La capacidad de carga no puede ser negativa.");
        DomainException.Require(tankCapacity is null or >= 0, "La capacidad del tanque no puede ser negativa.");
        Brand = brand?.Trim();
        Model = model?.Trim();
        Year = year;
        Vin = vin?.Trim().ToUpperInvariant();
        CargoCapacity = cargoCapacity;
        TankCapacity = tankCapacity;
    }

    public void SetCompliance(DateOnly? insuranceExpiry, DateOnly? circulationCardExpiry)
    {
        InsuranceExpiry = insuranceExpiry;
        CirculationCardExpiry = circulationCardExpiry;
    }

    public void ChangeType(Guid vehicleTypeId)
    {
        DomainException.Require(vehicleTypeId != Guid.Empty, "El tipo de unidad es obligatorio.");
        VehicleTypeId = vehicleTypeId;
    }

    /// <summary>El odómetro solo avanza; un retroceso indica captura errónea.</summary>
    public void UpdateOdometer(decimal reading)
    {
        DomainException.Require(reading >= 0, "La lectura del odómetro no puede ser negativa.");
        DomainException.Require(reading >= CurrentOdometer,
            $"El odómetro no puede retroceder (actual {CurrentOdometer:N0}, capturado {reading:N0}).");
        CurrentOdometer = reading;
    }

    public void SetInitialOdometer(decimal reading)
    {
        DomainException.Require(reading >= 0, "La lectura del odómetro no puede ser negativa.");
        CurrentOdometer = reading;
    }

    public void MarkOnTrip()
    {
        DomainException.Require(Status == VehicleStatus.Available,
            $"La unidad {EconomicNumber} no está disponible (estado actual: {Status}).");
        Status = VehicleStatus.OnTrip;
    }

    public void ReleaseFromTrip()
    {
        if (Status == VehicleStatus.OnTrip) Status = VehicleStatus.Available;
    }

    public void SendToMaintenance() => Status = VehicleStatus.InMaintenance;

    public void ReturnFromMaintenance()
    {
        if (Status == VehicleStatus.InMaintenance) Status = VehicleStatus.Available;
    }

    public void SetOutOfService() => Status = VehicleStatus.OutOfService;

    /// <summary>
    /// Regresa la unidad a disponible desde taller o desde fuera de servicio.
    /// </summary>
    /// <remarks>
    /// El único estado que no se puede abandonar por aquí es "en viaje": esa
    /// ocupación la impone el despacho y solo se suelta al cerrar o cancelar el
    /// viaje, para que unidad y viaje no se contradigan.
    /// </remarks>
    public void ReturnToService()
    {
        DomainException.Require(Status != VehicleStatus.OnTrip,
            $"La unidad {EconomicNumber} está en viaje; concluya o cancele el viaje para liberarla.");
        Status = VehicleStatus.Available;
    }

    public void Deactivate()
    {
        DomainException.Require(Status != VehicleStatus.OnTrip, "No se puede dar de baja una unidad que está en viaje.");
        IsActive = false;
    }

    public void Activate() => IsActive = true;
}
