using System.Net;
using System.Text.Json;
using FleetErp.Application.Common;
using FleetErp.Domain.Common;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace FleetErp.Api.Middleware;

public sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await WriteProblemAsync(context, ex);
        }
    }

    private async Task WriteProblemAsync(HttpContext context, Exception exception)
    {
        // AQUÍ ESTÁ EL TRUCO: Forzamos a que el detalle sea el mensaje real de C# para verlo en el navegador
        var status = (int)HttpStatusCode.InternalServerError;
        var title = "Error interno en el servidor.";
        var detail = exception.ToString(); // Esto mostrará la traza completa del error exacto

        logger.LogError(exception, "Error no controlado en {Path}", context.Request.Path);

        var problem = new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = detail, // Verás el error exacto aquí
            Instance = context.Request.Path
        };

        context.Response.Clear();
        context.Response.StatusCode = status;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(problem, JsonOptions));
    }
}

public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseFleetExceptionHandling(this IApplicationBuilder app) =>
        app.UseMiddleware<ExceptionHandlingMiddleware>();
}
