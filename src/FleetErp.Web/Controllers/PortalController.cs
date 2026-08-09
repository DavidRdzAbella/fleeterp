using FleetErp.Web.Services;
using FleetErp.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FleetErp.Web.Controllers;

/// <summary>
/// Base de las pantallas del portal: exige sesión, publica la parametrización de
/// la empresa a las vistas y convierte los errores de la API en mensajes que el
/// usuario pueda entender y accionar.
/// </summary>
[Authorize]
public abstract class PortalController(ISessionContext session) : Controller
{
    protected ISessionContext Session { get; } = session;

    public override void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context)
    {
        ViewData["Settings"] = Session.Settings;
        ViewData["TenantName"] = Session.TenantName;
        ViewData["UserName"] = Session.UserName;
        ViewData["CanWrite"] = Session.CanWrite;
        ViewData["IsAdministrator"] = Session.IsAdministrator;
        base.OnActionExecuting(context);
    }

    /// <summary>
    /// Ejecuta una operación contra la API dejando los errores de negocio en el
    /// <c>ModelState</c>, que es donde la vista ya sabe pintarlos.
    /// </summary>
    protected async Task<bool> TryAsync(Func<Task> operation)
    {
        try
        {
            await operation();
            return true;
        }
        catch (ApiException ex)
        {
            foreach (var (field, messages) in ex.FieldErrors)
                foreach (var message in messages)
                    ModelState.AddModelError(NormalizeField(field), message);

            if (ex.FieldErrors.Count == 0) ModelState.AddModelError(string.Empty, ex.UserMessage);
            return false;
        }
        catch (HttpRequestException)
        {
            ModelState.AddModelError(string.Empty,
                "No hay conexión con el servicio de flotilla. Verifique que la API esté disponible y vuelva a intentar.");
            return false;
        }
    }

    /// <summary>Deja un aviso para la siguiente pantalla tras una acción exitosa.</summary>
    protected void Notify(string message) => TempData["Notice"] = message;

    protected void Warn(string message) => TempData["Warning"] = message;

    protected string FirstError() =>
        ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).FirstOrDefault()
        ?? "No fue posible completar la operación.";

    /// <summary>
    /// Traduce los parámetros de la dirección al modo del panel de la derecha.
    /// El estado de la pantalla vive en la URL, así que es compartible y el botón
    /// de regresar del navegador funciona como el usuario espera.
    /// </summary>
    /// <remarks>
    /// <c>mode=new</c> manda sobre el identificador, y no al revés. La ruta por
    /// omisión es <c>{controller}/{action}/{id?}</c>, así que al generar enlaces
    /// desde una ficha abierta los tag helpers reutilizan el <c>id</c> del
    /// contexto; si el identificador tuviera prioridad, el botón de alta abriría
    /// el registro seleccionado en lugar de un formulario en blanco.
    /// </remarks>
    protected WorkbenchMode ResolveMode(Guid? id, string? mode)
    {
        var requested = mode?.Trim().ToLowerInvariant();

        if (!Session.CanWrite) return id is null ? WorkbenchMode.Empty : WorkbenchMode.View;

        return requested switch
        {
            "new" => WorkbenchMode.New,
            "edit" when id is not null => WorkbenchMode.Edit,
            _ => id is null ? WorkbenchMode.Empty : WorkbenchMode.View
        };
    }

    /// <summary>Filtros de la pantalla que deben viajar en todos los enlaces del módulo.</summary>
    protected static Dictionary<string, string?> Filters(params (string Key, string? Value)[] values) =>
        values.Where(v => !string.IsNullOrWhiteSpace(v.Value))
              .ToDictionary(v => v.Key, v => v.Value);

    /// <summary>
    /// La API nombra los campos como el DTO ("request.Origin"); el formulario los
    /// nombra sin prefijo. Sin esta traducción el error no se pinta junto al control.
    /// </summary>
    private static string NormalizeField(string field)
    {
        var lastDot = field.LastIndexOf('.');
        return lastDot >= 0 && lastDot < field.Length - 1 ? field[(lastDot + 1)..] : field;
    }
}
