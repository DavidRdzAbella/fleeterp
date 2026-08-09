using FleetErp.Application.Abstractions;
using FleetErp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FleetErp.Infrastructure.Services;

/// <summary>
/// Consecutivo de folios por empresa y año, con el prefijo que cada empresa
/// configuró (VJ-2026-000045). Se calcula sobre el máximo existente para que
/// reiniciar la aplicación o restaurar un respaldo no reinicie la numeración.
/// </summary>
/// <remarks>
/// Bajo alta concurrencia dos altas simultáneas podrían pelear por el mismo
/// número; el índice único por (empresa, folio) lo impide a nivel de base y el
/// reintento resuelve. Para volúmenes mayores, la evolución natural es una
/// secuencia de PostgreSQL por empresa detrás de este mismo puerto.
/// </remarks>
public sealed class FolioGenerator(FleetDbContext context, ICurrentTenant tenant, IClock clock) : IFolioGenerator
{
    private const string MaintenancePrefix = "OS";

    public async Task<string> NextTripFolioAsync(CancellationToken ct = default)
    {
        var prefix = await ResolveTripPrefixAsync(ct);
        var year = clock.UtcNow.Year;
        var head = $"{prefix}-{year}-";

        var last = await context.Trips
            .Where(t => t.Folio.StartsWith(head))
            .Select(t => t.Folio)
            .OrderByDescending(f => f)
            .FirstOrDefaultAsync(ct);

        return $"{head}{NextSequence(last, head):D6}";
    }

    public async Task<string> NextMaintenanceFolioAsync(CancellationToken ct = default)
    {
        var year = clock.UtcNow.Year;
        var head = $"{MaintenancePrefix}-{year}-";

        var last = await context.MaintenanceOrders
            .Where(m => m.Folio.StartsWith(head))
            .Select(m => m.Folio)
            .OrderByDescending(f => f)
            .FirstOrDefaultAsync(ct);

        return $"{head}{NextSequence(last, head):D6}";
    }

    private async Task<string> ResolveTripPrefixAsync(CancellationToken ct)
    {
        using var _ = tenant.BypassFilter();
        var company = await context.Tenants.AsNoTracking().FirstOrDefaultAsync(t => t.Id == tenant.TenantId, ct);
        var prefix = company?.Settings.TripFolioPrefix;
        return string.IsNullOrWhiteSpace(prefix) ? "VJ" : prefix.Trim().ToUpperInvariant();
    }

    private static int NextSequence(string? lastFolio, string head)
    {
        if (string.IsNullOrWhiteSpace(lastFolio)) return 1;
        var tail = lastFolio[head.Length..];
        return int.TryParse(tail, out var n) ? n + 1 : 1;
    }
}
