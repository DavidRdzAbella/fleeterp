using FleetErp.Application.Abstractions;
using FleetErp.Application.Common;
using FleetErp.Application.Contracts;
using FleetErp.Domain.Abstractions;
using FleetErp.Domain.Entities;
using FleetErp.Domain.Enums;

namespace FleetErp.Application.Services;

// ---- Combustible ----------------------------------------------------------

public interface IFuelLogService
{
    Task<Guid> CreateAsync(CreateFuelLogRequest request, CancellationToken ct = default);
    Task UpdateAsync(Guid id, UpdateFuelLogRequest request, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}

public sealed class FuelLogService(IUnitOfWork uow, ICurrentTenant tenant) : IFuelLogService
{
    public async Task<Guid> CreateAsync(CreateFuelLogRequest request, CancellationToken ct = default)
    {
        var vehicle = await uow.Vehicles.GetByIdAsync(request.VehicleId, ct)
                      ?? throw new NotFoundException("la unidad", request.VehicleId);

        Trip? trip = null;
        if (request.TripId is not null)
        {
            trip = await uow.Trips.GetByIdAsync(request.TripId.Value, ct)
                   ?? throw new NotFoundException("el viaje", request.TripId.Value);
            if (trip.Status == TripStatus.Cancelled)
                throw new ConflictException("No se pueden registrar cargas en un viaje cancelado.");
        }

        var log = new FuelLog(request.VehicleId, request.LoadedAtUtc, request.Quantity, request.PricePerUnit)
        {
            TenantId = tenant.TenantId
        };
        log.SetContext(request.TripId, request.DriverId ?? trip?.DriverId, request.OdometerReading,
                       request.Station, request.ReferenceNumber);

        // Cargar combustible normalmente implica una lectura fresca del odómetro:
        // aprovecharla mantiene al inventario al día sin captura adicional.
        if (request.OdometerReading is not null && request.OdometerReading.Value >= vehicle.CurrentOdometer)
        {
            vehicle.UpdateOdometer(request.OdometerReading.Value);
            uow.Vehicles.Update(vehicle);
        }

        await uow.FuelLogs.AddAsync(log, ct);
        await uow.SaveChangesAsync(ct);
        return log.Id;
    }

    public async Task UpdateAsync(Guid id, UpdateFuelLogRequest request, CancellationToken ct = default)
    {
        var log = await uow.FuelLogs.GetByIdAsync(id, ct)
                  ?? throw new NotFoundException("la carga de combustible", id);

        if (!await uow.Vehicles.AnyAsync(v => v.Id == request.VehicleId, ct))
            throw new NotFoundException("la unidad", request.VehicleId);

        if (request.TripId is not null && !await uow.Trips.AnyAsync(t => t.Id == request.TripId.Value, ct))
            throw new NotFoundException("el viaje", request.TripId.Value);

        log.Reassign(request.VehicleId, request.LoadedAtUtc);
        log.SetAmounts(request.Quantity, request.PricePerUnit);
        log.SetContext(request.TripId, request.DriverId, request.OdometerReading, request.Station, request.ReferenceNumber);

        uow.FuelLogs.Update(log);
        await uow.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var log = await uow.FuelLogs.GetByIdAsync(id, ct) ?? throw new NotFoundException("la carga de combustible", id);
        uow.FuelLogs.Remove(log);
        await uow.SaveChangesAsync(ct);
    }
}

// ---- Gastos ---------------------------------------------------------------

public interface IExpenseService
{
    Task<Guid> CreateAsync(CreateExpenseRequest request, CancellationToken ct = default);
    Task UpdateAsync(Guid id, UpdateExpenseRequest request, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}

public sealed class ExpenseService(IUnitOfWork uow, ICurrentTenant tenant) : IExpenseService
{
    public async Task<Guid> CreateAsync(CreateExpenseRequest request, CancellationToken ct = default)
    {
        if (!await uow.ExpenseCategories.AnyAsync(c => c.Id == request.CategoryId, ct))
            throw new NotFoundException("el concepto de gasto", request.CategoryId);

        Guid? driverId = request.DriverId;
        if (request.TripId is not null)
        {
            var trip = await uow.Trips.GetByIdAsync(request.TripId.Value, ct)
                       ?? throw new NotFoundException("el viaje", request.TripId.Value);
            if (trip.Status == TripStatus.Cancelled)
                throw new ConflictException("No se pueden registrar gastos en un viaje cancelado.");
            driverId ??= trip.DriverId;
        }

        var expense = new Expense(request.CategoryId, request.IncurredAtUtc, request.Amount, request.Description)
        {
            TenantId = tenant.TenantId
        };
        expense.SetContext(request.TripId, request.VehicleId, driverId, request.ReferenceNumber);

        await uow.Expenses.AddAsync(expense, ct);
        await uow.SaveChangesAsync(ct);
        return expense.Id;
    }

