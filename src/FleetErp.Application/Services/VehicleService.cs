using FleetErp.Application.Abstractions;
using FleetErp.Application.Common;
using FleetErp.Application.Contracts;
using FleetErp.Domain.Abstractions;
using FleetErp.Domain.Entities;
using FleetErp.Domain.Enums;

namespace FleetErp.Application.Services;

public interface IVehicleService
{
    Task<Guid> CreateAsync(CreateVehicleRequest request, CancellationToken ct = default);
    Task UpdateAsync(Guid id, UpdateVehicleRequest request, CancellationToken ct = default);
    Task ChangeStatusAsync(Guid id, VehicleStatus status, CancellationToken ct = default);
    Task SetActiveAsync(Guid id, bool active, CancellationToken ct = default);
}

public sealed class VehicleService(IUnitOfWork uow, ICurrentTenant tenant) : IVehicleService
{
    public async Task<Guid> CreateAsync(CreateVehicleRequest request, CancellationToken ct = default)
    {
        await RequireVehicleTypeAsync(request.VehicleTypeId, ct);
        await GuardUniquenessAsync(request.EconomicNumber, request.PlateNumber, null, ct);

        var vehicle = new Vehicle(request.EconomicNumber, request.PlateNumber, request.VehicleTypeId)
        {
            TenantId = tenant.TenantId
        };
        vehicle.SetSpecs(request.Brand, request.Model, request.Year, request.Vin, request.CargoCapacity, request.TankCapacity);
        vehicle.SetCompliance(request.InsuranceExpiry, request.CirculationCardExpiry);
        vehicle.SetInitialOdometer(request.InitialOdometer);
        vehicle.CustomFields.Replace(request.CustomFields);

        await uow.Vehicles.AddAsync(vehicle, ct);
        await uow.SaveChangesAsync(ct);
        return vehicle.Id;
    }

    public async Task UpdateAsync(Guid id, UpdateVehicleRequest request, CancellationToken ct = default)
    {
        var vehicle = await Require(id, ct);
        await RequireVehicleTypeAsync(request.VehicleTypeId, ct);
        await GuardUniquenessAsync(request.EconomicNumber, request.PlateNumber, id, ct);

        vehicle.SetIdentification(request.EconomicNumber, request.PlateNumber);
        vehicle.ChangeType(request.VehicleTypeId);
        vehicle.SetSpecs(request.Brand, request.Model, request.Year, request.Vin, request.CargoCapacity, request.TankCapacity);
        vehicle.SetCompliance(request.InsuranceExpiry, request.CirculationCardExpiry);
        vehicle.CustomFields.Replace(request.CustomFields);

        uow.Vehicles.Update(vehicle);
        await uow.SaveChangesAsync(ct);
    }

    public async Task ChangeStatusAsync(Guid id, VehicleStatus status, CancellationToken ct = default)
    {
        var vehicle = await Require(id, ct);

        // El estado "en viaje" lo pone y lo quita el despacho, nunca la pantalla
        // de unidades: si se pudiera forzar aquí, se rompería la consistencia
        // entre la unidad y el viaje que la tiene tomada.
        if (status == VehicleStatus.OnTrip)
            throw new ConflictException("El estado 'en viaje' se asigna al despachar un viaje, no manualmente.");
        if (vehicle.Status == VehicleStatus.OnTrip)
            throw new ConflictException("La unidad está en viaje; concluya o cancele el viaje para cambiar su estado.");

        switch (status)
        {
            case VehicleStatus.Available: vehicle.ReturnToService(); break;
            case VehicleStatus.InMaintenance: vehicle.SendToMaintenance(); break;
            case VehicleStatus.OutOfService: vehicle.SetOutOfService(); break;
        }

        uow.Vehicles.Update(vehicle);
        await uow.SaveChangesAsync(ct);
    }

    public async Task SetActiveAsync(Guid id, bool active, CancellationToken ct = default)
    {
        var vehicle = await Require(id, ct);
        if (active) vehicle.Activate(); else vehicle.Deactivate();
        uow.Vehicles.Update(vehicle);
        await uow.SaveChangesAsync(ct);
    }

    private async Task<Vehicle> Require(Guid id, CancellationToken ct) =>
        await uow.Vehicles.GetByIdAsync(id, ct) ?? throw new NotFoundException("la unidad", id);

    private async Task RequireVehicleTypeAsync(Guid typeId, CancellationToken ct)
    {
        if (!await uow.VehicleTypes.AnyAsync(t => t.Id == typeId, ct))
            throw new NotFoundException("el tipo de unidad", typeId);
    }

    private async Task GuardUniquenessAsync(string economicNumber, string plate, Guid? excludeId, CancellationToken ct)
    {
        var eco = economicNumber.Trim().ToUpperInvariant();
        var plateUpper = plate.Trim().ToUpperInvariant();

        if (await uow.Vehicles.AnyAsync(v => v.EconomicNumber == eco && (excludeId == null || v.Id != excludeId), ct))
            throw new ConflictException($"Ya existe una unidad con el número económico {eco}.");

        if (await uow.Vehicles.AnyAsync(v => v.PlateNumber == plateUpper && (excludeId == null || v.Id != excludeId), ct))
            throw new ConflictException($"Ya existe una unidad con la placa {plateUpper}.");
    }
}
