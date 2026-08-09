using FleetErp.Application.Abstractions;
using FleetErp.Application.Common;
using FleetErp.Application.Contracts;
using FleetErp.Application.Services;
using FleetErp.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FleetErp.Api.Controllers;

/// <summary>
/// Cuentas de acceso de la empresa. Todo el módulo exige perfil de administrador:
/// quien puede crear usuarios puede darse permisos, así que no se abre a despacho.
/// </summary>
[Authorize(Policy = Policies.IsAdministrator)]
public sealed class UsersController(IUserService users, IUserQueries queries) : ApiControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<UserDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<UserDto>>> Search(
        [FromQuery] string? search, [FromQuery] UserRole? role, [FromQuery] bool? isActive,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default) =>
        Ok(await queries.SearchAsync(new UserFilter(search, role, isActive), Paging(page, pageSize), ct));

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> Get(Guid id, CancellationToken ct)
    {
        var user = await queries.GetAsync(id, ct);
        return user is null ? NotFound() : Ok(user);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(CreateUserRequest request, CancellationToken ct) =>
        CreatedResource(await users.CreateAsync(request, ct));

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Update(Guid id, UpdateUserRequest request, CancellationToken ct)
    {
        await users.UpdateAsync(id, request, ct);
        return NoContent();
    }

    /// <summary>Restablece la contraseña. Es operación aparte de la edición del perfil.</summary>
    [HttpPost("{id:guid}/password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ChangePassword(Guid id, ChangePasswordRequest request, CancellationToken ct)
    {
        await users.ChangePasswordAsync(id, request, ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/active")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> SetActive(Guid id, [FromQuery] bool active, CancellationToken ct)
    {
        await users.SetActiveAsync(id, active, ct);
        return NoContent();
    }
}
