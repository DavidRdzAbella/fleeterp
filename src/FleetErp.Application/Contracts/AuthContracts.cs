using FleetErp.Domain.Enums;

namespace FleetErp.Application.Contracts;

public sealed record LoginRequest(string TenantSlug, string Email, string Password);

public sealed record LoginResponse(
    string Token,
    DateTimeOffset ExpiresAtUtc,
    Guid UserId,
    string FullName,
    string Email,
    UserRole Role,
    Guid TenantId,
    string TenantName,
    string TenantSlug,
    TenantSettingsDto Settings);
