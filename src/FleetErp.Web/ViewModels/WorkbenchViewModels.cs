using System.ComponentModel.DataAnnotations;
using FleetErp.Web.Models;

namespace FleetErp.Web.ViewModels;

/// <summary>
/// Base de las pantallas maestro-detalle. Todas comparten la misma estructura:
/// una lista con buscador, un modo y una ficha; lo único que cambia es qué
/// entidad se está capturando.
/// </summary>
public abstract class WorkbenchPage
{
    public required WorkbenchList List { get; init; }
    public required WorkbenchMode Mode { get; init; }
    public bool CanWrite { get; init; }

    public bool IsForm => Mode is WorkbenchMode.Edit or WorkbenchMode.New;
}

public sealed class FleetWorkbench : WorkbenchPage
{
    public VehicleModel? Selected { get; init; }
    public VehicleFormViewModel Form { get; init; } = new();
    public IReadOnlyList<MaintenanceOrderModel> RecentOrders { get; init; } = [];
}

public sealed class DriversWorkbench : WorkbenchPage
{
    public DriverModel? Selected { get; init; }
    public DriverFormViewModel Form { get; init; } = new();
    public DriverPerformanceModel? Performance { get; init; }
}

public sealed class CustomersWorkbench : WorkbenchPage
{
    public CustomerModel? Selected { get; init; }
    public CustomerFormViewModel Form { get; init; } = new();
}

public sealed class UsersWorkbench : WorkbenchPage
{
    public UserModel? Selected { get; init; }
    public UserFormViewModel Form { get; init; } = new();
}

public sealed class MaintenanceWorkbench : WorkbenchPage
{
    public MaintenanceOrderModel? Selected { get; init; }
    public MaintenanceFormViewModel Form { get; init; } = new();
    public CloseMaintenanceFormViewModel Close { get; init; } = new();
}

public sealed class FuelWorkbench : WorkbenchPage
{
    public FuelLogModel? Selected { get; init; }
    public FuelFormViewModel Form { get; init; } = new();
}

public sealed class ExpensesWorkbench : WorkbenchPage
{
    public ExpenseModel? Selected { get; init; }
    public ExpenseFormViewModel Form { get; init; } = new();
}

public sealed class CatalogWorkbench : WorkbenchPage
{
    /// <summary>Qué catálogo se está editando; los tres comparten la pantalla.</summary>
    public CatalogKind Kind { get; init; } = CatalogKind.VehicleTypes;

    public VehicleTypeModel? SelectedVehicleType { get; init; }
    public ExpenseCategoryModel? SelectedExpenseCategory { get; init; }
    public CustomFieldDefinitionModel? SelectedCustomField { get; init; }

    public VehicleTypeFormViewModel VehicleTypeForm { get; init; } = new();
    public ExpenseCategoryFormViewModel ExpenseCategoryForm { get; init; } = new();
    public CustomFieldFormViewModel CustomFieldForm { get; init; } = new();
}

public enum CatalogKind { VehicleTypes, ExpenseCategories, CustomFields }

// ---- Formularios ----------------------------------------------------------

public sealed class UserFormViewModel
{
    public Guid? Id { get; set; }

    [Display(Name = "Nombre completo")]
    [Required(ErrorMessage = "Capture el nombre del usuario.")]
    [StringLength(120)]
    public string FullName { get; set; } = string.Empty;

    [Display(Name = "Correo")]
    [Required(ErrorMessage = "Capture el correo.")]
    [EmailAddress(ErrorMessage = "El correo no tiene un formato válido.")]
    [StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Perfil")]
    public UserRole Role { get; set; } = UserRole.Dispatcher;

    /// <summary>Solo se pide al dar de alta; para cambiarla después hay acción aparte.</summary>
    [Display(Name = "Contraseña")]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
    public string? Password { get; set; }

    public bool IsNew => Id is null;
}

public sealed class MaintenanceFormViewModel
{
    public Guid? Id { get; set; }

    [Display(Name = "Unidad")]
    [Required(ErrorMessage = "Seleccione la unidad que entra a taller.")]
    public Guid VehicleId { get; set; }

    [Display(Name = "Tipo de servicio")]
    public MaintenanceKind Kind { get; set; } = MaintenanceKind.Preventive;

    [Display(Name = "Fecha de apertura")]
    public DateTime OpenedAt { get; set; } = DateTime.Now;

    [Display(Name = "Descripción del servicio")]
    [Required(ErrorMessage = "Describa el servicio requerido.")]
    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    public IReadOnlyList<LookupItemModel> VehicleOptions { get; set; } = [];
    public bool IsNew => Id is null;
}

public sealed class CloseMaintenanceFormViewModel
{
    [Display(Name = "Fecha de cierre")]
    public DateTime ClosedAt { get; set; } = DateTime.Now;

    [Display(Name = "Costo del servicio")]
    [Range(0, 10000000, ErrorMessage = "El costo debe ser un número positivo.")]
    public decimal Cost { get; set; }

    [Display(Name = "Taller")]
    [StringLength(150)]
    public string? Workshop { get; set; }

    [Display(Name = "Odómetro al servicio")]
    [Range(0, 10000000)]
    public decimal? OdometerAtService { get; set; }
}

public sealed class FuelFormViewModel
{
    public Guid? Id { get; set; }

