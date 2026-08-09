using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using FleetErp.Application.Abstractions;
using FleetErp.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace FleetErp.Infrastructure.Identity;

/// <summary>Parámetros de firma del token. Se configuran por entorno, nunca en código.</summary>
public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = "FleetErp.Api";
    public string Audience { get; set; } = "FleetErp.Web";
    public string SigningKey { get; set; } = string.Empty;
    public int ExpirationMinutes { get; set; } = 480;

    /// <summary>
    /// Deriva la llave de firma con SHA-256 en lugar de usar los bytes literales
    /// del texto configurado.
    /// </summary>
    /// <remarks>
    /// HMAC-SHA256 exige 256 bits de material de llave. Tomar los bytes crudos
    /// obligaría a que el secreto tuviera 32 caracteres o más, y una frase más
    /// corta haría fallar la emisión del token en tiempo de ejecución. Derivarlo
    /// admite cualquier longitud y produce siempre el mismo resultado, de modo
    /// que quien emite y quien valida coinciden.
    ///
    /// La derivación no crea entropía: un secreto corto o predecible sigue
    /// siendo débil. Para producción conviene una cadena larga y aleatoria
    /// guardada fuera del archivo de configuración.
    /// </remarks>
    public SymmetricSecurityKey CreateSigningKey()
    {
        if (string.IsNullOrWhiteSpace(SigningKey))
            throw new InvalidOperationException("Configure Jwt:SigningKey antes de iniciar la aplicación.");

        return new SymmetricSecurityKey(SHA256.HashData(Encoding.UTF8.GetBytes(SigningKey)));
    }
}

/// <summary>
/// Emite el JWT con el identificador de empresa embebido. Que el tenant viaje en
/// el token es lo que impide que un usuario autenticado consulte datos de otra
/// empresa cambiando una cabecera.
/// </summary>
public sealed class JwtTokenGenerator(IOptions<JwtOptions> options, IClock clock) : ITokenGenerator
{
    public const string TenantIdClaim = "tenant_id";
    public const string TenantSlugClaim = "tenant_slug";

    private readonly JwtOptions _options = options.Value;

    public (string Token, DateTimeOffset ExpiresAtUtc) Generate(AppUser user, Tenant tenant)
    {
        var expiresAt = clock.UtcNow.AddMinutes(_options.ExpirationMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.Name, user.FullName),
            new(ClaimTypes.Role, user.Role.ToString()),
            new(TenantIdClaim, tenant.Id.ToString()),
            new(TenantSlugClaim, tenant.Slug)
        };

        var key = _options.CreateSigningKey();
        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            notBefore: clock.UtcNow.UtcDateTime,
            expires: expiresAt.UtcDateTime,
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }
}
