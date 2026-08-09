using FleetErp.Application.Abstractions;
using FleetErp.Application.Common;
using FleetErp.Application.Contracts;
using FleetErp.Application.Services;
using FleetErp.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FleetErp.Api.Controllers;

/// <summary>Inventario de unidades: tractocamiones, cajas, remolques y cualquier tipo que la empresa defina.</summary>
public sealed class VehiclesController(IVehicleService vehicles, IVehicleQueries queries) : ApiControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<VehicleDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<VehicleDto>>> Search(
        [FromQuery] string? search,
        [FromQuery] Guid? vehicleTypeId,
        [FromQuery] VehicleStatus? status,
        [FromQuery] bool? isActive,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default) =>
        Ok(await queries.SearchAsync(new VehicleFilter(search, vehicleTypeId, status, isActive), Paging(page, pageSize), ct));

    /// <summary>Combo de unidades; filtrando por categoría se obtienen solo motrices o solo remolques.</summary>
    [HttpGet("lookup")]
    [ProducesResponseType(typeof(IReadOnlyList<LookupItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<LookupItemDto>>> Lookup(
        [FromQuery] VehicleCategory? category, CancellationToken ct = default) =>
        Ok(await queries.LookupAsync(category, ct));

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(VehicleDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<VehicleDto>> Get(Guid id, CancellationToken ct)
    {
        var vehicle = await queries.GetAsync(id, ct);
        return vehicle is null ? NotFound() : Ok(vehicle);
    }

    [HttpPost]
    [Authorize(Policy = Policies.CanWrite)]
    public async Task<IActionResult> Create(CreateVehicleRequest request, CancellationToken ct) =>
        CreatedResource(await vehicles.CreateAsync(request, ct));

    [HttpPut("{id:guid}")]
    [Authorize(Policy = Policies.CanWrite)]
    public async Task<IActionResult> Update(Guid id, UpdateVehicleRequest request, CancellationToken ct)
    {
        await vehicles.UpdateAsync(id, request, ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/status")]
    [Authorize(Policy = Policies.CanWrite)]
    public async Task<IActionResult> ChangeStatus(Guid id, [FromQuery] VehicleStatus status, CancellationToken ct)
    {
        await vehicles.ChangeStatusAsync(id, status, ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/active")]
    [Authorize(Policy = Policies.CanWrite)]
    public async Task<IActionResult> SetActive(Guid id, [FromQuery] bool active, CancellationToken ct)
    {
        await vehicles.SetActiveAsync(id, active, ct);
        return NoContent();
    }
}
