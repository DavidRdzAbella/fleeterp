using FleetErp.Web.Models;
using FleetErp.Web.Services;
using FleetErp.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FleetErp.Web.Controllers;

/// <summary>
/// Cuentas de acceso al portal. Solo administradores: quien da de alta usuarios
/// puede repartir permisos, así que el módulo no se abre a despacho.
/// </summary>
public sealed class UsersController(IFleetApiClient api, ISessionContext session) : PortalController(session)
{
    private const int ListSize = 200;

    public async Task<IActionResult> Index(string? search, Guid? id, string? mode, CancellationToken ct)
    {
        if (!Session.IsAdministrator) return Forbid();
        return View(await BuildAsync(search, id, mode, form: null, ct));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(UserFormViewModel form, string? search, CancellationToken ct)
    {
        if (!Session.IsAdministrator) return Forbid();

        // La contraseña solo se pide al dar de alta; en la edición se cambia aparte.
        if (form.IsNew && string.IsNullOrWhiteSpace(form.Password))
            ModelState.AddModelError(nameof(form.Password), "Capture la contraseña inicial del usuario.");

        if (ModelState.IsValid)
        {
            var id = form.Id;
            var saved = form.Id is null
                ? await TryAsync(async () => id = await api.CreateUserAsync(
                    new CreateUserRequest(form.Email, form.FullName, form.Role, form.Password!), ct))
                : await TryAsync(() => api.UpdateUserAsync(form.Id.Value,
                    new UpdateUserRequest(form.Email, form.FullName, form.Role), ct));

            if (saved)
            {
                Notify(form.IsNew ? $"Usuario {form.Email} dado de alta." : "Usuario actualizado.");
                return RedirectToAction(nameof(Index), new { search, id });
            }
        }

        return View(nameof(Index), await BuildAsync(search, form.Id, form.IsNew ? "new" : "edit", form, ct));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(Guid id, string password, string? search, CancellationToken ct)
    {
        if (!Session.IsAdministrator) return Forbid();

        var ok = await TryAsync(() => api.ChangeUserPasswordAsync(id, new ChangePasswordRequest(password ?? string.Empty), ct));
        if (ok) Notify("Contraseña restablecida. Compártala al usuario por un medio seguro.");
        else Warn(FirstError());

        return RedirectToAction(nameof(Index), new { search, id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, string? search, CancellationToken ct)
    {
        if (!Session.IsAdministrator) return Forbid();

        var user = await api.GetUserAsync(id, ct);
        var reactivating = user is { IsActive: false };

        var ok = await TryAsync(() => api.SetUserActiveAsync(id, reactivating, ct));
        if (ok) Notify(reactivating ? "Usuario reactivado." : "Usuario desactivado.");
        else Warn(FirstError());

        return RedirectToAction(nameof(Index), new { search, id });
    }

    private async Task<UsersWorkbench> BuildAsync(
        string? search, Guid? id, string? mode, UserFormViewModel? form, CancellationToken ct)
    {
        var page = await api.SearchUsersAsync(search, 1, ListSize, ct);
        var resolved = ResolveMode(id, mode);

        // En alta no debe quedar nada seleccionado: el identificador que arrastra
        // la ruta abriria la ficha existente y guardar acabaria actualizandola.
        if (resolved == WorkbenchMode.New) id = null;

        var selected = id is null ? null : await api.GetUserAsync(id.Value, ct);
        if (selected is null && resolved == WorkbenchMode.View) resolved = WorkbenchMode.Empty;

        return new UsersWorkbench
        {
            Mode = resolved,
            CanWrite = Session.IsAdministrator,
            Selected = selected,
            Form = form ?? (selected is null ? new UserFormViewModel() : ToForm(selected)),
            List = new WorkbenchList
            {
                SearchPlaceholder = "Nombre o correo",
                Search = search,
                SelectedId = id,
                TotalCount = page.TotalCount,
                EmptyMessage = "No hay usuarios que coincidan.",
                Filters = Filters(("search", search)),
                Items = page.Items.Select(u => new WorkbenchItem(
                    u.Id,
                    u.FullName,
                    u.Email,
                    null,
                    Display.RoleLabel(u.Role),
                    u.IsActive ? Display.RoleTone(u.Role) : "void",
                    !u.IsActive)).ToList()
            }
        };
    }

    private static UserFormViewModel ToForm(UserModel u) => new()
    {
        Id = u.Id,
        FullName = u.FullName,
        Email = u.Email,
        Role = u.Role
    };
}
