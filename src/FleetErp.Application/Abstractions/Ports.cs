using FleetErp.Domain.Entities;
using FleetErp.Domain.Enums;

namespace FleetErp.Application.Abstractions;

/// <summary>Empresa activa en la petición. La resuelve la API a partir del token o de la cabecera.</summary>
public interface ICurrentTenant
{
    Guid TenantId { get; }
    string Slug { get; }
    bool IsResolved { get; }
    void Set(Guid tenantId, string slug);

    /// <summary>Desactiva el filtro multi-empresa para tareas de sistema (semilla, migraciones).</summary>
    IDisposable BypassFilter();
    bool FilterDisabled { get; }
}

/// <summary>Usuario autenticado. Se usa para la auditoría y para reglas por rol.</summary>
public interface ICurrentUser
{
    Guid? UserId { get; }
    string? Email { get; }
    UserRole? Role { get; }
}

/// <summary>Reloj inyectable: sin esto, las reglas con fechas no son verificables en pruebas.</summary>
public interface IClock
{
    DateTimeOffset UtcNow { get; }
    DateOnly Today { get; }
}

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hash);
}

public interface ITokenGenerator
{
    /// <summary>Emite el JWT con los claims de usuario y empresa.</summary>
    (string Token, DateTimeOffset ExpiresAtUtc) Generate(AppUser user, Tenant tenant);
}

/// <summary>Consecutivo de folios por empresa (VJ-2026-000123).</summary>
public interface IFolioGenerator
{
    Task<string> NextTripFolioAsync(CancellationToken ct = default);
    Task<string> NextMaintenanceFolioAsync(CancellationToken ct = default);
}
