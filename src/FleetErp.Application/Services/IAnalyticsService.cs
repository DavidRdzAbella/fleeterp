using FleetErp.Application.Contracts;

namespace FleetErp.Application.Services;

/// <summary>Tableros y reportes. Es la parte del ERP que contesta "cómo va la flotilla".</summary>
public interface IAnalyticsService
{
    Task<FleetDashboardDto> GetFleetDashboardAsync(AnalyticsPeriod? period, CancellationToken ct = default);

    /// <summary>Ranking de conductores por el criterio indicado (top 1, 2, 3…).</summary>
    Task<IReadOnlyList<DriverRankingRowDto>> GetDriverRankingAsync(
        AnalyticsPeriod? period, DriverRankingCriteria criteria, int take, CancellationToken ct = default);

    Task<DriverPerformanceDto> GetDriverPerformanceAsync(Guid driverId, AnalyticsPeriod? period, CancellationToken ct = default);

    Task<FinanceReportDto> GetFinanceReportAsync(AnalyticsPeriod? period, CancellationToken ct = default);
}

/// <summary>Criterio de ordenamiento del ranking; el tablero deja al usuario cambiarlo.</summary>
public enum DriverRankingCriteria
{
    Distance = 0,
    Revenue = 1,
    Profit = 2,
    Trips = 3,
    FuelEfficiency = 4
}
