using FleetErp.Web.Models;
using FleetErp.Web.Services;
using FleetErp.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FleetErp.Web.Controllers;

/// <summary>
/// Cargas de combustible. Es la fuente única de litros y costo de diésel: de
/// aquí sale el gasto del tablero y el rendimiento por unidad, así que se lleva
/// aparte de los demás gastos.
/// </summary>
public sealed class FuelController(IFleetApiClient api, ISessionContext session) : PortalController(session)
{
    private const int ListSize = 150;

    public async Task<IActionResult> Index(string? search, Guid? id, string? mode, Guid? vehicleId, CancellationToken ct) =>
        View(await BuildAsync(search, id, mode, vehicleId, form: null, ct));

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(FuelFormViewModel form, string? search, CancellationToken ct)
    {
        if (!Session.CanWrite) return Forbid();

        if (ModelState.IsValid)
        {
            var id = form.Id;
            var saved = form.Id is null
                ? await TryAsync(async () => id = await api.CreateFuelLogAsync(new CreateFuelLogRequest(
                    form.VehicleId, Or(form.TripId), Or(form.DriverId), ToUtc(form.LoadedAt),
                    form.Quantity, form.PricePerUnit, form.OdometerReading, form.Station, form.ReferenceNumber), ct))
                : await TryAsync(() => api.UpdateFuelLogAsync(form.Id.Value, new UpdateFuelLogRequest(
                    form.VehicleId, Or(form.TripId), Or(form.DriverId), ToUtc(form.LoadedAt),
                    form.Quantity, form.PricePerUnit, form.OdometerReading, form.Station, form.ReferenceNumber), ct));

            if (saved)
            {
                Notify(form.IsNew ? "Carga registrada." : "Carga corregida.");
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

        var ok = await TryAsync(() => api.DeleteFuelLogAsync(id, ct));
        if (ok) Notify("Carga eliminada.");
        else Warn(FirstError());

        return RedirectToAction(nameof(Index), new { search });
    }

    private async Task<FuelWorkbench> BuildAsync(
        string? search, Guid? id, string? mode, Guid? vehicleId, FuelFormViewModel? form, CancellationToken ct)
    {
        var page = await api.SearchFuelLogsAsync(vehicleId, null, 1, ListSize, ct);
        var resolved = ResolveMode(id, mode);

        // En alta no debe quedar nada seleccionado: el identificador que arrastra
        // la ruta abriria la ficha existente y guardar acabaria actualizandola.
        if (resolved == WorkbenchMode.New) id = null;

        var selected = id is null ? null : await api.GetFuelLogAsync(id.Value, ct);
        if (selected is null && resolved == WorkbenchMode.View) resolved = WorkbenchMode.Empty;

        var settings = Session.Settings;
        var editable = form ?? (selected is null ? NewForm() : ToForm(selected));
        editable.VehicleOptions = await api.GetVehicleLookupAsync(VehicleCategory.Motorized, ct);
        editable.DriverOptions = await api.GetDriverLookupAsync(ct);
        editable.TripOptions = (await api.SearchTripsAsync(null, null, null, null, 1, 40, ct))
            .Items.Select(t => new LookupItemModel(t.Id, t.Folio, $"{t.Origin} a {t.Destination}")).ToList();

        var items = page.Items.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            items = items.Where(f =>
                f.VehicleLabel.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                (f.Station ?? string.Empty).Contains(term, StringComparison.OrdinalIgnoreCase) ||
                (f.TripFolio ?? string.Empty).Contains(term, StringComparison.OrdinalIgnoreCase) ||
                (f.ReferenceNumber ?? string.Empty).Contains(term, StringComparison.OrdinalIgnoreCase));
        }

        var filtered = items.ToList();

        return new FuelWorkbench
        {
            Mode = resolved,
            CanWrite = Session.CanWrite,
            Selected = selected,
            Form = editable,
            List = new WorkbenchList
            {
                SearchPlaceholder = "Unidad, estación, folio o ticket",
                Search = search,
                SelectedId = id,
                TotalCount = filtered.Count,
                EmptyMessage = "No hay cargas que coincidan.",
                Filters = Filters(("search", search), ("vehicleId", vehicleId?.ToString())),
                Items = filtered.Select(f => new WorkbenchItem(
                    f.Id,
                    f.VehicleLabel,
                    $"{Display.DateLabel(f.LoadedAtUtc, settings)} · {f.Station.Or("Sin estación")}",
                    Display.Money(f.TotalCost, settings),
                    Display.Volume(f.Quantity, settings),
                    "planned")).ToList()
            }
        };
    }

    private FuelFormViewModel NewForm() => new()
    {
        LoadedAt = DateTime.Now,
        PricePerUnit = Session.Settings.DefaultFuelPricePerUnit
    };

    private static FuelFormViewModel ToForm(FuelLogModel f) => new()
    {
        Id = f.Id,
        VehicleId = f.VehicleId,
        TripId = f.TripId,
        DriverId = f.DriverId,
        LoadedAt = f.LoadedAtUtc.ToLocalTime().DateTime,
        Quantity = f.Quantity,
        PricePerUnit = f.PricePerUnit,
        OdometerReading = f.OdometerReading,
        Station = f.Station,
        ReferenceNumber = f.ReferenceNumber
    };

    private static Guid? Or(Guid? value) => value == Guid.Empty ? null : value;

    private static DateTimeOffset ToUtc(DateTime local) =>
        new DateTimeOffset(DateTime.SpecifyKind(local, DateTimeKind.Local)).ToUniversalTime();
}
