using System.Net;

namespace FleetErp.Web.Services;

/// <summary>
/// Error devuelto por la API ya traducido a algo que la vista puede mostrar.
/// Conserva los errores por campo para poder pintarlos junto a cada control.
/// </summary>
public sealed class ApiException(HttpStatusCode statusCode, string title, string? detail,
                                 IDictionary<string, string[]>? fieldErrors = null)
    : Exception(detail is null ? title : $"{title} {detail}")
{
    public HttpStatusCode StatusCode { get; } = statusCode;
    public string Title { get; } = title;
    public string? Detail { get; } = detail;
    public IDictionary<string, string[]> FieldErrors { get; } = fieldErrors ?? new Dictionary<string, string[]>();

    public bool IsUnauthorized => StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden;

    /// <summary>Mensaje listo para el usuario, sin jerga técnica ni códigos.</summary>
    public string UserMessage => string.IsNullOrWhiteSpace(Detail) ? Title : Detail!;
}
