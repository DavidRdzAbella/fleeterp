using FleetErp.Web.Models;
using FleetErp.Web.Services;
using FleetErp.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FleetErp.Web.Controllers;

/// <summary>Clientes a los que se les factura el flete.</summary>
public sealed class CustomersController(IFleetApiClient api, ISessionContext session) : PortalController(session)
{
    private const int ListSize = 200;

    public async Task<IActionResult> Index(string? search, Guid? id, string? mode, CancellationToken ct) =>
        View(await BuildAsync(search, id, mode, form: null, ct));

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(CustomerFormViewModel form, string? search, CancellationToken ct)
    {
        if (!Session.CanWrite) return Forbid();

        if (ModelState.IsValid)
        {
            var id = form.Id;
            var saved = form.Id is null
                ? await TryAsync(async () => id = await api.CreateCustomerAsync(ToRequest(form), ct))
                : await TryAsync(() => api.UpdateCustomerAsync(form.Id.Value, ToRequest(form), ct));

            if (saved)
            {
                Notify(form.IsNew ? $"Cliente {form.Name} dado de alta." : "Cliente actualizado.");
                return RedirectToAction(nameof(Index), new { search, id });
            }
        }

        return View(nameof(Index), await BuildAsync(search, form.Id, form.IsNew ? "new" : "edit", form, ct));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, string? search, CancellationToken ct)
    {
        if (!Session.CanWrite) return Forbid();

        var customer = await api.GetCustomerAsync(id, ct);
        var reactivating = customer is { IsActive: false };

        var ok = await TryAsync(() => api.SetCustomerActiveAsync(id, reactivating, ct));
        if (ok) Notify(reactivating ? "Cliente reactivado." : "Cliente dado de baja.");
        else Warn(FirstError());

        return RedirectToAction(nameof(Index), new { search, id });
    }

    private async Task<CustomersWorkbench> BuildAsync(
        string? search, Guid? id, string? mode, CustomerFormViewModel? form, CancellationToken ct)
    {
        var page = await api.SearchCustomersAsync(search, 1, ListSize, ct);
        var resolved = ResolveMode(id, mode);

        // En alta no debe quedar nada seleccionado: el identificador que arrastra
        // la ruta abriria la ficha existente y guardar acabaria actualizandola.
        if (resolved == WorkbenchMode.New) id = null;

        var selected = id is null ? null : await api.GetCustomerAsync(id.Value, ct);
        if (selected is null && resolved == WorkbenchMode.View) resolved = WorkbenchMode.Empty;

        return new CustomersWorkbench
        {
            Mode = resolved,
            CanWrite = Session.CanWrite,
            Selected = selected,
            Form = form ?? (selected is null ? new CustomerFormViewModel() : ToForm(selected)),
            List = new WorkbenchList
            {
                SearchPlaceholder = "Nombre o RFC",
                Search = search,
                SelectedId = id,
                TotalCount = page.TotalCount,
                EmptyMessage = "No hay clientes que coincidan.",
                Filters = Filters(("search", search)),
                Items = page.Items.Select(c => new WorkbenchItem(
                    c.Id,
                    c.Name,
                    c.ContactName.Or(c.TaxId.Or("Sin contacto")),
                    c.Phone,
                    c.IsActive ? null : "Baja",
                    c.IsActive ? null : "void",
                    !c.IsActive)).ToList()
            }
        };
    }

    private static UpsertCustomerRequest ToRequest(CustomerFormViewModel f) =>
        new(f.Name, f.TaxId, f.ContactName, f.Phone, f.Email, f.Address, null);

    private static CustomerFormViewModel ToForm(CustomerModel c) => new()
    {
        Id = c.Id,
        Name = c.Name,
        TaxId = c.TaxId,
        ContactName = c.ContactName,
        Phone = c.Phone,
        Email = c.Email,
        Address = c.Address
    };
}
