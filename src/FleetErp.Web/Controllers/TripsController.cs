using FleetErp.Web.Models;
using FleetErp.Web.Services;
using FleetErp.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FleetErp.Web.Controllers;

/// <summary>
/// Viajes: la pantalla donde se programa, se despacha y se cierra. Es el centro
/// operativo del sistema y de donde salen todos los números de los tableros.
/// </summary>
public sealed class TripsController(IFleetApiClient api, ISessionContext session) : PortalController(session)
{
    public async Task<IActionResult> Index(string? search, TripStatus? status, Guid? driverId, int page = 1, CancellationToken ct = default)
    {
        var trips = await api.SearchTripsAsync(search, status, driverId, null, page, 20, ct);
        var drivers = await api.GetDriverLookupAsync(ct);

        return View(new TripsIndexViewModel
        {
            Trips = trips,
            Search = search,
            Status = status,
            DriverId = driverId,
            Drivers = drivers
        });
    }

    public async Task<IActionResult> Details(Guid id, CancellationToken ct)
    {
        var trip = await api.GetTripAsync(id, ct);
        if (trip is null) return NotFound();

        return View(await BuildDetailAsync(trip, ct));
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken ct)
    {
        if (!Session.CanWrite) return Forbid();

        var settings = Session.Settings;
        var form = new TripFormViewModel
        {
            DriverPayScheme = settings.DefaultDriverPayScheme,
            DriverPayRate = settings.DefaultDriverPayRate,
            CargoWeightUnit = settings.WeightUnit
        };

        return View("Form", await FillOptionsAsync(form, ct));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TripFormViewModel form, CancellationToken ct)
    {
        if (!Session.CanWrite) return Forbid();

        if (ModelState.IsValid)
        {
            var created = Guid.Empty;
            var ok = await TryAsync(async () => created = await api.CreateTripAsync(ToCreateRequest(form), ct));
            if (ok)
            {
                Notify("Viaje programado. Ya puede despacharlo cuando la unidad salga.");
                return RedirectToAction(nameof(Details), new { id = created });
            }
        }

        return View("Form", await FillOptionsAsync(form, ct));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id, CancellationToken ct)
    {
        if (!Session.CanWrite) return Forbid();

        var trip = await api.GetTripAsync(id, ct);
        if (trip is null) return NotFound();

        return View("Form", await FillOptionsAsync(ToForm(trip), ct));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, TripFormViewModel form, CancellationToken ct)
    {
        if (!Session.CanWrite) return Forbid();

        if (ModelState.IsValid)
        {
            var ok = await TryAsync(() => api.UpdateTripAsync(id, ToUpdateRequest(form), ct));
            if (ok)
            {
                Notify("Viaje actualizado.");
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        form.Id = id;
        return View("Form", await FillOptionsAsync(form, ct));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Dispatch(Guid id, DispatchForm form, CancellationToken ct)
    {
        if (!Session.CanWrite) return Forbid();

        var ok = await TryAsync(() => api.DispatchTripAsync(id,
            new DispatchTripRequest(ToUtc(form.DepartureAt), form.OdometerStart, form.InitialFuel), ct));

        if (ok) Notify("Salida registrada. La unidad y el operador quedaron ocupados.");
        else Warn(FirstError());

        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Complete(Guid id, CompleteForm form, CancellationToken ct)
    {
        if (!Session.CanWrite) return Forbid();

        var ok = await TryAsync(() => api.CompleteTripAsync(id,
            new CompleteTripRequest(ToUtc(form.ArrivalAt), form.OdometerEnd, form.FinalFuel, form.DriverHours), ct));

        if (ok) Notify("Llegada registrada. La unidad y el operador quedaron libres.");
        else Warn(FirstError());

        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(Guid id, string reason, CancellationToken ct)
    {
        if (!Session.CanWrite) return Forbid();

        var ok = await TryAsync(() => api.CancelTripAsync(id, new CancelTripRequest(reason ?? string.Empty), ct));

        if (ok) Notify("Viaje cancelado.");
        else Warn(FirstError());

        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddExpense(QuickExpenseForm form, CancellationToken ct)
    {
        if (!Session.CanWrite) return Forbid();

        var ok = await TryAsync(() => api.CreateExpenseAsync(new CreateExpenseRequest(
            form.CategoryId, form.TripId, null, null, ToUtc(form.IncurredAt),
            form.Amount, form.Description, null), ct));

        if (ok) Notify("Gasto registrado en el viaje.");
        else Warn(FirstError());

        return RedirectToAction(nameof(Details), new { id = form.TripId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddFuel(QuickFuelForm form, CancellationToken ct)
    {
        if (!Session.CanWrite) return Forbid();

        var ok = await TryAsync(() => api.CreateFuelLogAsync(new CreateFuelLogRequest(
            form.VehicleId, form.TripId, null, ToUtc(form.LoadedAt),
            form.Quantity, form.PricePerUnit, form.OdometerReading, form.Station, null), ct));

        if (ok) Notify("Carga de combustible registrada.");
        else Warn(FirstError());

        return RedirectToAction(nameof(Details), new { id = form.TripId });
    }

    // ---- Armado de la pantalla -------------------------------------------

    private async Task<TripDetailViewModel> BuildDetailAsync(TripDetailModel trip, CancellationToken ct)
    {
        var categories = await api.GetExpenseCategoriesAsync(false, ct);
        var vehicle = await api.GetVehicleAsync(trip.VehicleId, ct);

        // Se precarga lo que el sistema ya sabe para que el despachador solo
        // confirme: el odómetro de salida es el que trae la unidad, y el de
        // llegada arranca desde donde salió.
        return new TripDetailViewModel
        {
            Trip = trip,
            ExpenseCategories = categories,
            Dispatch = new DispatchForm
            {
                DepartureAt = DateTime.Now,
                OdometerStart = trip.OdometerStart ?? vehicle?.CurrentOdometer ?? 0m,
                InitialFuel = trip.InitialFuel
            },
            Complete = new CompleteForm
            {
                ArrivalAt = DateTime.Now,
                OdometerEnd = trip.OdometerStart ?? 0m
            }
        };
    }

    private async Task<TripFormViewModel> FillOptionsAsync(TripFormViewModel form, CancellationToken ct)
    {
        form.DriverOptions = await api.GetDriverLookupAsync(ct);
        form.VehicleOptions = await api.GetVehicleLookupAsync(VehicleCategory.Motorized, ct);
        form.TrailerOptions = await api.GetVehicleLookupAsync(VehicleCategory.Towed, ct);
        form.CustomerOptions = await api.GetCustomerLookupAsync(ct);
        form.CustomFieldDefinitions = await api.GetCustomFieldsAsync(CustomFieldTarget.Trip, ct);
        return form;
    }

    private static TripFormViewModel ToForm(TripDetailModel trip) => new()
    {
        Id = trip.Id,
        Folio = trip.Folio,
        Status = trip.Status,
        DriverId = trip.DriverId,
        VehicleId = trip.VehicleId,
        TrailerId = trip.TrailerId,
        CustomerId = trip.CustomerId,
        Origin = trip.Origin,
        Destination = trip.Destination,
        PlannedDistance = trip.PlannedDistance,
        ScheduledDeparture = trip.ScheduledDepartureUtc.ToLocalTime().DateTime,
        ScheduledArrival = trip.ScheduledArrivalUtc?.ToLocalTime().DateTime,
        InitialFuel = trip.InitialFuel,
        RefuelPlanned = trip.RefuelPlanned,
        CargoWeight = trip.CargoWeight,
        CargoWeightUnit = trip.CargoWeightUnit,
        CargoDescription = trip.CargoDescription,
        FreightRevenue = trip.FreightRevenue,
        DriverPayScheme = trip.DriverPayScheme,
        DriverPayRate = trip.DriverPayRate,
        Notes = trip.Notes,
        CustomFields = new Dictionary<string, string?>(trip.CustomFields)
    };

    private static CreateTripRequest ToCreateRequest(TripFormViewModel f) => new(
        f.DriverId, f.VehicleId, OrNull(f.TrailerId), OrNull(f.CustomerId),
        f.Origin, f.Destination, f.PlannedDistance,
        ToUtc(f.ScheduledDeparture), f.ScheduledArrival is null ? null : ToUtc(f.ScheduledArrival.Value),
        f.InitialFuel, f.RefuelPlanned,
        f.CargoWeight, f.CargoWeightUnit, f.CargoDescription,
        f.FreightRevenue, f.DriverPayScheme, f.DriverPayRate,
        f.Notes, Clean(f.CustomFields));

    private static UpdateTripRequest ToUpdateRequest(TripFormViewModel f) => new(
        f.DriverId, f.VehicleId, OrNull(f.TrailerId), OrNull(f.CustomerId),
        f.Origin, f.Destination, f.PlannedDistance,
        ToUtc(f.ScheduledDeparture), f.ScheduledArrival is null ? null : ToUtc(f.ScheduledArrival.Value),
        f.InitialFuel, f.RefuelPlanned,
        f.CargoWeight, f.CargoWeightUnit, f.CargoDescription,
        f.FreightRevenue, f.DriverPayScheme, f.DriverPayRate,
        f.Notes, Clean(f.CustomFields));

    /// <summary>Un combo vacío llega como Guid vacío; para la API eso significa "sin asignar".</summary>
    private static Guid? OrNull(Guid? value) => value == Guid.Empty ? null : value;

    private static Dictionary<string, string?> Clean(Dictionary<string, string?> fields) =>
        fields.Where(kv => !string.IsNullOrWhiteSpace(kv.Value)).ToDictionary(kv => kv.Key, kv => kv.Value);

    /// <summary>El usuario captura hora local; la API siempre trabaja en UTC.</summary>
    private static DateTimeOffset ToUtc(DateTime local) =>
        new DateTimeOffset(DateTime.SpecifyKind(local, DateTimeKind.Local)).ToUniversalTime();
}
