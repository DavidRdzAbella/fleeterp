using FleetErp.Application.Abstractions;
using FleetErp.Application.Common;
using FleetErp.Application.Contracts;
using FleetErp.Application.Services;
using FleetErp.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FleetErp.Api.Controllers;

/// <summary>
/// Viajes: alta, despacho, cierre y consulta. La escritura y la lectura usan
/// colaboradores distintos (<see cref="ITripService"/> e <see cref="ITripQueries"/>)
/// porque tienen razones de cambio distintas.
/// </summary>
public sealed class TripsController(ITripService trips, ITripQueries queries) : ApiControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<TripListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<TripListItemDto>>> Search(
        [FromQuery] string? search,
        [FromQuery] TripStatus? status,
        [FromQuery] Guid? driverId,
        [FromQuery] Guid? vehicleId,
        [FromQuery] Guid? customerId,
        [FromQuery] DateTimeOffset? fromUtc,
        [FromQuery] DateTimeOffset? toUtc,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default) =>
        Ok(await queries.SearchAsync(
            new TripFilter(search, status, driverId, vehicleId, customerId, fromUtc, toUtc), Paging(page, pageSize), ct));

    [HttpGet("active")]
    [ProducesResponseType(typeof(IReadOnlyList<TripListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<TripListItemDto>>> Active([FromQuery] int max = 10, CancellationToken ct = default) =>
        Ok(await queries.GetActiveAsync(max, ct));

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TripDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TripDetailDto>> Get(Guid id, CancellationToken ct)
    {
        var trip = await queries.GetDetailAsync(id, ct);
        return trip is null ? NotFound() : Ok(trip);
    }

    [HttpPost]
    [Authorize(Policy = Policies.CanWrite)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create(CreateTripRequest request, CancellationToken ct) =>
        CreatedResource(await trips.CreateAsync(request, ct));

    [HttpPut("{id:guid}")]
    [Authorize(Policy = Policies.CanWrite)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Update(Guid id, UpdateTripRequest request, CancellationToken ct)
    {
        await trips.UpdateAsync(id, request, ct);
        return NoContent();
    }

    /// <summary>Registra la salida a ruta: hora, odómetro y combustible inicial.</summary>
    [HttpPost("{id:guid}/dispatch")]
    [Authorize(Policy = Policies.CanWrite)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Dispatch(Guid id, DispatchTripRequest request, CancellationToken ct)
    {
        await trips.DispatchAsync(id, request, ct);
        return NoContent();
    }

    /// <summary>Registra la llegada y cierra los números del viaje.</summary>
    [HttpPost("{id:guid}/complete")]
    [Authorize(Policy = Policies.CanWrite)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Complete(Guid id, CompleteTripRequest request, CancellationToken ct)
    {
        await trips.CompleteAsync(id, request, ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/cancel")]
    [Authorize(Policy = Policies.CanWrite)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Cancel(Guid id, CancelTripRequest request, CancellationToken ct)
    {
        await trips.CancelAsync(id, request, ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = Policies.IsAdministrator)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await trips.DeleteAsync(id, ct);
        return NoContent();
    }
}
