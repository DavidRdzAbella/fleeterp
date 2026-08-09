using System.Net;
using System.Text.Json;
using FleetErp.Application.Common;
using FleetErp.Domain.Common;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace FleetErp.Api.Middleware;

/// <summary>
/// Traduce las excepciones de negocio a respuestas HTTP con <c>ProblemDetails</c>.
/// Gracias a esto los servicios pueden lanzar excepciones expresivas sin conocer
/// códigos de estado, y ninguna regla de negocio termina como un 500 opaco.
/// </summary>
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
        var (status, title, detail, errors) = Describe(exception);

        if (status >= (int)HttpStatusCode.InternalServerError)
            logger.LogError(exception, "Error no controlado en {Path}", context.Request.Path);
        else
            logger.LogInformation("Petición rechazada en {Path}: {Message}", context.Request.Path, exception.Message);

        var problem = new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path
        };

        if (errors is not null) problem.Extensions["errors"] = errors;

        context.Response.Clear();
        context.Response.StatusCode = status;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(problem, JsonOptions));
    }

    private static (int Status, string Title, string Detail, IDictionary<string, string[]>? Errors) Describe(Exception exception) =>
        exception switch
        {
            ValidationException validation => (
                (int)HttpStatusCode.UnprocessableEntity,
                "Los datos capturados no son válidos.",
                "Revise los campos marcados y vuelva a intentar.",
                validation.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())),

            NotFoundException notFound => (
                (int)HttpStatusCode.NotFound, "Registro no encontrado.", notFound.Message, null),

            ConflictException conflict => (
                (int)HttpStatusCode.Conflict, "La operación no se puede completar.", conflict.Message, null),

            DomainException domain => (
                (int)HttpStatusCode.Conflict, "Regla de negocio no cumplida.", domain.Message, null),

            UnauthorizedException unauthorized => (
                (int)HttpStatusCode.Unauthorized, "Acceso denegado.", unauthorized.Message, null),

            _ => (
                (int)HttpStatusCode.InternalServerError,
                "Ocurrió un error inesperado.",
                "El equipo técnico fue notificado. Intente de nuevo en unos momentos.",
                null)
        };
}

public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseFleetExceptionHandling(this IApplicationBuilder app) =>
        app.UseMiddleware<ExceptionHandlingMiddleware>();
}
