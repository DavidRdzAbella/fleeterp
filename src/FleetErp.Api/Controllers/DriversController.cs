using FleetErp.Application.Abstractions;
using FleetErp.Application.Common;
using FleetErp.Application.Contracts;
using FleetErp.Application.Services;
using FleetErp.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FleetErp.Api.Controllers;

public sealed class DriversController(IDriverService drivers, IDriverQueries queries, IAnalyticsService analytics)
    : ApiControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<DriverDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<DriverDto>>> Search(
        [FromQuery] string? search,
        [FromQuery] DriverStatus? status,
        [FromQuery] bool? isActive,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default) =>
        Ok(await queries.SearchAsync(new DriverFilter(search, status, isActive), Paging(page, pageSize), ct));

    [HttpGet("lookup")]
    public async Task<ActionResult<IReadOnlyList<LookupItemDto>>> Lookup(CancellationToken ct) =>
        Ok(await queries.LookupAsync(ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<DriverDto>> Get(Guid id, CancellationToken ct)
    {
        var driver = await queries.GetAsync(id, ct);
        return driver is null ? NotFound() : Ok(driver);
    }

    /// <summary>Desempeño del conductor en el periodo: kilómetros, combustible, ventas y utilidad.</summary>
    [HttpGet("{id:guid}/performance")]
    [ProducesResponseType(typeof(DriverPerformanceDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<DriverPerformanceDto>> Performance(
        Guid id, [FromQuery] DateTimeOffset? fromUtc, [FromQuery] DateTimeOffset? toUtc, CancellationToken ct) =>
        Ok(await analytics.GetDriverPerformanceAsync(id, BuildPeriod(fromUtc, toUtc), ct));

    [HttpPost]
    [Authorize(Policy = Policies.CanWrite)]
    public async Task<IActionResult> Create(UpsertDriverRequest request, CancellationToken ct) =>
        CreatedResource(await drivers.CreateAsync(request, ct));

    [HttpPut("{id:guid}")]
    [Authorize(Policy = Policies.CanWrite)]
    public async Task<IActionResult> Update(Guid id, UpsertDriverRequest request, CancellationToken ct)
    {
        await drivers.UpdateAsync(id, request, ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/active")]
    [Authorize(Policy = Policies.CanWrite)]
    public async Task<IActionResult> SetActive(Guid id, [FromQuery] bool active, CancellationToken ct)
    {
        await drivers.SetActiveAsync(id, active, ct);
        return NoContent();
    }

    internal static AnalyticsPeriod? BuildPeriod(DateTimeOffset? fromUtc, DateTimeOffset? toUtc) =>
        fromUtc is null || toUtc is null ? null : new AnalyticsPeriod(fromUtc.Value, toUtc.Value);
}
