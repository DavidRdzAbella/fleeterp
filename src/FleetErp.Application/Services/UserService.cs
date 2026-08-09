using FleetErp.Application.Abstractions;
using FleetErp.Application.Common;
using FleetErp.Application.Contracts;
using FleetErp.Domain.Abstractions;
using FleetErp.Domain.Entities;
using FleetErp.Domain.Enums;

namespace FleetErp.Application.Services;

/// <summary>Alta y mantenimiento de las cuentas que entran al portal.</summary>
public interface IUserService
{
    Task<Guid> CreateAsync(CreateUserRequest request, CancellationToken ct = default);
    Task UpdateAsync(Guid id, UpdateUserRequest request, CancellationToken ct = default);
    Task ChangePasswordAsync(Guid id, ChangePasswordRequest request, CancellationToken ct = default);
    Task SetActiveAsync(Guid id, bool active, CancellationToken ct = default);
}

public sealed class UserService(
    IUnitOfWork uow,
    ICurrentTenant tenant,
    ICurrentUser currentUser,
    IPasswordHasher hasher) : IUserService
{
    public async Task<Guid> CreateAsync(CreateUserRequest request, CancellationToken ct = default)
    {
        await GuardEmailAsync(request.Email, null, ct);

        var user = new AppUser(request.Email, request.FullName, hasher.Hash(request.Password), request.Role)
        {
            TenantId = tenant.TenantId
        };

        await uow.Users.AddAsync(user, ct);
        await uow.SaveChangesAsync(ct);
        return user.Id;
    }

    public async Task UpdateAsync(Guid id, UpdateUserRequest request, CancellationToken ct = default)
    {
        var user = await Require(id, ct);
        await GuardEmailAsync(request.Email, id, ct);

        // Quitarse a uno mismo el rol de administrador deja a la empresa sin quién
        // administre; se exige que lo haga otra cuenta.
        if (user.Id == currentUser.UserId && user.Role == UserRole.Administrator && request.Role != UserRole.Administrator)
            throw new ConflictException("No puede quitarse a sí mismo el perfil de administrador.");

        if (user.Role == UserRole.Administrator && request.Role != UserRole.Administrator)
            await GuardLastAdministratorAsync(id, ct);

        user.Rename(request.FullName);
        user.ChangeEmail(request.Email);
        user.ChangeRole(request.Role);

        uow.Users.Update(user);
        await uow.SaveChangesAsync(ct);
    }

    public async Task ChangePasswordAsync(Guid id, ChangePasswordRequest request, CancellationToken ct = default)
    {
        var user = await Require(id, ct);
        user.ChangePassword(hasher.Hash(request.Password));

        uow.Users.Update(user);
        await uow.SaveChangesAsync(ct);
    }

    public async Task SetActiveAsync(Guid id, bool active, CancellationToken ct = default)
    {
        var user = await Require(id, ct);

        if (!active)
        {
            if (user.Id == currentUser.UserId)
                throw new ConflictException("No puede desactivar su propia cuenta.");

            if (user.Role == UserRole.Administrator) await GuardLastAdministratorAsync(id, ct);
        }

        if (active) user.Activate(); else user.Deactivate();

        uow.Users.Update(user);
        await uow.SaveChangesAsync(ct);
    }

    private async Task<AppUser> Require(Guid id, CancellationToken ct) =>
        await uow.Users.GetByIdAsync(id, ct) ?? throw new NotFoundException("el usuario", id);

    private async Task GuardEmailAsync(string email, Guid? excludeId, CancellationToken ct)
    {
        var normalized = email.Trim().ToLowerInvariant();
        if (await uow.Users.AnyAsync(u => u.Email == normalized && (excludeId == null || u.Id != excludeId), ct))
            throw new ConflictException($"Ya existe un usuario con el correo {normalized}.");
    }

    /// <summary>Una empresa sin administrador activo queda sin quién la configure.</summary>
    private async Task GuardLastAdministratorAsync(Guid excludeId, CancellationToken ct)
    {
        var remaining = await uow.Users.CountAsync(
            u => u.Role == UserRole.Administrator && u.IsActive && u.Id != excludeId, ct);

        if (remaining == 0)
            throw new ConflictException("La empresa debe conservar al menos un administrador activo.");
    }
}
