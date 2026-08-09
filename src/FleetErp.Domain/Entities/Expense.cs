using FleetErp.Domain.Common;

namespace FleetErp.Domain.Entities;

/// <summary>
/// Gasto distinto al combustible: casetas, viáticos, refacciones, multas,
/// maniobras. Al colgar de un catálogo configurable, cada empresa desglosa sus
/// costos como los tenga acostumbrados sin cambiar el modelo.
/// </summary>
public class Expense : TenantEntity
{
    private Expense() { }

    public Expense(Guid categoryId, DateTimeOffset incurredAtUtc, decimal amount, string description)
    {
        DomainException.Require(categoryId != Guid.Empty, "El concepto de gasto es obligatorio.");
        CategoryId = categoryId;
        IncurredAtUtc = incurredAtUtc;
        SetDetails(amount, description);
    }

    public Guid CategoryId { get; private set; }
    public ExpenseCategory? Category { get; private set; }

    public Guid? TripId { get; private set; }
    public Trip? Trip { get; private set; }

    public Guid? VehicleId { get; private set; }
    public Vehicle? Vehicle { get; private set; }

    public Guid? DriverId { get; private set; }
    public Driver? Driver { get; private set; }

    public DateTimeOffset IncurredAtUtc { get; private set; }
    public decimal Amount { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public string? ReferenceNumber { get; private set; }

    /// <summary>Corrige el concepto o la fecha de un gasto ya capturado.</summary>
    public void Recategorize(Guid categoryId, DateTimeOffset incurredAtUtc)
    {
        DomainException.Require(categoryId != Guid.Empty, "El concepto de gasto es obligatorio.");
        CategoryId = categoryId;
        IncurredAtUtc = incurredAtUtc;
    }

    public void SetDetails(decimal amount, string description)
    {
        DomainException.Require(amount > 0, "El importe del gasto debe ser mayor a cero.");
        DomainException.Require(!string.IsNullOrWhiteSpace(description), "La descripción del gasto es obligatoria.");
        Amount = Math.Round(amount, 2, MidpointRounding.AwayFromZero);
        Description = description.Trim();
    }

    public void SetContext(Guid? tripId, Guid? vehicleId, Guid? driverId, string? referenceNumber)
    {
        TripId = tripId;
        VehicleId = vehicleId;
        DriverId = driverId;
        ReferenceNumber = referenceNumber?.Trim();
    }
}
