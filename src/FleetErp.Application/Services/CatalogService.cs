using FleetErp.Application.Abstractions;
using FleetErp.Application.Common;
using FleetErp.Application.Contracts;
using FleetErp.Domain.Abstractions;
using FleetErp.Domain.Entities;
using FleetErp.Domain.Enums;

namespace FleetErp.Application.Services;

/// <summary>
/// Catálogos parametrizables. Es la palanca principal de genericidad: tipos de
/// unidad, conceptos de gasto y campos a la medida se configuran por empresa
/// desde la propia aplicación.
/// </summary>
public interface ICatalogService
{
    Task<IReadOnlyList<VehicleTypeDto>> GetVehicleTypesAsync(bool includeInactive, CancellationToken ct = default);
    Task<Guid> CreateVehicleTypeAsync(UpsertVehicleTypeRequest request, CancellationToken ct = default);
    Task UpdateVehicleTypeAsync(Guid id, UpsertVehicleTypeRequest request, CancellationToken ct = default);
    Task SetVehicleTypeActiveAsync(Guid id, bool active, CancellationToken ct = default);

    Task<IReadOnlyList<ExpenseCategoryDto>> GetExpenseCategoriesAsync(bool includeInactive, CancellationToken ct = default);
    Task<Guid> CreateExpenseCategoryAsync(UpsertExpenseCategoryRequest request, CancellationToken ct = default);
    Task UpdateExpenseCategoryAsync(Guid id, UpsertExpenseCategoryRequest request, CancellationToken ct = default);
    Task SetExpenseCategoryActiveAsync(Guid id, bool active, CancellationToken ct = default);

    Task<IReadOnlyList<CustomFieldDefinitionDto>> GetCustomFieldsAsync(CustomFieldTarget? target, bool includeInactive, CancellationToken ct = default);
    Task<Guid> CreateCustomFieldAsync(UpsertCustomFieldDefinitionRequest request, CancellationToken ct = default);
    Task UpdateCustomFieldAsync(Guid id, UpsertCustomFieldDefinitionRequest request, CancellationToken ct = default);
    Task SetCustomFieldActiveAsync(Guid id, bool active, CancellationToken ct = default);
}

public sealed class CatalogService(IUnitOfWork uow, ICurrentTenant tenant) : ICatalogService
{
    // ---- Tipos de unidad --------------------------------------------------

    public async Task<IReadOnlyList<VehicleTypeDto>> GetVehicleTypesAsync(bool includeInactive, CancellationToken ct = default)
    {
        var items = await uow.VehicleTypes.ListAsync(t => includeInactive || t.IsActive, ct);
        return items
            .OrderBy(t => t.Category).ThenBy(t => t.Name)
            .Select(t => new VehicleTypeDto(t.Id, t.Code, t.Name, t.Category, t.IsActive))
            .ToList();
    }

    public async Task<Guid> CreateVehicleTypeAsync(UpsertVehicleTypeRequest request, CancellationToken ct = default)
    {
        await GuardCodeAsync<VehicleType>(request.Code, null, ct, t => t.Code, "tipo de unidad");
        var type = new VehicleType(request.Code, request.Name, request.Category) { TenantId = tenant.TenantId };
        await uow.VehicleTypes.AddAsync(type, ct);
        await uow.SaveChangesAsync(ct);
        return type.Id;
    }

    public async Task UpdateVehicleTypeAsync(Guid id, UpsertVehicleTypeRequest request, CancellationToken ct = default)
    {
        var type = await uow.VehicleTypes.GetByIdAsync(id, ct) ?? throw new NotFoundException("el tipo de unidad", id);
        await GuardCodeAsync<VehicleType>(request.Code, id, ct, t => t.Code, "tipo de unidad");
        type.Update(request.Code, request.Name, request.Category);
        uow.VehicleTypes.Update(type);
        await uow.SaveChangesAsync(ct);
    }

    public async Task SetVehicleTypeActiveAsync(Guid id, bool active, CancellationToken ct = default)
    {
        var type = await uow.VehicleTypes.GetByIdAsync(id, ct) ?? throw new NotFoundException("el tipo de unidad", id);

        if (!active && await uow.Vehicles.AnyAsync(v => v.VehicleTypeId == id && v.IsActive, ct))
            throw new ConflictException("No se puede desactivar un tipo con unidades activas asignadas.");

        if (active) type.Activate(); else type.Deactivate();
        uow.VehicleTypes.Update(type);
        await uow.SaveChangesAsync(ct);
    }

    // ---- Conceptos de gasto ----------------------------------------------

    public async Task<IReadOnlyList<ExpenseCategoryDto>> GetExpenseCategoriesAsync(bool includeInactive, CancellationToken ct = default)
    {
        var items = await uow.ExpenseCategories.ListAsync(c => includeInactive || c.IsActive, ct);
        return items
            .OrderBy(c => c.Name)
            .Select(c => new ExpenseCategoryDto(c.Id, c.Code, c.Name, c.IsTripRelated, c.IsActive))
            .ToList();
    }

