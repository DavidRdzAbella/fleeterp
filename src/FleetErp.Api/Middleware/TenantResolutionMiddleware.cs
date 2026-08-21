using FleetErp.Infrastructure.Identity;
using FleetErp.Infrastructure.Services;

namespace FleetErp.Api.Middleware;

/// <summary>
/// Fija la empresa de la petición a partir del token ya validado. Se ejecuta
/// después de la autenticación y antes de los controladores, de modo que cuando
/// cualquier consulta llega al contexto de datos el filtro ya está armado.
/// </summary>
/// <remarks>
/// La empresa se toma exclusivamente del claim firmado. Nunca de una cabecera
/// que el cliente pueda escribir: eso permitiría leer datos de otra empresa
/// simplemente cambiando un valor en la petición.
/// </remarks>
public async Task InvokeAsync(HttpContext context, CurrentTenant currentTenant)
{
    // Si la ruta es de autenticación o pública, salta el middleware y continúa
    var path = context.Request.Path.Value;
    if (path != null && path.Contains("/api/auth", StringComparison.OrdinalIgnoreCase))
    {
        await next(context);
        return;
    }

    var tenantId = context.User.FindFirst(JwtTokenGenerator.TenantIdClaim)?.Value;
    var slug = context.User.FindFirst(JwtTokenGenerator.TenantSlugClaim)?.Value;

    if (Guid.TryParse(tenantId, out var parsed))
    {
        currentTenant.Set(parsed, slug ?? string.Empty);
    }

    await next(context);
}
