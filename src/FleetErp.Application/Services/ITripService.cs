using FleetErp.Application.Contracts;

namespace FleetErp.Application.Services;

/// <summary>
/// Casos de uso del viaje. Coordina agregados (viaje, unidad, conductor) y
/// delega toda regla de negocio a las entidades; aquí no se decide nada que el
/// dominio pueda decidir por sí mismo.
/// </summary>
public interface ITripService
{
    Task<Guid> CreateAsync(CreateTripRequest request, CancellationToken ct = default);
    Task UpdateAsync(Guid id, UpdateTripRequest request, CancellationToken ct = default);

    /// <summary>Salida a ruta: ocupa la unidad y al operador.</summary>
    Task DispatchAsync(Guid id, DispatchTripRequest request, CancellationToken ct = default);

    /// <summary>Llegada: libera unidad y operador, y cierra números del viaje.</summary>
    Task CompleteAsync(Guid id, CompleteTripRequest request, CancellationToken ct = default);

    Task CancelAsync(Guid id, CancelTripRequest request, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
