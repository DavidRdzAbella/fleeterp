using FleetErp.Application.Abstractions;
using FleetErp.Application.Common;
using FleetErp.Application.Contracts;
using FleetErp.Domain.Abstractions;

namespace FleetErp.Application.Services;

/// <summary>
/// Lectura y ajuste de la parametrización de la empresa en sesión. Cambiar aquí
/// moneda, unidades o tarifas por defecto es lo que reconfigura el ERP completo.
/// </summary>
public interface ITenantService
{
    Task<TenantDto> GetCurrentAsync(CancellationToken ct = default);
    Task UpdateProfileAsync(UpdateTenantRequest request, CancellationToken ct = default);
    Task UpdateSettingsAsync(TenantSettingsDto settings, CancellationToken ct = default);
}

public sealed class TenantService(IUnitOfWork uow, ICurrentTenant tenant) : ITenantService
{
    public async Task<TenantDto> GetCurrentAsync(CancellationToken ct = default)
    {
        var entity = await Require(ct);
        return new TenantDto(entity.Id, entity.Name, entity.Slug, entity.TaxId, entity.ContactEmail,
                             entity.Phone, entity.IsActive, TenantSettingsDto.From(entity.Settings));
    }

    public async Task UpdateProfileAsync(UpdateTenantRequest request, CancellationToken ct = default)
    {
        var entity = await Require(ct);
        entity.Rename(request.Name);
        entity.SetContact(request.TaxId, request.ContactEmail, request.Phone);
        uow.Tenants.Update(entity);
        await uow.SaveChangesAsync(ct);
    }

    public async Task UpdateSettingsAsync(TenantSettingsDto settings, CancellationToken ct = default)
    {
        var entity = await Require(ct);
        entity.UpdateSettings(settings.ToEntity());
        uow.Tenants.Update(entity);
        await uow.SaveChangesAsync(ct);
    }

    private async Task<Domain.Entities.Tenant> Require(CancellationToken ct)
    {
        using var _ = tenant.BypassFilter();
        return await uow.Tenants.GetByIdAsync(tenant.TenantId, ct)
               ?? throw new NotFoundException("la empresa", tenant.TenantId);
    }
}