    [Display(Name = "Unidad")]
    [Required(ErrorMessage = "Seleccione la unidad que cargó.")]
    public Guid VehicleId { get; set; }

    [Display(Name = "Viaje")]
    public Guid? TripId { get; set; }

    [Display(Name = "Conductor")]
    public Guid? DriverId { get; set; }

    [Display(Name = "Fecha y hora de carga")]
    public DateTime LoadedAt { get; set; } = DateTime.Now;

    [Display(Name = "Cantidad")]
    [Range(0.01, 5000, ErrorMessage = "La cantidad debe ser mayor a cero.")]
    public decimal Quantity { get; set; }

    [Display(Name = "Precio por unidad")]
    [Range(0, 1000, ErrorMessage = "El precio debe ser un número positivo.")]
    public decimal PricePerUnit { get; set; }

    [Display(Name = "Odómetro")]
    [Range(0, 10000000)]
    public decimal? OdometerReading { get; set; }

    [Display(Name = "Estación")]
    [StringLength(120)]
    public string? Station { get; set; }

    [Display(Name = "Ticket o referencia")]
    [StringLength(60)]
    public string? ReferenceNumber { get; set; }

    public IReadOnlyList<LookupItemModel> VehicleOptions { get; set; } = [];
    public IReadOnlyList<LookupItemModel> DriverOptions { get; set; } = [];
    public IReadOnlyList<LookupItemModel> TripOptions { get; set; } = [];
    public bool IsNew => Id is null;
}

public sealed class ExpenseFormViewModel
{
    public Guid? Id { get; set; }

    [Display(Name = "Concepto")]
    [Required(ErrorMessage = "Seleccione el concepto de gasto.")]
    public Guid CategoryId { get; set; }

    [Display(Name = "Viaje")]
    public Guid? TripId { get; set; }

    [Display(Name = "Unidad")]
    public Guid? VehicleId { get; set; }

    [Display(Name = "Conductor")]
    public Guid? DriverId { get; set; }

    [Display(Name = "Fecha")]
    public DateTime IncurredAt { get; set; } = DateTime.Now;

    [Display(Name = "Importe")]
    [Range(0.01, 10000000, ErrorMessage = "El importe debe ser mayor a cero.")]
    public decimal Amount { get; set; }

    [Display(Name = "Descripción")]
    [Required(ErrorMessage = "Describa el gasto.")]
    [StringLength(250)]
    public string Description { get; set; } = string.Empty;

    [Display(Name = "Comprobante o referencia")]
    [StringLength(60)]
    public string? ReferenceNumber { get; set; }

    public IReadOnlyList<ExpenseCategoryModel> CategoryOptions { get; set; } = [];
    public IReadOnlyList<LookupItemModel> VehicleOptions { get; set; } = [];
    public IReadOnlyList<LookupItemModel> DriverOptions { get; set; } = [];
    public IReadOnlyList<LookupItemModel> TripOptions { get; set; } = [];
    public bool IsNew => Id is null;
}

public sealed class VehicleTypeFormViewModel
{
    public Guid? Id { get; set; }

    [Display(Name = "Código")]
    [Required(ErrorMessage = "Capture el código.")]
    [StringLength(20)]
    public string Code { get; set; } = string.Empty;

    [Display(Name = "Nombre")]
    [Required(ErrorMessage = "Capture el nombre.")]
    [StringLength(80)]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Categoría")]
    public VehicleCategory Category { get; set; } = VehicleCategory.Motorized;

    public bool IsNew => Id is null;
}

public sealed class ExpenseCategoryFormViewModel
{
    public Guid? Id { get; set; }

    [Display(Name = "Código")]
    [Required(ErrorMessage = "Capture el código.")]
    [StringLength(20)]
    public string Code { get; set; } = string.Empty;

    [Display(Name = "Nombre")]
    [Required(ErrorMessage = "Capture el nombre.")]
    [StringLength(80)]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Aplica al viaje")]
    public bool IsTripRelated { get; set; } = true;

    public bool IsNew => Id is null;
}

public sealed class CustomFieldFormViewModel
{
    public Guid? Id { get; set; }

    [Display(Name = "Entidad")]
    public CustomFieldTarget Target { get; set; } = CustomFieldTarget.Trip;

    [Display(Name = "Etiqueta")]
    [Required(ErrorMessage = "Capture la etiqueta que verá el usuario.")]
    [StringLength(80)]
    public string Label { get; set; } = string.Empty;

    [Display(Name = "Llave")]
    [Required(ErrorMessage = "Capture la llave técnica.")]
    [StringLength(40)]
    [RegularExpression("^[A-Za-z0-9_ ]+$", ErrorMessage = "La llave solo admite letras, números, espacios y guion bajo.")]
    public string Key { get; set; } = string.Empty;

    [Display(Name = "Tipo de dato")]
    public CustomFieldType Type { get; set; } = CustomFieldType.Text;

    [Display(Name = "Obligatorio")]
    public bool IsRequired { get; set; }

    [Display(Name = "Opciones")]
    [StringLength(1000)]
    public string? Options { get; set; }

    [Display(Name = "Orden")]
    [Range(0, 999)]
    public int DisplayOrder { get; set; }

    public bool IsNew => Id is null;
}
