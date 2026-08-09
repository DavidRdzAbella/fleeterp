using FleetErp.Web.Models;
using FleetErp.Web.Services;
using FleetErp.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FleetErp.Web.Controllers;

/// <summary>
/// Catálogos parametrizables: tipos de unidad, conceptos de gasto y campos a la
/// medida. Es la palanca de genericidad del producto, y por eso los tres viven
/// en la misma pantalla: quien implanta el sistema los recorre de corrido.
/// </summary>
public sealed class CatalogsController(IFleetApiClient api, ISessionContext session) : PortalController(session)
{
    public async Task<IActionResult> Index(CatalogKind kind, string? search, Guid? id, string? mode, CancellationToken ct)
    {
        if (!Session.IsAdministrator) return Forbid();
        return View(await BuildAsync(kind, search, id, mode, null, null, null, ct));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveVehicleType(VehicleTypeFormViewModel form, string? search, CancellationToken ct)
    {
        if (!Session.IsAdministrator) return Forbid();

        if (ModelState.IsValid)
        {
            var id = form.Id;
            var request = new UpsertVehicleTypeRequest(form.Code, form.Name, form.Category);
            var saved = form.Id is null
                ? await TryAsync(async () => id = await api.CreateVehicleTypeAsync(request, ct))
                : await TryAsync(() => api.UpdateVehicleTypeAsync(form.Id.Value, request, ct));

            if (saved)
            {
                Notify(form.IsNew ? $"Tipo de unidad {form.Name} agregado." : "Tipo de unidad actualizado.");
                return RedirectToAction(nameof(Index), new { kind = CatalogKind.VehicleTypes, search, id });
            }
        }

        return View(nameof(Index), await BuildAsync(CatalogKind.VehicleTypes, search, form.Id,
            form.IsNew ? "new" : "edit", form, null, null, ct));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveExpenseCategory(ExpenseCategoryFormViewModel form, string? search, CancellationToken ct)
    {
        if (!Session.IsAdministrator) return Forbid();

        if (ModelState.IsValid)
        {
            var id = form.Id;
            var request = new UpsertExpenseCategoryRequest(form.Code, form.Name, form.IsTripRelated);
            var saved = form.Id is null
                ? await TryAsync(async () => id = await api.CreateExpenseCategoryAsync(request, ct))
                : await TryAsync(() => api.UpdateExpenseCategoryAsync(form.Id.Value, request, ct));

            if (saved)
            {
                Notify(form.IsNew ? $"Concepto {form.Name} agregado." : "Concepto actualizado.");
                return RedirectToAction(nameof(Index), new { kind = CatalogKind.ExpenseCategories, search, id });
            }
        }

        return View(nameof(Index), await BuildAsync(CatalogKind.ExpenseCategories, search, form.Id,
            form.IsNew ? "new" : "edit", null, form, null, ct));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveCustomField(CustomFieldFormViewModel form, string? search, CancellationToken ct)
    {
        if (!Session.IsAdministrator) return Forbid();

        if (form.Type == CustomFieldType.Select && string.IsNullOrWhiteSpace(form.Options))
            ModelState.AddModelError(nameof(form.Options), "Un campo de tipo lista requiere sus opciones separadas por |.");

        if (ModelState.IsValid)
        {
            var id = form.Id;
            var request = new UpsertCustomFieldDefinitionRequest(
                form.Target, form.Key, form.Label, form.Type, form.IsRequired, form.Options, form.DisplayOrder);

            var saved = form.Id is null
                ? await TryAsync(async () => id = await api.CreateCustomFieldAsync(request, ct))
                : await TryAsync(() => api.UpdateCustomFieldAsync(form.Id.Value, request, ct));

            if (saved)
            {
                Notify(form.IsNew
                    ? $"El campo «{form.Label}» ya aparece en los formularios de {form.Target}."
                    : "Campo actualizado.");
                return RedirectToAction(nameof(Index), new { kind = CatalogKind.CustomFields, search, id });
            }
        }

        return View(nameof(Index), await BuildAsync(CatalogKind.CustomFields, search, form.Id,
            form.IsNew ? "new" : "edit", null, null, form, ct));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(CatalogKind kind, Guid id, string? search, CancellationToken ct)
    {
        if (!Session.IsAdministrator) return Forbid();

        // Los catálogos no se borran: hay movimientos históricos que los citan.
        // Se desactivan, y por eso la acción también sirve para reactivar.
        var activate = await IsInactiveAsync(kind, id, ct);

        var ok = kind switch
        {
            CatalogKind.VehicleTypes => await TryAsync(() => api.SetVehicleTypeActiveAsync(id, activate, ct)),
            CatalogKind.ExpenseCategories => await TryAsync(() => api.SetExpenseCategoryActiveAsync(id, activate, ct)),
            _ => await TryAsync(() => api.SetCustomFieldActiveAsync(id, activate, ct))
        };

        if (ok) Notify(activate ? "Registro reactivado." : "Registro desactivado.");
        else Warn(FirstError());

        return RedirectToAction(nameof(Index), new { kind, search, id });
    }

    private async Task<bool> IsInactiveAsync(CatalogKind kind, Guid id, CancellationToken ct) => kind switch
    {
        CatalogKind.VehicleTypes => (await api.GetVehicleTypesAsync(true, ct)).FirstOrDefault(t => t.Id == id) is { IsActive: false },
        CatalogKind.ExpenseCategories => (await api.GetExpenseCategoriesAsync(true, ct)).FirstOrDefault(c => c.Id == id) is { IsActive: false },
        _ => (await api.GetCustomFieldsAsync(null, ct)).FirstOrDefault(f => f.Id == id) is { IsActive: false }
    };

    private async Task<CatalogWorkbench> BuildAsync(
        CatalogKind kind, string? search, Guid? id, string? mode,
        VehicleTypeFormViewModel? vehicleTypeForm,
        ExpenseCategoryFormViewModel? expenseCategoryForm,
        CustomFieldFormViewModel? customFieldForm,
        CancellationToken ct)
    {
        var resolved = ResolveMode(id, mode);

        // En alta no debe quedar nada seleccionado: el identificador que arrastra
        // la ruta abriría la ficha existente y guardar acabaría actualizándola.
        if (resolved == WorkbenchMode.New) id = null;

        var filters = Filters(("kind", kind.ToString()), ("search", search));

        var types = await api.GetVehicleTypesAsync(true, ct);
        var categories = await api.GetExpenseCategoriesAsync(true, ct);
        var fields = await api.GetCustomFieldsAsync(null, ct);

        bool Matches(params string?[] haystack) =>
            string.IsNullOrWhiteSpace(search) ||
            haystack.Any(h => (h ?? string.Empty).Contains(search.Trim(), StringComparison.OrdinalIgnoreCase));

        var (items, total) = kind switch
        {
            CatalogKind.VehicleTypes => Build(types.Where(t => Matches(t.Code, t.Name))
                .Select(t => new WorkbenchItem(t.Id, t.Name, t.Code, Display.CategoryLabel(t.Category),
                    t.IsActive ? null : "Inactivo", t.IsActive ? null : "void", !t.IsActive))),

            CatalogKind.ExpenseCategories => Build(categories.Where(c => Matches(c.Code, c.Name))
                .Select(c => new WorkbenchItem(c.Id, c.Name, c.Code, c.IsTripRelated ? "Viaje" : "Estructura",
                    c.IsActive ? null : "Inactivo", c.IsActive ? null : "void", !c.IsActive))),

            _ => Build(fields.Where(f => Matches(f.Key, f.Label))
                .Select(f => new WorkbenchItem(f.Id, f.Label, $"{f.Target} · {f.Key}", f.Type.ToString(),
                    f.IsActive ? null : "Inactivo", f.IsActive ? null : "void", !f.IsActive)))
        };

        var selectedType = kind == CatalogKind.VehicleTypes ? types.FirstOrDefault(t => t.Id == id) : null;
        var selectedCategory = kind == CatalogKind.ExpenseCategories ? categories.FirstOrDefault(c => c.Id == id) : null;
        var selectedField = kind == CatalogKind.CustomFields ? fields.FirstOrDefault(f => f.Id == id) : null;

        var nothingSelected = selectedType is null && selectedCategory is null && selectedField is null;
        if (nothingSelected && resolved == WorkbenchMode.View) resolved = WorkbenchMode.Empty;

        return new CatalogWorkbench
        {
            Kind = kind,
            Mode = resolved,
            CanWrite = Session.IsAdministrator,
            SelectedVehicleType = selectedType,
            SelectedExpenseCategory = selectedCategory,
            SelectedCustomField = selectedField,
            VehicleTypeForm = vehicleTypeForm ?? (selectedType is null ? new VehicleTypeFormViewModel() : new VehicleTypeFormViewModel
            {
                Id = selectedType.Id, Code = selectedType.Code, Name = selectedType.Name, Category = selectedType.Category
            }),
            ExpenseCategoryForm = expenseCategoryForm ?? (selectedCategory is null ? new ExpenseCategoryFormViewModel() : new ExpenseCategoryFormViewModel
            {
                Id = selectedCategory.Id, Code = selectedCategory.Code,
                Name = selectedCategory.Name, IsTripRelated = selectedCategory.IsTripRelated
            }),
            CustomFieldForm = customFieldForm ?? (selectedField is null ? new CustomFieldFormViewModel() : new CustomFieldFormViewModel
            {
                Id = selectedField.Id, Target = selectedField.Target, Label = selectedField.Label,
                Key = selectedField.Key, Type = selectedField.Type, IsRequired = selectedField.IsRequired,
                Options = string.Join('|', selectedField.Options), DisplayOrder = selectedField.DisplayOrder
            }),
            List = new WorkbenchList
            {
                SearchPlaceholder = kind switch
                {
                    CatalogKind.VehicleTypes => "Código o nombre del tipo",
                    CatalogKind.ExpenseCategories => "Código o nombre del concepto",
                    _ => "Llave o etiqueta del campo"
                },
                Search = search,
                SelectedId = id,
                TotalCount = total,
                EmptyMessage = "No hay registros que coincidan.",
                Filters = filters,
                Items = items
            }
        };
    }

    private static (IReadOnlyList<WorkbenchItem> Items, int Total) Build(IEnumerable<WorkbenchItem> source)
    {
        var list = source.ToList();
        return (list, list.Count);
    }
}
