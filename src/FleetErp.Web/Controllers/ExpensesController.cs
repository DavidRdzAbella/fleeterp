using FleetErp.Web.Models;
using FleetErp.Web.Services;
using FleetErp.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FleetErp.Web.Controllers;

/// <summary>
/// Gastos distintos al combustible: casetas, viáticos, maniobras, refacciones y
/// lo que cada empresa haya dado de alta en su catálogo de conceptos.
/// </summary>
public sealed class ExpensesController(IFleetApiClient api, ISessionContext session) : PortalController(session)
{
    private const int ListSize = 150;

    public async Task<IActionResult> Index(string? search, Guid? id, string? mode, Guid? categoryId, CancellationToken ct) =>
        View(await BuildAsync(search, id, mode, categoryId, form: null, ct));

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(ExpenseFormViewModel form, string? search, CancellationToken ct)
    {
        if (!Session.CanWrite) return Forbid();

        if (ModelState.IsValid)
        {
            var id = form.Id;
            var saved = form.Id is null
                ? await TryAsync(async () => id = await api.CreateExpenseAsync(new CreateExpenseRequest(
                    form.CategoryId, Or(form.TripId), Or(form.VehicleId), Or(form.DriverId),
                    ToUtc(form.IncurredAt), form.Amount, form.Description, form.ReferenceNumber), ct))
                : await TryAsync(() => api.UpdateExpenseAsync(form.Id.Value, new UpdateExpenseRequest(
                    form.CategoryId, Or(form.TripId), Or(form.VehicleId), Or(form.DriverId),
                    ToUtc(form.IncurredAt), form.Amount, form.Description, form.ReferenceNumber), ct));

            if (saved)
            {
                Notify(form.IsNew ? "Gasto registrado." : "Gasto corregido.");
                return RedirectToAction(nameof(Index), new { search, id });
            }
        }

        return View(nameof(Index), await BuildAsync(search, form.Id, form.IsNew ? "new" : "edit", null, form, ct));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, string? search, CancellationToken ct)
    {
        if (!Session.CanWrite) return Forbid();

        var ok = await TryAsync(() => api.DeleteExpenseAsync(id, ct));
        if (ok) Notify("Gasto eliminado.");
        else Warn(FirstError());

        return RedirectToAction(nameof(Index), new { search });
    }

    private async Task<ExpensesWorkbench> BuildAsync(
        string? search, Guid? id, string? mode, Guid? categoryId, ExpenseFormViewModel? form, CancellationToken ct)
    {
        var page = await api.SearchExpensesAsync(categoryId, null, null, null, 1, ListSize, ct);
        var resolved = ResolveMode(id, mode);

        // En alta no debe quedar nada seleccionado: el identificador que arrastra
        // la ruta abriria la ficha existente y guardar acabaria actualizandola.
        if (resolved == WorkbenchMode.New) id = null;

        var selected = id is null ? null : await api.GetExpenseAsync(id.Value, ct);
        if (selected is null && resolved == WorkbenchMode.View) resolved = WorkbenchMode.Empty;

        var settings = Session.Settings;
        var editable = form ?? (selected is null ? new ExpenseFormViewModel { IncurredAt = DateTime.Now } : ToForm(selected));
        editable.CategoryOptions = await api.GetExpenseCategoriesAsync(false, ct);
        editable.VehicleOptions = await api.GetVehicleLookupAsync(null, ct);
        editable.DriverOptions = await api.GetDriverLookupAsync(ct);
        editable.TripOptions = (await api.SearchTripsAsync(null, null, null, null, 1, 40, ct))
            .Items.Select(t => new LookupItemModel(t.Id, t.Folio, $"{t.Origin} a {t.Destination}")).ToList();

        var items = page.Items.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            items = items.Where(e =>
                e.Description.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                e.CategoryName.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                (e.TripFolio ?? string.Empty).Contains(term, StringComparison.OrdinalIgnoreCase) ||
                (e.VehicleLabel ?? string.Empty).Contains(term, StringComparison.OrdinalIgnoreCase));
        }

        var filtered = items.ToList();

        return new ExpensesWorkbench
        {
            Mode = resolved,
            CanWrite = Session.CanWrite,
            Selected = selected,
            Form = editable,
            List = new WorkbenchList
            {
                SearchPlaceholder = "Concepto, descripción, folio o unidad",
                Search = search,
                SelectedId = id,
                TotalCount = filtered.Count,
                EmptyMessage = "No hay gastos que coincidan.",
                Filters = Filters(("search", search), ("categoryId", categoryId?.ToString())),
                Items = filtered.Select(e => new WorkbenchItem(
                    e.Id,
                    e.Description,
                    $"{Display.DateLabel(e.IncurredAtUtc, settings)} · {e.CategoryName}",
                    Display.Money(e.Amount, settings),
                    e.TripFolio,
                    "planned")).ToList()
            }
        };
    }

    private static ExpenseFormViewModel ToForm(ExpenseModel e) => new()
    {
        Id = e.Id,
        CategoryId = e.CategoryId,
        TripId = e.TripId,
        VehicleId = e.VehicleId,
        DriverId = e.DriverId,
        IncurredAt = e.IncurredAtUtc.ToLocalTime().DateTime,
        Amount = e.Amount,
        Description = e.Description,
        ReferenceNumber = e.ReferenceNumber
    };

    private static Guid? Or(Guid? value) => value == Guid.Empty ? null : value;

    private static DateTimeOffset ToUtc(DateTime local) =>
        new DateTimeOffset(DateTime.SpecifyKind(local, DateTimeKind.Local)).ToUniversalTime();
}
