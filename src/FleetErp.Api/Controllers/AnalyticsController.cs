using FleetErp.Application.Contracts;
using FleetErp.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace FleetErp.Api.Controllers;

/// <summary>
/// Tableros. Cada endpoint resuelve una pantalla completa en una sola llamada
/// para que el portal no tenga que orquestar varias peticiones ni recalcular nada.
/// </summary>
[Route("api/analytics")]
public sealed class AnalyticsController(IAnalyticsService analytics) : ApiControllerBase
{
    /// <summary>Estado general de la flotilla: entradas, salidas, costos y utilidad del periodo.</summary>
    [HttpGet("fleet-dashboard")]
    [ProducesResponseType(typeof(FleetDashboardDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<FleetDashboardDto>> FleetDashboard(
        [FromQuery] DateTimeOffset? fromUtc, [FromQuery] DateTimeOffset? toUtc, CancellationToken ct) =>
        Ok(await analytics.GetFleetDashboardAsync(DriversController.BuildPeriod(fromUtc, toUtc), ct));

    /// <summary>Ranking de conductores (top 1, 2, 3…) por el criterio solicitado.</summary>
    [HttpGet("driver-ranking")]
    [ProducesResponseType(typeof(IReadOnlyList<DriverRankingRowDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<DriverRankingRowDto>>> DriverRanking(
        [FromQuery] DriverRankingCriteria criteria = DriverRankingCriteria.Distance,
        [FromQuery] int take = 10,
        [FromQuery] DateTimeOffset? fromUtc = null,
        [FromQuery] DateTimeOffset? toUtc = null,
        CancellationToken ct = default) =>
        Ok(await analytics.GetDriverRankingAsync(DriversController.BuildPeriod(fromUtc, toUtc), criteria, take, ct));

    /// <summary>Gastos y ganancias del periodo, con desglose y nómina de operadores.</summary>
    [HttpGet("finance")]
    [ProducesResponseType(typeof(FinanceReportDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<FinanceReportDto>> Finance(
        [FromQuery] DateTimeOffset? fromUtc, [FromQuery] DateTimeOffset? toUtc, CancellationToken ct) =>
        Ok(await analytics.GetFinanceReportAsync(DriversController.BuildPeriod(fromUtc, toUtc), ct));
}
