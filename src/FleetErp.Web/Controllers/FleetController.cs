using FleetErp.Web.Models;
using FleetErp.Web.Services;
using FleetErp.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FleetErp.Web.Controllers;

/// <summary>
/// Inventario de unidades. Tractocamiones y cajas se administran en la misma
/// pantalla: lo que las distingue es su tipo, que cada empresa define.
/// </summary>
public sealed class FleetController(IFleetApiClient api, ISessionContext session) : PortalController(session)
{
    private const int ListSize = 200;

    public async Task<IActionResult> Index(string? search, Guid? id, string? mode, CancellationToken ct)
    {
        return View(await BuildAsync(search, id, mode, form: null, ct));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(VehicleFormViewModel form, string? search, CancellationToken ct)
    {
        if (!Session.CanWrite) return Forbid();

        if (ModelState.IsValid)
        {
            var id = form.Id;
            var saved = form.Id is null
                ? await TryAsync(async () => id = await api.CreateVehicleAsync(ToCreate(form), ct))
                : await TryAsync(() => api.UpdateVehicleAsync(form.Id.Value, ToUpdate(form), ct));

            if (saved)
            {
                Notify(form.IsNew ? $"Unidad {form.EconomicNumber} dada de alta." : "Unidad actualizada.");
                return RedirectToAction(nameof(Index), new { search, id });
            }
        }

        // Se vuelve al mismo modo con lo que el usuario ya había capturado.
        return View(nameof(Index), await BuildAsync(search, form.Id, form.IsNew ? "new" : "edit", form, ct));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, string? search, CancellationToken ct)
    {
        if (!Session.CanWrite) return Forbid();

        var vehicle = await api.GetVehicleAsync(id, ct);
        var reactivating = vehicle is { IsActive: false };

        var ok = await TryAsync(() => api.SetVehicleActiveAsync(id, reactivating, ct));
        if (ok) Notify(reactivating ? "Unidad reactivada." : "Unidad dada de baja.");
        else Warn(FirstError());

        return RedirectToAction(nameof(Index), new { search, id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeStatus(Guid id, VehicleStatus status, string? search, CancellationToken ct)
    {
        if (!Session.CanWrite) return Forbid();

        var ok = await TryAsync(() => api.ChangeVehicleStatusAsync(id, status, ct));
        if (ok) Notify($"La unidad quedó como {Display.VehicleStatusLabel(status).ToLowerInvariant()}.");
        else Warn(FirstError());

        return RedirectToAction(nameof(Index), new { search, id });
    }

    // ---- Armado de la pantalla -------------------------------------------

    private async Task<FleetWorkbench> BuildAsync(
        string? search, Guid? id, string? mode, VehicleFormViewModel? form, CancellationToken ct)
    {
        var page = await api.SearchVehiclesAsync(search, null, null, 1, ListSize, ct);
        var resolved = ResolveMode(id, mode);

        // En alta no debe quedar nada seleccionado: el identificador que arrastra
        // la ruta abriria la ficha existente y guardar acabaria actualizandola.
        if (resolved == WorkbenchMode.New) id = null;

        var selected = id is null ? null : await api.GetVehicleAsync(id.Value, ct);
        if (selected is null && resolved == WorkbenchMode.View) resolved = WorkbenchMode.Empty;

        var editable = form ?? (selected is null ? new VehicleFormViewModel() : ToForm(selected));
        editable.TypeOptions = await api.GetVehicleTypesAsync(false, ct);
        editable.CustomFieldDefinitions = await api.GetCustomFieldsAsync(CustomFieldTarget.Vehicle, ct);

        var orders = selected is null
            ? []
            : (await api.SearchMaintenanceAsync(selected.Id, null, 1, 5, ct)).Items;

        return new FleetWorkbench
        {
            Mode = resolved,
            CanWrite = Session.CanWrite,
            Selected = selected,
            Form = editable,
            RecentOrders = orders,
            List = new WorkbenchList
            {
                SearchPlaceholder = "Económico, placa o marca",
                Search = search,
                SelectedId = id,
                TotalCount = page.TotalCount,
                EmptyMessage = "No hay unidades que coincidan.",
                Filters = Filters(("search", search)),
                Items = page.Items.Select(v => new WorkbenchItem(
                    v.Id,
                    v.EconomicNumber,
                    v.VehicleTypeName,
                    v.PlateNumber,
                    Display.VehicleStatusLabel(v.Status),
                    Display.VehicleStatusTone(v.Status),
                    !v.IsActive)).ToList()
            }
        };
    }

    private static VehicleFormViewModel ToForm(VehicleModel v) => new()
    {
        Id = v.Id,
        EconomicNumber = v.EconomicNumber,
        PlateNumber = v.PlateNumber,
        VehicleTypeId = v.VehicleTypeId,
        Brand = v.Brand,
        Model = v.Model,
        Year = v.Year,
        Vin = v.Vin,
        CargoCapacity = v.CargoCapacity,
        TankCapacity = v.TankCapacity,
        InitialOdometer = v.CurrentOdometer,
        InsuranceExpiry = v.InsuranceExpiry?.ToDateTime(TimeOnly.MinValue),
        CirculationCardExpiry = v.CirculationCardExpiry?.ToDateTime(TimeOnly.MinValue),
        CustomFields = new Dictionary<string, string?>(v.CustomFields)
    };

    private static CreateVehicleRequest ToCreate(VehicleFormViewModel f) => new(
        f.EconomicNumber, f.PlateNumber, f.VehicleTypeId,
        f.Brand, f.Model, f.Year, f.Vin,
        f.CargoCapacity, f.TankCapacity, f.InitialOdometer,
        ToDateOnly(f.InsuranceExpiry), ToDateOnly(f.CirculationCardExpiry), Clean(f.CustomFields));

    private static UpdateVehicleRequest ToUpdate(VehicleFormViewModel f) => new(
        f.EconomicNumber, f.PlateNumber, f.VehicleTypeId,
        f.Brand, f.Model, f.Year, f.Vin,
        f.CargoCapacity, f.TankCapacity,
        ToDateOnly(f.InsuranceExpiry), ToDateOnly(f.CirculationCardExpiry), Clean(f.CustomFields));

    private static DateOnly? ToDateOnly(DateTime? value) => value is null ? null : DateOnly.FromDateTime(value.Value);

    private static Dictionary<string, string?> Clean(Dictionary<string, string?> fields) =>
        fields.Where(kv => !string.IsNullOrWhiteSpace(kv.Value)).ToDictionary(kv => kv.Key, kv => kv.Value);
}
