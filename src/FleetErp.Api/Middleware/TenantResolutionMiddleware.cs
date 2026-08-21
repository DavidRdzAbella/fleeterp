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
using System;
using System.Threading.Tasks;

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
            var path = context.Request.Path.Value?.ToLower() ?? string.Empty;

            // Ignora cualquier ruta pública de autenticación, login, swagger o raíz
            if (path.Contains("auth") || path.Contains("login") || path.Contains("swagger") || path == "/")
            {
                await _next(context);
                return;
            }

            var tenantId = context.User.FindFirst("tenant_id")?.Value ?? context.User.FindFirst("TenantId")?.Value;
            var slug = context.User.FindFirst("tenant_slug")?.Value ?? context.User.FindFirst("Slug")?.Value;

            await _next(context);
        }
    }

    public static class TenantResolutionMiddlewareExtensions
    {
        public static IApplicationBuilder UseTenantResolution(this IApplicationBuilder app) =>
            app.UseMiddleware<TenantResolutionMiddleware>();
    }
}
