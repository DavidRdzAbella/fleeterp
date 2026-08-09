using FleetErp.Application.Abstractions;
using FleetErp.Application.Common;
using FleetErp.Application.Contracts;
using FleetErp.Domain.Abstractions;
using FleetErp.Domain.Entities;

namespace FleetErp.Application.Services;

public interface IDriverService
{
    Task<Guid> CreateAsync(UpsertDriverRequest request, CancellationToken ct = default);
    Task UpdateAsync(Guid id, UpsertDriverRequest request, CancellationToken ct = default);
    Task SetActiveAsync(Guid id, bool active, CancellationToken ct = default);
}

public sealed class DriverService(IUnitOfWork uow, ICurrentTenant tenant) : IDriverService
{
    public async Task<Guid> CreateAsync(UpsertDriverRequest request, CancellationToken ct = default)
    {
        await GuardLicenseAsync(request.LicenseNumber, null, ct);

        var driver = new Driver(request.FirstName, request.LastName, request.LicenseNumber)
        {
            TenantId = tenant.TenantId
        };
        Apply(driver, request);

        await uow.Drivers.AddAsync(driver, ct);
        await uow.SaveChangesAsync(ct);
        return driver.Id;
    }

    public async Task UpdateAsync(Guid id, UpsertDriverRequest request, CancellationToken ct = default)
    {
        var driver = await Require(id, ct);
        await GuardLicenseAsync(request.LicenseNumber, id, ct);

        driver.SetName(request.FirstName, request.LastName);
        Apply(driver, request);

        uow.Drivers.Update(driver);
        await uow.SaveChangesAsync(ct);
    }

    public async Task SetActiveAsync(Guid id, bool active, CancellationToken ct = default)
    {
        var driver = await Require(id, ct);
        if (active) driver.Activate(); else driver.Deactivate();
        uow.Drivers.Update(driver);
        await uow.SaveChangesAsync(ct);
    }

    private static void Apply(Driver driver, UpsertDriverRequest request)
    {
        driver.SetLicense(request.LicenseNumber, request.LicenseType, request.LicenseExpiry);
        driver.SetContact(request.EmployeeNumber, request.Phone, request.Email, request.HireDate);
        driver.SetCompensation(request.PayScheme, request.PayRate);
        driver.CustomFields.Replace(request.CustomFields);
    }

    private async Task<Driver> Require(Guid id, CancellationToken ct) =>
        await uow.Drivers.GetByIdAsync(id, ct) ?? throw new NotFoundException("el conductor", id);

    private async Task GuardLicenseAsync(string licenseNumber, Guid? excludeId, CancellationToken ct)
    {
        var license = licenseNumber.Trim().ToUpperInvariant();
        if (await uow.Drivers.AnyAsync(d => d.LicenseNumber == license && (excludeId == null || d.Id != excludeId), ct))
            throw new ConflictException($"Ya existe un conductor con la licencia {license}.");
    }
}
