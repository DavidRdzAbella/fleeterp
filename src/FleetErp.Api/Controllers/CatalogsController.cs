using FleetErp.Application.Contracts;
using FleetErp.Application.Services;
using FleetErp.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FleetErp.Api.Controllers;

/// <summary>
/// Parametrización de la empresa. Es el punto de entrada para adaptar el ERP a un
/// cliente nuevo: tipos de unidad, conceptos de gasto y campos a la medida.
/// </summary>
[Route("api/catalogs")]
public sealed class CatalogsController(ICatalogService catalogs) : ApiControllerBase
{
    // ---- Tipos de unidad --------------------------------------------------

    [HttpGet("vehicle-types")]
    public async Task<ActionResult<IReadOnlyList<VehicleTypeDto>>> VehicleTypes(
        [FromQuery] bool includeInactive = false, CancellationToken ct = default) =>
        Ok(await catalogs.GetVehicleTypesAsync(includeInactive, ct));

    [HttpPost("vehicle-types")]
    [Authorize(Policy = Policies.IsAdministrator)]
    public async Task<IActionResult> CreateVehicleType(UpsertVehicleTypeRequest request, CancellationToken ct) =>
        CreatedResource(await catalogs.CreateVehicleTypeAsync(request, ct));

    [HttpPut("vehicle-types/{id:guid}")]
    [Authorize(Policy = Policies.IsAdministrator)]
    public async Task<IActionResult> UpdateVehicleType(Guid id, UpsertVehicleTypeRequest request, CancellationToken ct)
    {
        await catalogs.UpdateVehicleTypeAsync(id, request, ct);
        return NoContent();
    }

    [HttpPost("vehicle-types/{id:guid}/active")]
    [Authorize(Policy = Policies.IsAdministrator)]
    public async Task<IActionResult> SetVehicleTypeActive(Guid id, [FromQuery] bool active, CancellationToken ct)
    {
        await catalogs.SetVehicleTypeActiveAsync(id, active, ct);
        return NoContent();
    }

    // ---- Conceptos de gasto ----------------------------------------------

    [HttpGet("expense-categories")]
    public async Task<ActionResult<IReadOnlyList<ExpenseCategoryDto>>> ExpenseCategories(
        [FromQuery] bool includeInactive = false, CancellationToken ct = default) =>
        Ok(await catalogs.GetExpenseCategoriesAsync(includeInactive, ct));

    [HttpPost("expense-categories")]
    [Authorize(Policy = Policies.IsAdministrator)]
    public async Task<IActionResult> CreateExpenseCategory(UpsertExpenseCategoryRequest request, CancellationToken ct) =>
        CreatedResource(await catalogs.CreateExpenseCategoryAsync(request, ct));

    [HttpPut("expense-categories/{id:guid}")]
    [Authorize(Policy = Policies.IsAdministrator)]
    public async Task<IActionResult> UpdateExpenseCategory(Guid id, UpsertExpenseCategoryRequest request, CancellationToken ct)
    {
        await catalogs.UpdateExpenseCategoryAsync(id, request, ct);
        return NoContent();
    }

    [HttpPost("expense-categories/{id:guid}/active")]
    [Authorize(Policy = Policies.IsAdministrator)]
    public async Task<IActionResult> SetExpenseCategoryActive(Guid id, [FromQuery] bool active, CancellationToken ct)
    {
        await catalogs.SetExpenseCategoryActiveAsync(id, active, ct);
        return NoContent();
    }

    // ---- Campos configurables --------------------------------------------

    [HttpGet("custom-fields")]
    public async Task<ActionResult<IReadOnlyList<CustomFieldDefinitionDto>>> CustomFields(
        [FromQuery] CustomFieldTarget? target, [FromQuery] bool includeInactive = false, CancellationToken ct = default) =>
        Ok(await catalogs.GetCustomFieldsAsync(target, includeInactive, ct));

    [HttpPost("custom-fields")]
    [Authorize(Policy = Policies.IsAdministrator)]
    public async Task<IActionResult> CreateCustomField(UpsertCustomFieldDefinitionRequest request, CancellationToken ct) =>
        CreatedResource(await catalogs.CreateCustomFieldAsync(request, ct));

    [HttpPut("custom-fields/{id:guid}")]
    [Authorize(Policy = Policies.IsAdministrator)]
    public async Task<IActionResult> UpdateCustomField(Guid id, UpsertCustomFieldDefinitionRequest request, CancellationToken ct)
    {
        await catalogs.UpdateCustomFieldAsync(id, request, ct);
        return NoContent();
    }

    [HttpPost("custom-fields/{id:guid}/active")]
    [Authorize(Policy = Policies.IsAdministrator)]
    public async Task<IActionResult> SetCustomFieldActive(Guid id, [FromQuery] bool active, CancellationToken ct)
    {
        await catalogs.SetCustomFieldActiveAsync(id, active, ct);
        return NoContent();
    }
}

/// <summary>Perfil y parametrización de la empresa en sesión.</summary>
[Route("api/tenant")]
public sealed class TenantController(ITenantService tenants) : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<TenantDto>> Current(CancellationToken ct) => Ok(await tenants.GetCurrentAsync(ct));

    [HttpPut]
    [Authorize(Policy = Policies.IsAdministrator)]
    public async Task<IActionResult> UpdateProfile(UpdateTenantRequest request, CancellationToken ct)
    {
        await tenants.UpdateProfileAsync(request, ct);
        return NoContent();
    }

    [HttpPut("settings")]
    [Authorize(Policy = Policies.IsAdministrator)]
    public async Task<IActionResult> UpdateSettings(TenantSettingsDto settings, CancellationToken ct)
    {
        await tenants.UpdateSettingsAsync(settings, ct);
        return NoContent();
    }
}