    public async Task<Guid> CreateExpenseCategoryAsync(UpsertExpenseCategoryRequest request, CancellationToken ct = default)
    {
        await GuardCodeAsync<ExpenseCategory>(request.Code, null, ct, c => c.Code, "concepto de gasto");
        var category = new ExpenseCategory(request.Code, request.Name, request.IsTripRelated) { TenantId = tenant.TenantId };
        await uow.ExpenseCategories.AddAsync(category, ct);
        await uow.SaveChangesAsync(ct);
        return category.Id;
    }

    public async Task UpdateExpenseCategoryAsync(Guid id, UpsertExpenseCategoryRequest request, CancellationToken ct = default)
    {
        var category = await uow.ExpenseCategories.GetByIdAsync(id, ct) ?? throw new NotFoundException("el concepto de gasto", id);
        await GuardCodeAsync<ExpenseCategory>(request.Code, id, ct, c => c.Code, "concepto de gasto");
        category.Update(request.Code, request.Name, request.IsTripRelated);
        uow.ExpenseCategories.Update(category);
        await uow.SaveChangesAsync(ct);
    }

    public async Task SetExpenseCategoryActiveAsync(Guid id, bool active, CancellationToken ct = default)
    {
        var category = await uow.ExpenseCategories.GetByIdAsync(id, ct) ?? throw new NotFoundException("el concepto de gasto", id);
        if (active) category.Activate(); else category.Deactivate();
        uow.ExpenseCategories.Update(category);
        await uow.SaveChangesAsync(ct);
    }

    // ---- Campos configurables --------------------------------------------

    public async Task<IReadOnlyList<CustomFieldDefinitionDto>> GetCustomFieldsAsync(
        CustomFieldTarget? target, bool includeInactive, CancellationToken ct = default)
    {
        var items = await uow.CustomFieldDefinitions.ListAsync(
            f => (target == null || f.Target == target) && (includeInactive || f.IsActive), ct);

        return items
            .OrderBy(f => f.Target).ThenBy(f => f.DisplayOrder).ThenBy(f => f.Label)
            .Select(f => new CustomFieldDefinitionDto(
                f.Id, f.Target, f.Key, f.Label, f.Type, f.IsRequired, f.OptionList, f.DisplayOrder, f.IsActive))
            .ToList();
    }

    public async Task<Guid> CreateCustomFieldAsync(UpsertCustomFieldDefinitionRequest request, CancellationToken ct = default)
    {
        var key = Normalize(request.Key);
        if (await uow.CustomFieldDefinitions.AnyAsync(f => f.Target == request.Target && f.Key == key, ct))
            throw new ConflictException($"Ya existe un campo '{key}' para {request.Target}.");

        var field = new CustomFieldDefinition(request.Target, request.Key, request.Label, request.Type,
                                              request.IsRequired, request.Options, request.DisplayOrder)
        {
            TenantId = tenant.TenantId
        };

        await uow.CustomFieldDefinitions.AddAsync(field, ct);
        await uow.SaveChangesAsync(ct);
        return field.Id;
    }

    public async Task UpdateCustomFieldAsync(Guid id, UpsertCustomFieldDefinitionRequest request, CancellationToken ct = default)
    {
        var field = await uow.CustomFieldDefinitions.GetByIdAsync(id, ct) ?? throw new NotFoundException("el campo configurable", id);
        var key = Normalize(request.Key);
        if (await uow.CustomFieldDefinitions.AnyAsync(f => f.Target == request.Target && f.Key == key && f.Id != id, ct))
            throw new ConflictException($"Ya existe un campo '{key}' para {request.Target}.");

        field.Update(request.Key, request.Label, request.Type, request.IsRequired, request.Options, request.DisplayOrder);
        uow.CustomFieldDefinitions.Update(field);
        await uow.SaveChangesAsync(ct);
    }

    public async Task SetCustomFieldActiveAsync(Guid id, bool active, CancellationToken ct = default)
    {
        var field = await uow.CustomFieldDefinitions.GetByIdAsync(id, ct) ?? throw new NotFoundException("el campo configurable", id);
        if (active) field.Activate(); else field.Deactivate();
        uow.CustomFieldDefinitions.Update(field);
        await uow.SaveChangesAsync(ct);
    }

    private static string Normalize(string key) => key.Trim().Replace(' ', '_').ToLowerInvariant();

    private async Task GuardCodeAsync<T>(string code, Guid? excludeId, CancellationToken ct,
        Func<T, string> codeSelector, string label) where T : Domain.Common.BaseEntity
    {
        var normalized = code.Trim().ToUpperInvariant();
        var existing = await uow.Repository<T>().ListAsync(ct);
        if (existing.Any(e => codeSelector(e) == normalized && (excludeId == null || e.Id != excludeId)))
            throw new ConflictException($"Ya existe un {label} con el código {normalized}.");
    }
}
