using FleetErp.Web.Services;
using FleetErp.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FleetErp.Web.Controllers;

/// <summary>
/// Gastos y ganancias: combustible, gastos de ruta, nómina de operadores y
/// utilidad del periodo, con el desglose por concepto, unidad y cliente.
/// </summary>
public sealed class FinanceController(IFleetApiClient api, ISessionContext session) : PortalController(session)
{
    public async Task<IActionResult> Index(string? period, CancellationToken ct)
    {
        var selection = PeriodSelection.Resolve(period);
        var (from, to) = selection.ToRange();

        var report = await api.GetFinanceReportAsync(from, to, ct);
        var expenses = await api.SearchExpensesAsync(null, null, from, to, 1, 15, ct);

        return View(new FinanceViewModel
        {
            Report = report,
            Period = selection,
            RecentExpenses = expenses
        });
    }
}
