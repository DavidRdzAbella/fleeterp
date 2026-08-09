using FleetErp.Application.Contracts;
using FleetErp.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FleetErp.Api.Controllers;

[AllowAnonymous]
public sealed class AuthController(IAuthService auth) : ApiControllerBase
{
    /// <summary>Autentica al usuario dentro de una empresa y devuelve el token de sesión.</summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request, CancellationToken ct) =>
        Ok(await auth.LoginAsync(request, ct));
}
