using FleetErp.Application.Abstractions;
using FleetErp.Application.Common;
using FleetErp.Application.Contracts;
using FleetErp.Application.Services;
using FleetErp.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FleetErp.Api.Controllers;

public sealed class CustomersController(ICustomerService customers, ICustomerQueries queries) : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<CustomerDto>>> Search(
        [FromQuery] string? search, [FromQuery] bool? isActive,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default) =>
        Ok(await queries.SearchAsync(search, isActive, Paging(page, pageSize), ct));

    [HttpGet("lookup")]
    public async Task<ActionResult<IReadOnlyList<LookupItemDto>>> Lookup(CancellationToken ct) =>
        Ok(await queries.LookupAsync(ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CustomerDto>> Get(Guid id, CancellationToken ct)
    {
        var customer = await queries.GetAsync(id, ct);
        return customer is null ? NotFound() : Ok(customer);
    }

    [HttpPost]
    [Authorize(Policy = Policies.CanWrite)]
    public async Task<IActionResult> Create(UpsertCustomerRequest request, CancellationToken ct) =>
        CreatedResource(await customers.CreateAsync(request, ct));

    [HttpPut("{id:guid}")]
    [Authorize(Policy = Policies.CanWrite)]
    public async Task<IActionResult> Update(Guid id, UpsertCustomerRequest request, CancellationToken ct)
    {
        await customers.UpdateAsync(id, request, ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/active")]
    [Authorize(Policy = Policies.CanWrite)]
    public async Task<IActionResult> SetActive(Guid id, [FromQuery] bool active, CancellationToken ct)
    {
        await customers.SetActiveAsync(id, active, ct);
        return NoContent();
    }
}

[Route("api/fuel-logs")]
public sealed class FuelLogsController(IFuelLogService fuel, IFuelLogQueries queries) : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<FuelLogDto>>> Search(
        [FromQuery] Guid? vehicleId, [FromQuery] Guid? tripId,
        [FromQuery] DateTimeOffset? fromUtc, [FromQuery] DateTimeOffset? toUtc,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default) =>
        Ok(await queries.SearchAsync(vehicleId, tripId, fromUtc, toUtc, Paging(page, pageSize), ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<FuelLogDto>> Get(Guid id, CancellationToken ct)
    {
        var log = await queries.GetAsync(id, ct);
        return log is null ? NotFound() : Ok(log);
    }

    [HttpPost]
    [Authorize(Policy = Policies.CanWrite)]
    public async Task<IActionResult> Create(CreateFuelLogRequest request, CancellationToken ct) =>
        CreatedResource(await fuel.CreateAsync(request, ct));

    [HttpPut("{id:guid}")]
    [Authorize(Policy = Policies.CanWrite)]
    public async Task<IActionResult> Update(Guid id, UpdateFuelLogRequest request, CancellationToken ct)
    {
        await fuel.UpdateAsync(id, request, ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = Policies.CanWrite)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await fuel.DeleteAsync(id, ct);
        return NoContent();
    }
}

public sealed class ExpensesController(IExpenseService expenses, IExpenseQueries queries) : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<ExpenseDto>>> Search(
        [FromQuery] Guid? categoryId, [FromQuery] Guid? tripId, [FromQuery] Guid? vehicleId, [FromQuery] Guid? driverId,
        [FromQuery] DateTimeOffset? fromUtc, [FromQuery] DateTimeOffset? toUtc,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default) =>
        Ok(await queries.SearchAsync(
            new ExpenseFilter(categoryId, tripId, vehicleId, driverId, fromUtc, toUtc), Paging(page, pageSize), ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ExpenseDto>> Get(Guid id, CancellationToken ct)
    {
        var expense = await queries.GetAsync(id, ct);
        return expense is null ? NotFound() : Ok(expense);
    }

    [HttpPost]
    [Authorize(Policy = Policies.CanWrite)]
    public async Task<IActionResult> Create(CreateExpenseRequest request, CancellationToken ct) =>
        CreatedResource(await expenses.CreateAsync(request, ct));

    [HttpPut("{id:guid}")]
    [Authorize(Policy = Policies.CanWrite)]
    public async Task<IActionResult> Update(Guid id, UpdateExpenseRequest request, CancellationToken ct)
    {
        await expenses.UpdateAsync(id, request, ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = Policies.CanWrite)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await expenses.DeleteAsync(id, ct);
        return NoContent();
    }
}

[Route("api/maintenance")]
public sealed class MaintenanceController(IMaintenanceService maintenance, IMaintenanceQueries queries) : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<MaintenanceOrderDto>>> Search(
        [FromQuery] Guid? vehicleId, [FromQuery] MaintenanceStatus? status,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default) =>
        Ok(await queries.SearchAsync(vehicleId, status, Paging(page, pageSize), ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<MaintenanceOrderDto>> Get(Guid id, CancellationToken ct)
    {
        var order = await queries.GetAsync(id, ct);
        return order is null ? NotFound() : Ok(order);
    }

    [HttpPost]
    [Authorize(Policy = Policies.CanWrite)]
    public async Task<IActionResult> Create(CreateMaintenanceOrderRequest request, CancellationToken ct) =>
        CreatedResource(await maintenance.CreateAsync(request, ct));

    [HttpPost("{id:guid}/start")]
    [Authorize(Policy = Policies.CanWrite)]
    public async Task<IActionResult> Start(Guid id, CancellationToken ct)
    {
        await maintenance.StartAsync(id, ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/close")]
    [Authorize(Policy = Policies.CanWrite)]
    public async Task<IActionResult> Close(Guid id, CloseMaintenanceOrderRequest request, CancellationToken ct)
    {
        await maintenance.CloseAsync(id, request, ct);
        return NoContent();
    }
}
