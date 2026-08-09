using FleetErp.Application.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FleetErp.Api.Controllers;

/// <summary>
/// Base de todos los controladores: ruta, autenticación por defecto y utilidades
/// compartidas. Los endpoints públicos deben marcarse explícitamente con
/// <see cref="AllowAnonymousAttribute"/>, nunca al revés.
/// </summary>
[ApiController]
[Authorize]
[Route("api/[controller]")]
[Produces("application/json")]
public abstract class ApiControllerBase : ControllerBase
{
    protected static PageQuery Paging(int page, int pageSize) => new() { Page = page, PageSize = pageSize };

    /// <summary>201 con la ubicación del recurso recién creado.</summary>
    protected IActionResult CreatedResource(Guid id) =>
        Created($"{Request.Path}/{id}", new { id });
}

/// <summary>Roles con permiso de escritura. La consulta queda abierta a cualquier usuario autenticado.</summary>
public static class Policies
{
    public const string CanWrite = nameof(CanWrite);
    public const string IsAdministrator = nameof(IsAdministrator);
}
