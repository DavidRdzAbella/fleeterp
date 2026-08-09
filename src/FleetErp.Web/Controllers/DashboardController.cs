using FleetErp.Web.Models;
using FleetErp.Web.Services;
using FleetErp.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FleetErp.Web.Controllers;

/// <summary>
/// Tablero de flotilla: la pantalla de inicio. Responde de un vistazo cómo va la
/// operación — qué unidades están dónde, qué salió y qué llegó hoy, y cuánto
/// dejó el periodo.
/// </summary>
public sealed class DashboardController(IFleetApiClient api, ISessionContext session) : PortalController(session)
{
    private const int FleetStripSize = 60;

    public async Task<IActionResult> Index(string? period, CancellationToken ct)
    {
        var selection = PeriodSelection.Resolve(period);
        var (from, to) = selection.ToRange();

        // Tres llamadas en paralelo: el tablero no debe encadenar esperas.
        var dashboardTask = api.GetFleetDashboardAsync(from, to, ct);
        var fleetTask = api.SearchVehiclesAsync(null, null, null, 1, FleetStripSize, ct);
        var activeTask = api.GetActiveTripsAsync(8, ct);
        var upcomingTask = api.SearchTripsAsync(null, TripStatus.Planned, null, null, 1, 6, ct);

        try
        {
            await Task.WhenAll(dashboardTask, fleetTask, activeTask, upcomingTask);
        }
        catch (Exception ex) when (ex is ApiException or HttpRequestException)
        {
            Warn("No fue posible cargar el tablero completo. Revise la conexión con la API e intente de nuevo.");
            return View(EmptyModel(selection));
        }

        return View(new DashboardViewModel
        {
            Dashboard = dashboardTask.Result,
            Fleet = fleetTask.Result.Items,
            ActiveTrips = activeTask.Result,
            UpcomingTrips = upcomingTask.Result.Items,
            Period = selection
        });
    }

    /// <summary>Estructura vacía para poder renderizar la pantalla aunque la API no responda.</summary>
    private static DashboardViewModel EmptyModel(PeriodSelection selection)
    {
        var (from, to) = selection.ToRange();
        return new DashboardViewModel
        {
            Dashboard = new FleetDashboardModel(
                new AnalyticsPeriodModel(from, to),
                new FleetStatusModel(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0),
                new TripActivityModel(0, 0, 0, 0, 0, 0, 0, 0, 0, 0),
                new FinancialSummaryModel(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0),
                [], [], [], [], [], [], []),
            Fleet = [],
            ActiveTrips = [],
            UpcomingTrips = [],
            Period = selection
        };
    }
}
