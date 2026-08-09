using FleetErp.Domain.Common;
using FleetErp.Domain.Enums;

namespace FleetErp.Domain.Entities;

/// <summary>
/// Orden de servicio de una unidad. Existe en el MVP porque es lo que justifica
/// que un camión aparezca fuera de circulación en el tablero de flotilla.
/// </summary>
public class MaintenanceOrder : TenantEntity
{
    private MaintenanceOrder() { }

    public MaintenanceOrder(string folio, Guid vehicleId, MaintenanceKind kind, DateTimeOffset openedAtUtc, string description)
    {
        DomainException.Require(!string.IsNullOrWhiteSpace(folio), "El folio de la orden es obligatorio.");
        DomainException.Require(vehicleId != Guid.Empty, "La unidad es obligatoria.");
        DomainException.Require(!string.IsNullOrWhiteSpace(description), "La descripción del servicio es obligatoria.");

        Folio = folio.Trim().ToUpperInvariant();
        VehicleId = vehicleId;
        Kind = kind;
        OpenedAtUtc = openedAtUtc;
        Description = description.Trim();
    }

    public string Folio { get; private set; } = string.Empty;

    public Guid VehicleId { get; private set; }
    public Vehicle? Vehicle { get; private set; }

    public MaintenanceKind Kind { get; private set; }
    public MaintenanceStatus Status { get; private set; } = MaintenanceStatus.Open;

    public DateTimeOffset OpenedAtUtc { get; private set; }
    public DateTimeOffset? ClosedAtUtc { get; private set; }

    public string Description { get; private set; } = string.Empty;
    public string? Workshop { get; private set; }
    public decimal Cost { get; private set; }
    public decimal? OdometerAtService { get; private set; }

    public void Start()
    {
        DomainException.Require(Status == MaintenanceStatus.Open, "La orden ya fue iniciada o cerrada.");
        Status = MaintenanceStatus.InProgress;
    }

    public void Close(DateTimeOffset closedAtUtc, decimal cost, string? workshop, decimal? odometerAtService)
    {
        DomainException.Require(Status != MaintenanceStatus.Closed, "La orden ya está cerrada.");
        DomainException.Require(closedAtUtc >= OpenedAtUtc, "El cierre no puede ser anterior a la apertura.");
        DomainException.Require(cost >= 0, "El costo del servicio no puede ser negativo.");
        ClosedAtUtc = closedAtUtc;
        Cost = Math.Round(cost, 2, MidpointRounding.AwayFromZero);
        Workshop = workshop?.Trim();
        OdometerAtService = odometerAtService;
        Status = MaintenanceStatus.Closed;
    }
}
