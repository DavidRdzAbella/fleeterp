using FleetErp.Web.Models;
using FleetErp.Web.Services;
using FleetErp.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FleetErp.Web.Controllers;

/// <summary>
/// Órdenes de taller. Abrir una manda la unidad a mantenimiento y cerrarla la
/// devuelve a disponible, de modo que el tablero de flotilla siempre refleja lo
/// que de verdad puede salir a ruta.
/// </summary>
public sealed class MaintenanceController(IFleetApiClient api, ISessionContext session) : PortalController(session)
{
    private const int ListSize = 200;

    public async Task<IActionResult> Index(string? search, Guid? id, string? mode, MaintenanceStatus? status, CancellationToken ct) =>
        View(await BuildAsync(search, id, mode, status, form: null, ct));

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(MaintenanceFormViewModel form, string? search, CancellationToken ct)
    {
        if (!Session.CanWrite) return Forbid();

        // La API solo permite abrir órdenes nuevas: una orden en curso se cierra,
        // no se reescribe, porque su folio ya es evidencia del gasto.
        if (!form.IsNew)
        {
            Warn("Una orden abierta se cierra desde su ficha; sus datos no se reescriben.");
            return RedirectToAction(nameof(Index), new { search, id = form.Id });
        }

        if (ModelState.IsValid)
        {
            var id = Guid.Empty;
            var ok = await TryAsync(async () => id = await api.CreateMaintenanceAsync(
                new CreateMaintenanceOrderRequest(form.VehicleId, form.Kind, ToUtc(form.OpenedAt), form.Description), ct));

            if (ok)
            {
                Notify("Orden abierta. La unidad quedó en taller.");
                return RedirectToAction(nameof(Index), new { search, id });
            }
        }

        return View(nameof(Index), await BuildAsync(search, null, "new", null, form, ct));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Start(Guid id, string? search, CancellationToken ct)
    {
        if (!Session.CanWrite) return Forbid();

        var ok = await TryAsync(() => api.StartMaintenanceAsync(id, ct));
        if (ok) Notify("Orden marcada como en proceso.");
        else Warn(FirstError());

        return RedirectToAction(nameof(Index), new { search, id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Close(Guid id, CloseMaintenanceFormViewModel form, string? search, CancellationToken ct)
    {
        if (!Session.CanWrite) return Forbid();

        var ok = await TryAsync(() => api.CloseMaintenanceAsync(id, new CloseMaintenanceOrderRequest(
            ToUtc(form.ClosedAt), form.Cost, form.Workshop, form.OdometerAtService), ct));

        if (ok) Notify("Orden cerrada. Si no quedan órdenes abiertas, la unidad vuelve a estar disponible.");
        else Warn(FirstError());

        return RedirectToAction(nameof(Index), new { search, id });
    }

    private async Task<MaintenanceWorkbench> BuildAsync(
        string? search, Guid? id, string? mode, MaintenanceStatus? status,
        MaintenanceFormViewModel? form, CancellationToken ct)
    {
        var page = await api.SearchMaintenanceAsync(null, status, 1, ListSize, ct);
        var resolved = ResolveMode(id, mode);

        // En alta no debe quedar nada seleccionado: el identificador que arrastra
        // la ruta abriria la ficha existente y guardar acabaria actualizandola.
        if (resolved == WorkbenchMode.New) id = null;

        var selected = id is null ? null : await api.GetMaintenanceAsync(id.Value, ct);
        if (selected is null && resolved == WorkbenchMode.View) resolved = WorkbenchMode.Empty;

        // La ficha de una orden no se edita: solo se abre o se cierra.
        if (resolved == WorkbenchMode.Edit) resolved = WorkbenchMode.View;

        var editable = form ?? new MaintenanceFormViewModel();
        editable.VehicleOptions = await api.GetVehicleLookupAsync(null, ct);

        // El buscador filtra sobre lo ya traído: son pocas órdenes y así se evita
        // un endpoint de búsqueda que hoy nadie más necesitaría.
        var items = page.Items.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            items = items.Where(o =>
                o.Folio.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                o.VehicleLabel.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                o.Description.Contains(term, StringComparison.OrdinalIgnoreCase));
        }

        var filtered = items.ToList();
        var settings = Session.Settings;

        return new MaintenanceWorkbench
        {
            Mode = resolved,
            CanWrite = Session.CanWrite,
            Selected = selected,
            Form = editable,
            Close = new CloseMaintenanceFormViewModel
            {
                ClosedAt = DateTime.Now,
                OdometerAtService = null
            },
            List = new WorkbenchList
            {
                SearchPlaceholder = "Folio, unidad o descripción",
                Search = search,
                SelectedId = id,
                TotalCount = filtered.Count,
                EmptyMessage = "No hay órdenes que coincidan.",
                Filters = Filters(("search", search), ("status", status?.ToString())),
                Items = filtered.Select(o => new WorkbenchItem(
                    o.Id,
                    o.Folio,
                    $"{o.VehicleLabel} · {Display.MaintenanceKindLabel(o.Kind)}",
                    Display.DateLabel(o.OpenedAtUtc, settings),
                    Display.MaintenanceStatusLabel(o.Status),
                    o.Status switch
                    {
                        MaintenanceStatus.Open => "late",
                        MaintenanceStatus.InProgress => "active",
                        _ => "done"
                    },
                    o.Status == MaintenanceStatus.Closed)).ToList()
            }
        };
    }

    private static DateTimeOffset ToUtc(DateTime local) =>
        new DateTimeOffset(DateTime.SpecifyKind(local, DateTimeKind.Local)).ToUniversalTime();
}
