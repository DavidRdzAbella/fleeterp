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
public sealed class TenantResolutionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, CurrentTenant currentTenant)
    {
        var tenantId = context.User.FindFirst(JwtTokenGenerator.TenantIdClaim)?.Value;
        var slug = context.User.FindFirst(JwtTokenGenerator.TenantSlugClaim)?.Value;

        if (Guid.TryParse(tenantId, out var parsed))
            currentTenant.Set(parsed, slug ?? string.Empty);

        await next(context);
    }
}

public static class TenantResolutionMiddlewareExtensions
{
    public static IApplicationBuilder UseTenantResolution(this IApplicationBuilder app) =>
        app.UseMiddleware<TenantResolutionMiddleware>();
}
