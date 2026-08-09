using FleetErp.Domain.Common;

namespace FleetErp.Domain.Entities;

/// <summary>
/// Carga de combustible. Fuente única de litros y costo de diésel: de aquí salen
/// tanto el "gasto total en combustible" del tablero como el rendimiento por unidad.
/// Puede ir ligada a un viaje o ser una carga de patio sin viaje asociado.
/// </summary>
public class FuelLog : TenantEntity
{
    private FuelLog() { }

    public FuelLog(Guid vehicleId, DateTimeOffset loadedAtUtc, decimal quantity, decimal pricePerUnit)
    {
        DomainException.Require(vehicleId != Guid.Empty, "La unidad es obligatoria.");
        VehicleId = vehicleId;
        LoadedAtUtc = loadedAtUtc;
        SetAmounts(quantity, pricePerUnit);
    }

    public Guid VehicleId { get; private set; }
    public Vehicle? Vehicle { get; private set; }

    public Guid? TripId { get; private set; }
    public Trip? Trip { get; private set; }

    public Guid? DriverId { get; private set; }
    public Driver? Driver { get; private set; }

    public DateTimeOffset LoadedAtUtc { get; private set; }

    /// <summary>Cantidad cargada en la unidad de volumen configurada por la empresa.</summary>
    public decimal Quantity { get; private set; }
    public decimal PricePerUnit { get; private set; }
    public decimal TotalCost { get; private set; }

    public decimal? OdometerReading { get; private set; }
    public string? Station { get; private set; }
    public string? ReferenceNumber { get; private set; }

    /// <summary>Corrige la unidad o la fecha de una carga ya capturada.</summary>
    public void Reassign(Guid vehicleId, DateTimeOffset loadedAtUtc)
    {
        DomainException.Require(vehicleId != Guid.Empty, "La unidad es obligatoria.");
        VehicleId = vehicleId;
        LoadedAtUtc = loadedAtUtc;
    }

    public void SetAmounts(decimal quantity, decimal pricePerUnit)
    {
        DomainException.Require(quantity > 0, "La cantidad de combustible debe ser mayor a cero.");
        DomainException.Require(pricePerUnit >= 0, "El precio por unidad no puede ser negativo.");
        Quantity = quantity;
        PricePerUnit = pricePerUnit;
        TotalCost = Math.Round(quantity * pricePerUnit, 2, MidpointRounding.AwayFromZero);
    }

    public void SetContext(Guid? tripId, Guid? driverId, decimal? odometerReading, string? station, string? referenceNumber)
    {
        DomainException.Require(odometerReading is null or >= 0, "La lectura del odómetro no puede ser negativa.");
        TripId = tripId;
        DriverId = driverId;
        OdometerReading = odometerReading;
        Station = station?.Trim();
        ReferenceNumber = referenceNumber?.Trim();
    }
}
