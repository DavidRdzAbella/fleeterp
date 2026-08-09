using FleetErp.Application.Abstractions;

namespace FleetErp.Infrastructure.Services;

/// <summary>
/// Empresa activa durante la petición. Es un objeto con estado de alcance
/// <c>Scoped</c>: la API lo llena al autenticar y el contexto de datos lo lee
/// para filtrar. Nadie más debería escribirlo.
/// </summary>
public sealed class CurrentTenant : ICurrentTenant
{
    private int _bypassDepth;

    public Guid TenantId { get; private set; }
    public string Slug { get; private set; } = string.Empty;
    public bool IsResolved => TenantId != Guid.Empty;
    public bool FilterDisabled => _bypassDepth > 0;

    public void Set(Guid tenantId, string slug)
    {
        TenantId = tenantId;
        Slug = slug ?? string.Empty;
    }

    /// <summary>
    /// Suspende el filtro multi-empresa mientras dure el ámbito. Se usa en el
    /// login (todavía no hay empresa) y en la carga de datos semilla.
    /// El contador permite anidar sin que un bloque interno reactive el filtro.
    /// </summary>
    public IDisposable BypassFilter()
    {
        Interlocked.Increment(ref _bypassDepth);
        return new BypassScope(this);
    }

    private sealed class BypassScope(CurrentTenant owner) : IDisposable
    {
        private bool _disposed;

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            Interlocked.Decrement(ref owner._bypassDepth);
        }
    }
}

public sealed class SystemClock : IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
    public DateOnly Today => DateOnly.FromDateTime(DateTime.UtcNow);
}