    public async Task UpdateAsync(Guid id, UpdateExpenseRequest request, CancellationToken ct = default)
    {
        var expense = await uow.Expenses.GetByIdAsync(id, ct) ?? throw new NotFoundException("el gasto", id);

        if (!await uow.ExpenseCategories.AnyAsync(c => c.Id == request.CategoryId, ct))
            throw new NotFoundException("el concepto de gasto", request.CategoryId);

        if (request.TripId is not null && !await uow.Trips.AnyAsync(t => t.Id == request.TripId.Value, ct))
            throw new NotFoundException("el viaje", request.TripId.Value);

        expense.Recategorize(request.CategoryId, request.IncurredAtUtc);
        expense.SetDetails(request.Amount, request.Description);
        expense.SetContext(request.TripId, request.VehicleId, request.DriverId, request.ReferenceNumber);

        uow.Expenses.Update(expense);
        await uow.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var expense = await uow.Expenses.GetByIdAsync(id, ct) ?? throw new NotFoundException("el gasto", id);
        uow.Expenses.Remove(expense);
        await uow.SaveChangesAsync(ct);
    }
}

// ---- Mantenimiento --------------------------------------------------------

public interface IMaintenanceService
{
    Task<Guid> CreateAsync(CreateMaintenanceOrderRequest request, CancellationToken ct = default);
    Task StartAsync(Guid id, CancellationToken ct = default);
    Task CloseAsync(Guid id, CloseMaintenanceOrderRequest request, CancellationToken ct = default);
}

public sealed class MaintenanceService(IUnitOfWork uow, ICurrentTenant tenant, IFolioGenerator folios) : IMaintenanceService
{
    public async Task<Guid> CreateAsync(CreateMaintenanceOrderRequest request, CancellationToken ct = default)
    {
        var vehicle = await uow.Vehicles.GetByIdAsync(request.VehicleId, ct)
                      ?? throw new NotFoundException("la unidad", request.VehicleId);

        if (vehicle.Status == VehicleStatus.OnTrip)
            throw new ConflictException($"La unidad {vehicle.EconomicNumber} está en viaje; no puede entrar a taller.");

        var folio = await folios.NextMaintenanceFolioAsync(ct);
        var order = new MaintenanceOrder(folio, request.VehicleId, request.Kind, request.OpenedAtUtc, request.Description)
        {
            TenantId = tenant.TenantId
        };

        vehicle.SendToMaintenance();
        uow.Vehicles.Update(vehicle);

        await uow.MaintenanceOrders.AddAsync(order, ct);
        await uow.SaveChangesAsync(ct);
        return order.Id;
    }

    public async Task StartAsync(Guid id, CancellationToken ct = default)
    {
        var order = await Require(id, ct);
        order.Start();
        uow.MaintenanceOrders.Update(order);
        await uow.SaveChangesAsync(ct);
    }

    public async Task CloseAsync(Guid id, CloseMaintenanceOrderRequest request, CancellationToken ct = default)
    {
        var order = await Require(id, ct);
        order.Close(request.ClosedAtUtc, request.Cost, request.Workshop, request.OdometerAtService);

        var vehicle = await uow.Vehicles.GetByIdAsync(order.VehicleId, ct);
        if (vehicle is not null)
        {
            if (request.OdometerAtService is not null && request.OdometerAtService.Value >= vehicle.CurrentOdometer)
                vehicle.UpdateOdometer(request.OdometerAtService.Value);

            // Solo se libera la unidad cuando ya no le queda ninguna orden abierta.
            var stillOpen = await uow.MaintenanceOrders.AnyAsync(
                o => o.VehicleId == order.VehicleId && o.Id != order.Id && o.Status != MaintenanceStatus.Closed, ct);
            if (!stillOpen) vehicle.ReturnFromMaintenance();

            uow.Vehicles.Update(vehicle);
        }

        uow.MaintenanceOrders.Update(order);
        await uow.SaveChangesAsync(ct);
    }

    private async Task<MaintenanceOrder> Require(Guid id, CancellationToken ct) =>
        await uow.MaintenanceOrders.GetByIdAsync(id, ct) ?? throw new NotFoundException("la orden de servicio", id);
}
