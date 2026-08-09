using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FleetErp.Application.Abstractions;
using FleetErp.Domain.Enums;

namespace FleetErp.Api.Middleware;

/// <summary>
/// Adapta los claims del token al puerto <see cref="ICurrentUser"/>. Es el único
/// punto del sistema que conoce HTTP; el resto trabaja contra la abstracción.
/// </summary>
public sealed class CurrentUserAccessor(IHttpContextAccessor accessor) : ICurrentUser
{
    private ClaimsPrincipal? Principal => accessor.HttpContext?.User;

    public Guid? UserId =>
        Guid.TryParse(Find(JwtRegisteredClaimNames.Sub) ?? Find(ClaimTypes.NameIdentifier), out var id) ? id : null;

    public string? Email => Find(JwtRegisteredClaimNames.Email) ?? Find(ClaimTypes.Email);

    public UserRole? Role =>
        Enum.TryParse<UserRole>(Find(ClaimTypes.Role), ignoreCase: true, out var role) ? role : null;

    private string? Find(string claimType) => Principal?.FindFirst(claimType)?.Value;
}
