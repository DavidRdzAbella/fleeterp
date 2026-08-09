using FleetErp.Application.Contracts;
using FleetErp.Domain.Entities;

namespace FleetErp.Infrastructure.Persistence.Queries;

/// <summary>
/// Traducción entidad → contrato. Se hace en memoria sobre entidades ya
/// materializadas (y paginadas) en lugar de proyectar en SQL, porque los campos
/// configurables viven en JSON y los totales del viaje son cálculos del dominio:
/// proyectarlos en la consulta obligaría a duplicar esas reglas en LINQ.
/// </summary>
internal static class Mapping
{
    public static string Label(this Vehicle? v) =>
        v is null ? "—" : $"{v.EconomicNumber} · {v.PlateNumber}";

    public static VehicleDto ToDto(this Vehicle v) => new(
        v.Id, v.EconomicNumber, v.PlateNumber,
        v.VehicleTypeId, v.VehicleType?.Name ?? "—", v.VehicleType?.Category ?? default,
        v.Brand, v.Model, v.Year, v.Vin,
        v.CargoCapacity, v.TankCapacity, v.CurrentOdometer, v.Status,
        v.InsuranceExpiry, v.CirculationCardExpiry,
        v.CustomFields.Values, v.IsActive);

    public static DriverDto ToDto(this Driver d, DateOnly today, int alertDays) => new(
        d.Id, d.FirstName, d.LastName, d.FullName,
        d.EmployeeNumber, d.LicenseNumber, d.LicenseType, d.LicenseExpiry,
        d.LicenseExpiresWithin(today, alertDays),
        d.Phone, d.Email, d.HireDate,
        d.PayScheme, d.PayRate, d.Status,
        d.CustomFields.Values, d.IsActive);

    public static CustomerDto ToDto(this Customer c) => new(
        c.Id, c.Name, c.TaxId, c.ContactName, c.Phone, c.Email, c.Address,
        c.CustomFields.Values, c.IsActive);

    public static TripListItemDto ToListItem(this Trip t) => new(
        t.Id, t.Folio, t.Status,
        t.Driver?.FullName ?? "—", t.Vehicle.Label(), t.Customer?.Name,
        t.Origin, t.Destination,
        t.ScheduledDepartureUtc, t.ActualDepartureUtc, t.ActualArrivalUtc,
        t.PlannedDistance, t.ActualDistance,
        t.CargoWeight, t.CargoWeightUnit,
        t.FreightRevenue, t.TotalCost, t.Profit,
        t.IsLate);

    public static TripDetailDto ToDetail(this Trip t) => new(
        t.Id, t.Folio, t.Status,
        t.DriverId, t.Driver?.FullName ?? "—",
        t.VehicleId, t.Vehicle.Label(),
        t.TrailerId, t.Trailer?.Label(),
        t.CustomerId, t.Customer?.Name,
        t.Origin, t.Destination, t.PlannedDistance,
        t.ScheduledDepartureUtc, t.ScheduledArrivalUtc, t.ActualDepartureUtc, t.ActualArrivalUtc,
        t.OdometerStart, t.OdometerEnd, t.ActualDistance,
        t.InitialFuel, t.FinalFuel, t.RefuelPlanned,
        t.FuelPurchased, t.FuelConsumed, t.FuelEfficiency,
        t.CargoWeight, t.CargoWeightUnit, t.CargoDescription,
        t.FreightRevenue, t.DriverPayScheme, t.DriverPayRate, t.DriverHours, t.DriverPayAmount,
        t.FuelCost, t.OtherExpensesCost, t.TotalCost, t.Profit, t.ProfitMargin,
        t.IsLate, t.Notes, t.CancellationReason,
        t.CustomFields.Values,
        t.FuelLogs.OrderByDescending(f => f.LoadedAtUtc).Select(f => f.ToDto(t.Folio)).ToList(),
        t.Expenses.OrderByDescending(e => e.IncurredAtUtc).Select(e => e.ToDto(t.Folio)).ToList());

    public static FuelLogDto ToDto(this FuelLog f, string? tripFolio = null) => new(
        f.Id, f.VehicleId, f.Vehicle.Label(), f.TripId, tripFolio ?? f.Trip?.Folio,
        f.DriverId, f.Driver?.FullName,
        f.LoadedAtUtc, f.Quantity, f.PricePerUnit, f.TotalCost,
        f.OdometerReading, f.Station, f.ReferenceNumber);

    public static ExpenseDto ToDto(this Expense e, string? tripFolio = null) => new(
        e.Id, e.CategoryId, e.Category?.Name ?? "—",
        e.TripId, tripFolio ?? e.Trip?.Folio,
        e.VehicleId, e.Vehicle?.Label(),
        e.DriverId, e.Driver?.FullName,
        e.IncurredAtUtc, e.Amount, e.Description, e.ReferenceNumber);

    public static MaintenanceOrderDto ToDto(this MaintenanceOrder m) => new(
        m.Id, m.Folio, m.VehicleId, m.Vehicle.Label(),
        m.Kind, m.Status, m.OpenedAtUtc, m.ClosedAtUtc,
        m.Description, m.Workshop, m.Cost, m.OdometerAtService);
}
