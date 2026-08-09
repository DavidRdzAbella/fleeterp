using FleetErp.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace FleetErp.Infrastructure.Persistence.Queries;

/// <summary>
/// Carga los hechos del periodo y los aplana para el motor de tableros. Es la
/// única clase que sabe de dónde salen los datos; la aritmética de los KPIs vive
/// en la capa de aplicación y no depende de esta implementación.
/// </summary>
public sealed class EfAnalyticsDataSource(FleetDbContext context) : IAnalyticsDataSource
{
    public async Task<AnalyticsDataSet> LoadAsync(DateTimeOffset fromUtc, DateTimeOffset toUtc, CancellationToken ct = default)
    {
        // Un viaje entra al periodo por su salida real; si aún no sale, por la
        // programada. Así los viajes planeados de hoy también se ven en el tablero.
        var trips = await context.Trips.AsNoTracking()
            .Include(t => t.Driver)
            .Include(t => t.Vehicle)
            .Include(t => t.Customer)
            .Where(t => (t.ActualDepartureUtc ?? t.ScheduledDepartureUtc) >= fromUtc &&
                        (t.ActualDepartureUtc ?? t.ScheduledDepartureUtc) <= toUtc)
            .Select(t => new TripFact(
                t.Id, t.Folio, t.Status,
                t.DriverId, t.Driver!.FirstName + " " + t.Driver.LastName,
                t.VehicleId, t.Vehicle!.EconomicNumber + " · " + t.Vehicle.PlateNumber,
                t.CustomerId, t.Customer != null ? t.Customer.Name : null,
                t.ScheduledDepartureUtc, t.ScheduledArrivalUtc, t.ActualDepartureUtc, t.ActualArrivalUtc,
                t.PlannedDistance,
                t.OdometerStart != null && t.OdometerEnd != null ? t.OdometerEnd.Value - t.OdometerStart.Value : 0m,
                t.InitialFuel, t.FinalFuel,
                t.FreightRevenue, t.DriverPayAmount, t.DriverHours, t.DriverPayScheme,
                t.ScheduledArrivalUtc != null && t.ActualArrivalUtc != null &&
                    t.ActualArrivalUtc.Value > t.ScheduledArrivalUtc.Value))
            .ToListAsync(ct);

        var expenses = await context.Expenses.AsNoTracking()
            .Include(e => e.Category)
            .Where(e => e.IncurredAtUtc >= fromUtc && e.IncurredAtUtc <= toUtc)
            .Select(e => new ExpenseFact(
                e.Id, e.CategoryId, e.Category!.Name, e.Category.IsTripRelated,
                e.TripId, e.VehicleId, e.DriverId, e.IncurredAtUtc, e.Amount))
            .ToListAsync(ct);

        var fuel = await context.FuelLogs.AsNoTracking()
            .Include(f => f.Vehicle)
            .Where(f => f.LoadedAtUtc >= fromUtc && f.LoadedAtUtc <= toUtc)
            .Select(f => new FuelFact(
                f.Id, f.VehicleId, f.Vehicle!.EconomicNumber + " · " + f.Vehicle.PlateNumber,
                f.TripId, f.DriverId, f.LoadedAtUtc, f.Quantity, f.TotalCost))
            .ToListAsync(ct);

        var maintenance = await context.MaintenanceOrders.AsNoTracking()
            .Where(m => m.OpenedAtUtc <= toUtc && (m.ClosedAtUtc == null || m.ClosedAtUtc >= fromUtc))
            .Select(m => new MaintenanceFact(m.Id, m.VehicleId, m.Status, m.OpenedAtUtc, m.ClosedAtUtc, m.Cost))
            .ToListAsync(ct);

        // El inventario y la plantilla se leen completos: el tablero informa el
        // estado actual de la flotilla, no el que tenía dentro del rango.
        var vehicles = await context.Vehicles.AsNoTracking()
            .Include(v => v.VehicleType)
            .Select(v => new VehicleFact(
                v.Id, v.EconomicNumber + " · " + v.PlateNumber, v.Status,
                v.VehicleType!.Category, v.IsActive, v.InsuranceExpiry))
            .ToListAsync(ct);

        var drivers = await context.Drivers.AsNoTracking()
            .Select(d => new DriverFact(d.Id, d.FirstName + " " + d.LastName, d.Status, d.IsActive, d.LicenseExpiry))
            .ToListAsync(ct);

        return new AnalyticsDataSet(trips, expenses, fuel, maintenance, vehicles, drivers);
    }
}
