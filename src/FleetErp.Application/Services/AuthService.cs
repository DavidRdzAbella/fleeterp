using FleetErp.Application.Abstractions;
using FleetErp.Application.Common;
using FleetErp.Application.Contracts;
using FleetErp.Domain.Abstractions;

namespace FleetErp.Application.Services;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken ct = default);
}

public sealed class AuthService(
    IUnitOfWork uow,
    ICurrentTenant tenant,
    IPasswordHasher hasher,
    ITokenGenerator tokens,
    IClock clock) : IAuthService
{
    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        // El login ocurre antes de que exista una empresa en contexto, así que se
        // resuelve el tenant por su slug con el filtro desactivado.
        using var _ = tenant.BypassFilter();

        var slug = request.TenantSlug.Trim().ToLowerInvariant();
        var company = (await uow.Tenants.ListAsync(t => t.Slug == slug && t.IsActive, ct)).FirstOrDefault()
                      ?? throw new UnauthorizedException("Empresa, correo o contraseña incorrectos.");

        var email = request.Email.Trim().ToLowerInvariant();
        var user = (await uow.Users.ListAsync(u => u.TenantId == company.Id && u.Email == email && u.IsActive, ct))
                   .FirstOrDefault()
                   ?? throw new UnauthorizedException("Empresa, correo o contraseña incorrectos.");

        if (!hasher.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedException("Empresa, correo o contraseña incorrectos.");

        user.RegisterLogin(clock.UtcNow);
        uow.Users.Update(user);
        await uow.SaveChangesAsync(ct);

        var (token, expiresAt) = tokens.Generate(user, company);

        return new LoginResponse(
            token, expiresAt, user.Id, user.FullName, user.Email, user.Role,
            company.Id, company.Name, company.Slug, TenantSettingsDto.From(company.Settings));
    }
}
