using FleetErp.Application.Abstractions;
using FleetErp.Application.Common;
using FleetErp.Application.Contracts;
using FleetErp.Domain.Abstractions;
using FleetErp.Domain.Entities;
using FleetErp.Domain.Enums;

namespace FleetErp.Application.Services;

public sealed class TripService(
    IUnitOfWork uow,
    ICurrentTenant tenant,
    IFolioGenerator folios) : ITripService
{
    public async Task<Guid> CreateAsync(CreateTripRequest request, CancellationToken ct = default)
    {
        var driver = await RequireDriverAsync(request.DriverId, ct);
        var vehicle = await RequireVehicleAsync(request.VehicleId, ct);
        await ValidateTrailerAsync(request.TrailerId, request.VehicleId, ct);
        await ValidateCustomerAsync(request.CustomerId, ct);

        var folio = await folios.NextTripFolioAsync(ct);

        var trip = new Trip(folio, driver.Id, vehicle.Id, request.Origin, request.Destination,
                            request.ScheduledDepartureUtc)
        {
            TenantId = tenant.TenantId
        };

        trip.SetAssignment(request.DriverId, request.VehicleId, request.TrailerId, request.CustomerId);
        trip.SetRoute(request.Origin, request.Destination, request.PlannedDistance);
        trip.SetSchedule(request.ScheduledDepartureUtc, request.ScheduledArrivalUtc);
        trip.SetFuelPlan(request.InitialFuel, request.RefuelPlanned);
        trip.SetCargo(request.CargoWeight, request.CargoWeightUnit, request.CargoDescription);

        // Si el alta no trae esquema de pago, se hereda el del conductor: es el
        // caso normal y evita recapturar la tarifa en cada viaje.
        trip.SetCommercialTerms(
            request.FreightRevenue,
            request.DriverPayScheme ?? driver.PayScheme,
            request.DriverPayRate ?? driver.PayRate);

        trip.SetNotes(request.Notes);
        trip.CustomFields.Replace(request.CustomFields);

        await uow.Trips.AddAsync(trip, ct);
        await uow.SaveChangesAsync(ct);
        return trip.Id;
    }

    public async Task UpdateAsync(Guid id, UpdateTripRequest request, CancellationToken ct = default)
    {
        var trip = await RequireTripAsync(id, ct);

        await RequireDriverAsync(request.DriverId, ct);
        await RequireVehicleAsync(request.VehicleId, ct);
        await ValidateTrailerAsync(request.TrailerId, request.VehicleId, ct);
        await ValidateCustomerAsync(request.CustomerId, ct);

        trip.SetAssignment(request.DriverId, request.VehicleId, request.TrailerId, request.CustomerId);
        trip.SetRoute(request.Origin, request.Destination, request.PlannedDistance);
        trip.SetSchedule(request.ScheduledDepartureUtc, request.ScheduledArrivalUtc);
        trip.SetFuelPlan(request.InitialFuel, request.RefuelPlanned);
        trip.SetCargo(request.CargoWeight, request.CargoWeightUnit, request.CargoDescription);
        trip.SetCommercialTerms(request.FreightRevenue, request.DriverPayScheme, request.DriverPayRate);
        trip.SetNotes(request.Notes);
        trip.CustomFields.Replace(request.CustomFields);

        uow.Trips.Update(trip);
        await uow.SaveChangesAsync(ct);
    }

    public async Task DispatchAsync(Guid id, DispatchTripRequest request, CancellationToken ct = default)
    {
        var trip = await RequireTripAsync(id, ct);
        var vehicle = await RequireVehicleAsync(trip.VehicleId, ct);
        var driver = await RequireDriverAsync(trip.DriverId, ct);

        trip.Dispatch(request.DepartureUtc, request.OdometerStart, request.InitialFuel);

        vehicle.UpdateOdometer(request.OdometerStart);
        vehicle.MarkOnTrip();
        driver.MarkOnTrip();

        if (trip.TrailerId is not null)
        {
            var trailer = await uow.Vehicles.GetByIdAsync(trip.TrailerId.Value, ct);
            trailer?.MarkOnTrip();
            if (trailer is not null) uow.Vehicles.Update(trailer);
        }

        uow.Trips.Update(trip);
        uow.Vehicles.Update(vehicle);
        uow.Drivers.Update(driver);
        await uow.SaveChangesAsync(ct);
    }

    public async Task CompleteAsync(Guid id, CompleteTripRequest request, CancellationToken ct = default)
    {
        var trip = await RequireTripAsync(id, ct);
        var vehicle = await RequireVehicleAsync(trip.VehicleId, ct);
        var driver = await RequireDriverAsync(trip.DriverId, ct);

        trip.Complete(request.ArrivalUtc, request.OdometerEnd, request.FinalFuel, request.DriverHours);

        vehicle.UpdateOdometer(request.OdometerEnd);
        vehicle.ReleaseFromTrip();
        driver.ReleaseFromTrip();
        await ReleaseTrailerAsync(trip, ct);

        uow.Trips.Update(trip);
        uow.Vehicles.Update(vehicle);
        uow.Drivers.Update(driver);
        await uow.SaveChangesAsync(ct);
    }

    public async Task CancelAsync(Guid id, CancelTripRequest request, CancellationToken ct = default)
    {
        var trip = await RequireTripAsync(id, ct);
        var wasDispatched = trip.Status == TripStatus.InProgress;

        trip.Cancel(request.Reason);

        if (wasDispatched)
        {
            var vehicle = await uow.Vehicles.GetByIdAsync(trip.VehicleId, ct);
            var driver = await uow.Drivers.GetByIdAsync(trip.DriverId, ct);
            vehicle?.ReleaseFromTrip();
            driver?.ReleaseFromTrip();
            if (vehicle is not null) uow.Vehicles.Update(vehicle);
            if (driver is not null) uow.Drivers.Update(driver);
            await ReleaseTrailerAsync(trip, ct);
        }

        uow.Trips.Update(trip);
        await uow.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var trip = await RequireTripAsync(id, ct);

        // Un viaje ya despachado es evidencia contable: se cancela, no se borra.
        if (trip.Status != TripStatus.Planned)
            throw new ConflictException("Solo se puede eliminar un viaje en planeación; los demás se cancelan.");

        uow.Trips.Remove(trip);
        await uow.SaveChangesAsync(ct);
    }

    // ---- Guardas ----------------------------------------------------------

    private async Task<Trip> RequireTripAsync(Guid id, CancellationToken ct) =>
        await uow.Trips.GetByIdAsync(id, ct) ?? throw new NotFoundException("el viaje", id);

    private async Task<Driver> RequireDriverAsync(Guid id, CancellationToken ct) =>
        await uow.Drivers.GetByIdAsync(id, ct) ?? throw new NotFoundException("el conductor", id);

    private async Task<Vehicle> RequireVehicleAsync(Guid id, CancellationToken ct) =>
        await uow.Vehicles.GetByIdAsync(id, ct) ?? throw new NotFoundException("la unidad", id);

    private async Task ValidateTrailerAsync(Guid? trailerId, Guid vehicleId, CancellationToken ct)
    {
        if (trailerId is null) return;
        if (trailerId == vehicleId) throw new ConflictException("El remolque no puede ser la misma unidad motriz.");

        var trailer = await uow.Vehicles.GetByIdAsync(trailerId.Value, ct)
                      ?? throw new NotFoundException("el remolque", trailerId.Value);

        var type = await uow.VehicleTypes.GetByIdAsync(trailer.VehicleTypeId, ct);
        if (type is not null && type.Category != VehicleCategory.Towed)
            throw new ConflictException($"La unidad {trailer.EconomicNumber} no es de arrastre y no puede engancharse como remolque.");
    }

    private async Task ValidateCustomerAsync(Guid? customerId, CancellationToken ct)
    {
        if (customerId is null) return;
        if (!await uow.Customers.AnyAsync(c => c.Id == customerId.Value, ct))
            throw new NotFoundException("el cliente", customerId.Value);
    }

    private async Task ReleaseTrailerAsync(Trip trip, CancellationToken ct)
    {
        if (trip.TrailerId is null) return;
        var trailer = await uow.Vehicles.GetByIdAsync(trip.TrailerId.Value, ct);
        if (trailer is null) return;
        trailer.ReleaseFromTrip();
        uow.Vehicles.Update(trailer);
    }
}
