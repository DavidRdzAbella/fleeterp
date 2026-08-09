using FleetErp.Web.Models;
using FleetErp.Web.Services;
using FleetErp.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FleetErp.Web.Controllers;

/// <summary>
/// Parametrización de la empresa: identidad, moneda, unidades de medida, valores
/// por defecto y marca visual. Los catálogos viven en su propio módulo porque
/// son mantenimiento continuo, no configuración de una sola vez.
/// </summary>
public sealed class SettingsController(IFleetApiClient api, ISessionContext session) : PortalController(session)
{
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        if (!Session.IsAdministrator) return Forbid();

        return View(new SettingsViewModel
        {
            Tenant = await api.GetTenantAsync(ct),
            VehicleTypes = await api.GetVehicleTypesAsync(true, ct),
            ExpenseCategories = await api.GetExpenseCategoriesAsync(true, ct),
            CustomFields = await api.GetCustomFieldsAsync(null, ct)
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveProfile(string name, string? taxId, string? contactEmail, string? phone, CancellationToken ct)
    {
        if (!Session.IsAdministrator) return Forbid();

        var ok = await TryAsync(() => api.UpdateTenantAsync(new UpdateTenantRequest(name, taxId, contactEmail, phone), ct));
        if (ok) Notify("Datos de la empresa actualizados.");
        else Warn(FirstError());

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveSettings(TenantSettingsModel settings, CancellationToken ct)
    {
        if (!Session.IsAdministrator) return Forbid();

        var ok = await TryAsync(() => api.UpdateTenantSettingsAsync(settings, ct));

        if (ok)
        {
            // La sesión guarda una copia de la parametrización: hay que volver a
            // entrar para que el portal tome los nuevos colores y unidades.
            Notify("Parametrización guardada. Cierre y vuelva a abrir sesión para ver los cambios de marca.");
        }
        else
        {
            Warn(FirstError());
        }

        return RedirectToAction(nameof(Index));
    }
}
