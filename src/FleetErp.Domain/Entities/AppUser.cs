using FleetErp.Domain.Common;
using FleetErp.Domain.Enums;

namespace FleetErp.Domain.Entities;

/// <summary>
/// Usuario del portal, siempre ligado a una empresa. El hash se calcula fuera del
/// dominio (puerto <c>IPasswordHasher</c>) para no atar el modelo a un algoritmo.
/// </summary>
public class AppUser : TenantEntity, ISoftDeletable
{
    private AppUser() { }

    public AppUser(string email, string fullName, string passwordHash, UserRole role)
    {
        DomainException.Require(!string.IsNullOrWhiteSpace(email), "El correo es obligatorio.");
        DomainException.Require(email.Contains('@'), "El correo no tiene un formato válido.");
        DomainException.Require(!string.IsNullOrWhiteSpace(fullName), "El nombre del usuario es obligatorio.");
        DomainException.Require(!string.IsNullOrWhiteSpace(passwordHash), "La contraseña es obligatoria.");

        Email = email.Trim().ToLowerInvariant();
        FullName = fullName.Trim();
        PasswordHash = passwordHash;
        Role = role;
    }

    public string Email { get; private set; } = string.Empty;
    public string FullName { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public UserRole Role { get; private set; }
    public DateTimeOffset? LastLoginUtc { get; private set; }
    public bool IsActive { get; private set; } = true;

    public void ChangePassword(string passwordHash)
    {
        DomainException.Require(!string.IsNullOrWhiteSpace(passwordHash), "La contraseña es obligatoria.");
        PasswordHash = passwordHash;
    }

    public void Rename(string fullName)
    {
        DomainException.Require(!string.IsNullOrWhiteSpace(fullName), "El nombre del usuario es obligatorio.");
        FullName = fullName.Trim();
    }

    public void ChangeEmail(string email)
    {
        DomainException.Require(!string.IsNullOrWhiteSpace(email), "El correo es obligatorio.");
        DomainException.Require(email.Contains('@'), "El correo no tiene un formato válido.");
        Email = email.Trim().ToLowerInvariant();
    }

    public void ChangeRole(UserRole role) => Role = role;

    public void RegisterLogin(DateTimeOffset whenUtc) => LastLoginUtc = whenUtc;

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}
