using FleetErp.Domain.Enums;

namespace FleetErp.Application.Contracts;

public sealed record UserDto(
    Guid Id, string Email, string FullName, UserRole Role,
    DateTimeOffset? LastLoginUtc, DateTimeOffset CreatedAtUtc, bool IsActive);

/// <summary>
/// Alta de usuario. La contraseña llega en claro una sola vez y se convierte en
/// hash antes de tocar la base; nunca se almacena ni se devuelve el texto.
/// </summary>
public sealed record CreateUserRequest(string Email, string FullName, UserRole Role, string Password);

/// <summary>
/// Edición. No incluye contraseña a propósito: cambiarla es una operación
/// distinta, con su propio permiso y su propia bitácora.
/// </summary>
public sealed record UpdateUserRequest(string Email, string FullName, UserRole Role);

public sealed record ChangePasswordRequest(string Password);

public sealed record UserFilter(string? Search, UserRole? Role, bool? IsActive);
