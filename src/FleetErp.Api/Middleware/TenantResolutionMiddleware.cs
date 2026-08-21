//using FleetErp.Infrastructure.Identity;
//using FleetErp.Infrastructure.Services;

//namespace FleetErp.Api.Middleware;

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
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using FleetErp.Application.Common; // Ajusta este namespace si tu CurrentTenant/JwtTokenGenerator está en otra capa

namespace FleetErp.Api.Middleware
{
    public sealed class TenantResolutionMiddleware
    {
        private readonly RequestDelegate _next;

        public TenantResolutionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Salta el middleware si es una ruta de autenticación o pública
            var path = context.Request.Path.Value;
            if (path != null && path.Contains("/api/auth", StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }

            var currentTenant = context.RequestServices.GetRequiredService<CurrentTenant>();

            var tenantId = context.User.FindFirst(JwtTokenGenerator.TenantIdClaim)?.Value;
            var slug = context.User.FindFirst(JwtTokenGenerator.TenantSlugClaim)?.Value;

            if (Guid.TryParse(tenantId, out var parsed))
            {
                currentTenant.Set(parsed, slug ?? string.Empty);
            }

            await _next(context);
        }
    }

    public static class TenantResolutionMiddlewareExtensions
    {
        public static IApplicationBuilder UseTenantResolution(this IApplicationBuilder app) =>
            app.UseMiddleware<TenantResolutionMiddleware>();
    }
}
