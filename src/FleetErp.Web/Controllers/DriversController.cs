using FleetErp.Web.Models;
using FleetErp.Web.Services;
using FleetErp.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FleetErp.Web.Controllers;

/// <summary>
/// Padrón de operadores. El alta y la corrección se hacen en la mesa de trabajo;
/// el análisis de desempeño, con sus gráficas, vive en su propia pantalla porque
/// responde a otra pregunta y se consulta con otro ritmo.
/// </summary>
public sealed class DriversController(IFleetApiClient api, ISessionContext session) : PortalController(session)
{
    private const int ListSize = 200;

    public async Task<IActionResult> Index(string? search, Guid? id, string? mode, CancellationToken ct) =>
        View(await BuildAsync(search, id, mode, form: null, ct));

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(DriverFormViewModel form, string? search, CancellationToken ct)
    {
        if (!Session.CanWrite) return Forbid();

        if (ModelState.IsValid)
        {
            var id = form.Id;
            var saved = form.Id is null
                ? await TryAsync(async () => id = await api.CreateDriverAsync(ToRequest(form), ct))
                : await TryAsync(() => api.UpdateDriverAsync(form.Id.Value, ToRequest(form), ct));

            if (saved)
            {
                Notify(form.IsNew ? $"Conductor {form.FirstName} {form.LastName} dado de alta." : "Conductor actualizado.");
                return RedirectToAction(nameof(Index), new { search, id });
            }
        }

        return View(nameof(Index), await BuildAsync(search, form.Id, form.IsNew ? "new" : "edit", form, ct));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, string? search, CancellationToken ct)
    {
        if (!Session.CanWrite) return Forbid();

        var driver = await api.GetDriverAsync(id, ct);
        var reactivating = driver is { IsActive: false };

        var ok = await TryAsync(() => api.SetDriverActiveAsync(id, reactivating, ct));
        if (ok) Notify(reactivating ? "Conductor reactivado." : "Conductor dado de baja.");
        else Warn(FirstError());

        return RedirectToAction(nameof(Index), new { search, id });
    }

    /// <summary>Desempeño del operador en el periodo: kilómetros, combustible, venta y utilidad.</summary>
    public async Task<IActionResult> Details(Guid id, string? period, CancellationToken ct)
    {
        var selection = PeriodSelection.Resolve(period);
        var (from, to) = selection.ToRange();

        var driver = await api.GetDriverAsync(id, ct);
        if (driver is null) return NotFound();

        var performance = await api.GetDriverPerformanceAsync(id, from, to, ct);
        var trips = await api.SearchTripsAsync(null, null, id, null, 1, 10, ct);

        return View(new DriverDetailViewModel
        {
            Driver = driver,
            Performance = performance,
            RecentTrips = trips.Items,
            Period = selection
        });
    }

    /// <summary>Comparativa y ranking de toda la plantilla.</summary>
    public async Task<IActionResult> Ranking(string? period, DriverRankingCriteria criteria = DriverRankingCriteria.Distance,
                                             CancellationToken ct = default)
    {
        var selection = PeriodSelection.Resolve(period);
        var (from, to) = selection.ToRange();

        return View(new DriversIndexViewModel
        {
            Drivers = PagedResult<DriverModel>.Empty(),
            Ranking = await api.GetDriverRankingAsync(criteria, 15, from, to, ct),
            Period = selection,
            Criteria = criteria,
            Search = null
        });
    }

    // ---- Armado de la pantalla -------------------------------------------

    private async Task<DriversWorkbench> BuildAsync(
        string? search, Guid? id, string? mode, DriverFormViewModel? form, CancellationToken ct)
    {
        var page = await api.SearchDriversAsync(search, null, 1, ListSize, ct);
        var resolved = ResolveMode(id, mode);

        // En alta no debe quedar nada seleccionado: el identificador que arrastra
        // la ruta abriria la ficha existente y guardar acabaria actualizandola.
        if (resolved == WorkbenchMode.New) id = null;

        var selected = id is null ? null : await api.GetDriverAsync(id.Value, ct);
        if (selected is null && resolved == WorkbenchMode.View) resolved = WorkbenchMode.Empty;

        var editable = form ?? (selected is null ? NewForm() : ToForm(selected));
        editable.CustomFieldDefinitions = await api.GetCustomFieldsAsync(CustomFieldTarget.Driver, ct);

        // El resumen del periodo va en la ficha para no obligar a cambiar de
        // pantalla solo para saber cómo va el operador.
        DriverPerformanceModel? performance = null;
        if (selected is not null && resolved == WorkbenchMode.View)
        {
            var (from, to) = PeriodSelection.Month.ToRange();
            performance = await api.GetDriverPerformanceAsync(selected.Id, from, to, ct);
        }

        return new DriversWorkbench
        {
            Mode = resolved,
            CanWrite = Session.CanWrite,
            Selected = selected,
            Form = editable,
            Performance = performance,
            List = new WorkbenchList
            {
                SearchPlaceholder = "Nombre, licencia o número de empleado",
                Search = search,
                SelectedId = id,
                TotalCount = page.TotalCount,
                EmptyMessage = "No hay conductores que coincidan.",
                Filters = Filters(("search", search)),
                Items = page.Items.Select(d => new WorkbenchItem(
                    d.Id,
                    d.FullName,
                    d.EmployeeNumber.Or(d.LicenseNumber),
                    Display.PaySchemeLabel(d.PayScheme),
                    d.LicenseExpiringSoon && d.IsActive ? "Licencia por vencer" : Display.DriverStatusLabel(d.Status),
                    d.LicenseExpiringSoon && d.IsActive ? "late" : Display.DriverStatusTone(d.Status),
                    !d.IsActive)).ToList()
            }
        };
    }

    private DriverFormViewModel NewForm() => new()
    {
        PayScheme = Session.Settings.DefaultDriverPayScheme,
        PayRate = Session.Settings.DefaultDriverPayRate
    };

    private static UpsertDriverRequest ToRequest(DriverFormViewModel f) => new(
        f.FirstName, f.LastName, f.EmployeeNumber,
        f.LicenseNumber, f.LicenseType, ToDateOnly(f.LicenseExpiry),
        f.Phone, f.Email, ToDateOnly(f.HireDate),
        f.PayScheme, f.PayRate,
        f.CustomFields.Where(kv => !string.IsNullOrWhiteSpace(kv.Value)).ToDictionary(kv => kv.Key, kv => kv.Value));

    private static DriverFormViewModel ToForm(DriverModel d) => new()
    {
        Id = d.Id,
        FirstName = d.FirstName,
        LastName = d.LastName,
        EmployeeNumber = d.EmployeeNumber,
        LicenseNumber = d.LicenseNumber,
        LicenseType = d.LicenseType,
        LicenseExpiry = d.LicenseExpiry?.ToDateTime(TimeOnly.MinValue),
        Phone = d.Phone,
        Email = d.Email,
        HireDate = d.HireDate?.ToDateTime(TimeOnly.MinValue),
        PayScheme = d.PayScheme,
        PayRate = d.PayRate,
        CustomFields = new Dictionary<string, string?>(d.CustomFields)
    };

    private static DateOnly? ToDateOnly(DateTime? value) => value is null ? null : DateOnly.FromDateTime(value.Value);
}
