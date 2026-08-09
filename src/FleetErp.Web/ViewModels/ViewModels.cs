using System.ComponentModel.DataAnnotations;
using FleetErp.Web.Models;

namespace FleetErp.Web.ViewModels;

/// <summary>Rango de análisis elegido en la barra de periodo.</summary>
public sealed record PeriodSelection(string Key, string Label, int Days)
{
    public static readonly PeriodSelection Week = new("7d", "Últimos 7 días", 7);
    public static readonly PeriodSelection Fortnight = new("15d", "Últimos 15 días", 15);
    public static readonly PeriodSelection Month = new("30d", "Últimos 30 días", 30);
    public static readonly PeriodSelection Quarter = new("90d", "Últimos 90 días", 90);

    public static IReadOnlyList<PeriodSelection> All => [Week, Fortnight, Month, Quarter];

    public static PeriodSelection Resolve(string? key) =>
        All.FirstOrDefault(p => p.Key == key) ?? Month;

    /// <summary>Ventana en UTC que cierra al final del día de hoy.</summary>
    public (DateTimeOffset From, DateTimeOffset To) ToRange()
    {
        var today = DateTimeOffset.UtcNow.UtcDateTime.Date;
        return (new DateTimeOffset(today.AddDays(-Days + 1), TimeSpan.Zero),
                new DateTimeOffset(today.AddDays(1).AddTicks(-1), TimeSpan.Zero));
    }
}

public sealed class DashboardViewModel
{
    public required FleetDashboardModel Dashboard { get; init; }
    public required IReadOnlyList<VehicleModel> Fleet { get; init; }
    public required IReadOnlyList<TripListItemModel> ActiveTrips { get; init; }
    public required IReadOnlyList<TripListItemModel> UpcomingTrips { get; init; }
    public required PeriodSelection Period { get; init; }
}

public sealed class TripsIndexViewModel
{
    public required PagedResult<TripListItemModel> Trips { get; init; }
    public string? Search { get; init; }
    public TripStatus? Status { get; init; }
    public Guid? DriverId { get; init; }
    public required IReadOnlyList<LookupItemModel> Drivers { get; init; }
}

/// <summary>
/// Formulario de viaje. Reúne en una sola pantalla lo que el despachador captura
/// al programar: quién, con qué unidad, a dónde, cuántos kilómetros, con cuánto
/// combustible sale, si va a cargar en ruta y cuánta carga lleva.
/// </summary>
public sealed class TripFormViewModel
{
    public Guid? Id { get; set; }
    public string? Folio { get; set; }
    public TripStatus Status { get; set; } = TripStatus.Planned;

    [Display(Name = "Conductor")]
    [Required(ErrorMessage = "Seleccione al conductor.")]
    public Guid DriverId { get; set; }

    [Display(Name = "Unidad motriz")]
    [Required(ErrorMessage = "Seleccione la unidad.")]
    public Guid VehicleId { get; set; }

    [Display(Name = "Caja o remolque")]
    public Guid? TrailerId { get; set; }

    [Display(Name = "Cliente")]
    public Guid? CustomerId { get; set; }

    [Display(Name = "Origen")]
    [Required(ErrorMessage = "Indique el origen.")]
    [StringLength(150)]
    public string Origin { get; set; } = string.Empty;

    [Display(Name = "Destino")]
    [Required(ErrorMessage = "Indique el destino.")]
    [StringLength(150)]
    public string Destination { get; set; } = string.Empty;

    [Display(Name = "Kilómetros por recorrer")]
    [Range(0, 100000, ErrorMessage = "La distancia debe ser un número positivo.")]
    public decimal PlannedDistance { get; set; }

    [Display(Name = "Salida programada")]
    [Required(ErrorMessage = "Indique la fecha y hora de salida.")]
    public DateTime ScheduledDeparture { get; set; } = DateTime.Now.AddHours(2);

    [Display(Name = "Llegada programada")]
    public DateTime? ScheduledArrival { get; set; }

    [Display(Name = "Combustible inicial")]
    [Range(0, 5000, ErrorMessage = "El combustible inicial debe ser un número positivo.")]
    public decimal InitialFuel { get; set; }

    [Display(Name = "Va a cargar combustible en ruta")]
    public bool RefuelPlanned { get; set; }

    [Display(Name = "Peso de la carga")]
    [Range(0, 1000000, ErrorMessage = "El peso debe ser un número positivo.")]
    public decimal CargoWeight { get; set; }

    [Display(Name = "Unidad de peso")]
    public WeightUnit CargoWeightUnit { get; set; } = WeightUnit.Kilogram;

    [Display(Name = "Descripción de la carga")]
    [StringLength(300)]
    public string? CargoDescription { get; set; }

    [Display(Name = "Flete cobrado")]
    [Range(0, 10000000, ErrorMessage = "El flete debe ser un número positivo.")]
    public decimal FreightRevenue { get; set; }

    [Display(Name = "Esquema de pago al operador")]
    public DriverPayScheme DriverPayScheme { get; set; } = Models.DriverPayScheme.PerHour;

    [Display(Name = "Tarifa")]
    [Range(0, 1000000, ErrorMessage = "La tarifa debe ser un número positivo.")]
    public decimal DriverPayRate { get; set; }

    [Display(Name = "Notas")]
    [StringLength(1000)]
    public string? Notes { get; set; }

    /// <summary>Valores de los campos que la empresa agregó por su cuenta.</summary>
    public Dictionary<string, string?> CustomFields { get; set; } = [];

    // Datos para poblar la pantalla; no forman parte de lo que se envía.
    public IReadOnlyList<LookupItemModel> DriverOptions { get; set; } = [];
    public IReadOnlyList<LookupItemModel> VehicleOptions { get; set; } = [];
    public IReadOnlyList<LookupItemModel> TrailerOptions { get; set; } = [];
    public IReadOnlyList<LookupItemModel> CustomerOptions { get; set; } = [];
    public IReadOnlyList<CustomFieldDefinitionModel> CustomFieldDefinitions { get; set; } = [];
    public bool IsNew => Id is null;
}

public sealed class TripDetailViewModel
{
    public required TripDetailModel Trip { get; init; }
    public required IReadOnlyList<ExpenseCategoryModel> ExpenseCategories { get; init; }
    public DispatchForm Dispatch { get; init; } = new();
    public CompleteForm Complete { get; init; } = new();
}

public sealed class DispatchForm
{
    [Display(Name = "Hora de salida")]
    public DateTime DepartureAt { get; set; } = DateTime.Now;

    [Display(Name = "Odómetro de salida")]
    [Range(0, 10000000, ErrorMessage = "Capture una lectura válida del odómetro.")]
    public decimal OdometerStart { get; set; }

    [Display(Name = "Combustible al salir")]
    [Range(0, 5000)]
    public decimal? InitialFuel { get; set; }
}

public sealed class CompleteForm
{
    [Display(Name = "Hora de llegada")]
    public DateTime ArrivalAt { get; set; } = DateTime.Now;

    [Display(Name = "Odómetro de llegada")]
    [Range(0, 10000000, ErrorMessage = "Capture una lectura válida del odómetro.")]
    public decimal OdometerEnd { get; set; }

    [Display(Name = "Combustible al llegar")]
    [Range(0, 5000)]
    public decimal? FinalFuel { get; set; }

    [Display(Name = "Horas del operador")]
    [Range(0, 720)]
    public decimal? DriverHours { get; set; }
}

public sealed class QuickExpenseForm
{
    public Guid TripId { get; set; }

    [Display(Name = "Concepto")]
    [Required(ErrorMessage = "Seleccione el concepto de gasto.")]
    public Guid CategoryId { get; set; }

    [Display(Name = "Importe")]
    [Range(0.01, 1000000, ErrorMessage = "El importe debe ser mayor a cero.")]
    public decimal Amount { get; set; }

    [Display(Name = "Descripción")]
    [Required(ErrorMessage = "Describa el gasto.")]
    [StringLength(250)]
    public string Description { get; set; } = string.Empty;

    [Display(Name = "Fecha")]
    public DateTime IncurredAt { get; set; } = DateTime.Now;
}

public sealed class QuickFuelForm
{
    public Guid TripId { get; set; }
    public Guid VehicleId { get; set; }

    [Display(Name = "Cantidad cargada")]
    [Range(0.01, 5000, ErrorMessage = "La cantidad debe ser mayor a cero.")]
    public decimal Quantity { get; set; }

    [Display(Name = "Precio por unidad")]
    [Range(0, 1000, ErrorMessage = "El precio debe ser un número positivo.")]
    public decimal PricePerUnit { get; set; }

    [Display(Name = "Odómetro")]
    public decimal? OdometerReading { get; set; }

    [Display(Name = "Estación")]
    [StringLength(120)]
    public string? Station { get; set; }

    [Display(Name = "Fecha")]
    public DateTime LoadedAt { get; set; } = DateTime.Now;
}

public sealed class VehicleFormViewModel
{
    public Guid? Id { get; set; }

    [Display(Name = "Número económico")]
    [Required(ErrorMessage = "Capture el número económico.")]
    [StringLength(30)]
    public string EconomicNumber { get; set; } = string.Empty;

    [Display(Name = "Placa")]
    [Required(ErrorMessage = "Capture la placa.")]
    [StringLength(20)]
    public string PlateNumber { get; set; } = string.Empty;

    [Display(Name = "Tipo de unidad")]
    [Required(ErrorMessage = "Seleccione el tipo de unidad.")]
    public Guid VehicleTypeId { get; set; }

    [Display(Name = "Marca")]
    [StringLength(60)]
    public string? Brand { get; set; }

    [Display(Name = "Modelo")]
    [StringLength(60)]
    public string? Model { get; set; }

    [Display(Name = "Año")]
    [Range(1950, 2100, ErrorMessage = "El año no es válido.")]
    public int? Year { get; set; }

    [Display(Name = "Número de serie (VIN)")]
    [StringLength(40)]
    public string? Vin { get; set; }

    [Display(Name = "Capacidad de carga")]
    [Range(0, 1000000)]
    public decimal? CargoCapacity { get; set; }

    [Display(Name = "Capacidad del tanque")]
    [Range(0, 10000)]
    public decimal? TankCapacity { get; set; }

    [Display(Name = "Odómetro inicial")]
    [Range(0, 10000000)]
    public decimal InitialOdometer { get; set; }

    [Display(Name = "Vence el seguro")]
    public DateTime? InsuranceExpiry { get; set; }

    [Display(Name = "Vence la tarjeta de circulación")]
    public DateTime? CirculationCardExpiry { get; set; }

    public Dictionary<string, string?> CustomFields { get; set; } = [];
    public IReadOnlyList<VehicleTypeModel> TypeOptions { get; set; } = [];
    public IReadOnlyList<CustomFieldDefinitionModel> CustomFieldDefinitions { get; set; } = [];
    public bool IsNew => Id is null;
}

public sealed class DriversIndexViewModel
{
    public required PagedResult<DriverModel> Drivers { get; init; }
    public required IReadOnlyList<DriverRankingRowModel> Ranking { get; init; }
    public required PeriodSelection Period { get; init; }
    public required DriverRankingCriteria Criteria { get; init; }
    public string? Search { get; init; }
}

public sealed class DriverFormViewModel
{
    public Guid? Id { get; set; }

    [Display(Name = "Nombre")]
    [Required(ErrorMessage = "Capture el nombre.")]
    [StringLength(60)]
    public string FirstName { get; set; } = string.Empty;

    [Display(Name = "Apellidos")]
    [Required(ErrorMessage = "Capture los apellidos.")]
    [StringLength(60)]
    public string LastName { get; set; } = string.Empty;

    [Display(Name = "Número de empleado")]
    [StringLength(30)]
    public string? EmployeeNumber { get; set; }

    [Display(Name = "Número de licencia")]
    [Required(ErrorMessage = "Capture el número de licencia.")]
    [StringLength(40)]
    public string LicenseNumber { get; set; } = string.Empty;

    [Display(Name = "Tipo de licencia")]
    [StringLength(30)]
    public string? LicenseType { get; set; }

    [Display(Name = "Vence la licencia")]
    public DateTime? LicenseExpiry { get; set; }

    [Display(Name = "Teléfono")]
    [StringLength(30)]
    public string? Phone { get; set; }

    [Display(Name = "Correo")]
    [EmailAddress(ErrorMessage = "El correo no tiene un formato válido.")]
    public string? Email { get; set; }

    [Display(Name = "Fecha de ingreso")]
    public DateTime? HireDate { get; set; }

    [Display(Name = "Esquema de pago")]
    public DriverPayScheme PayScheme { get; set; } = Models.DriverPayScheme.PerHour;

    [Display(Name = "Tarifa")]
    [Range(0, 1000000, ErrorMessage = "La tarifa debe ser un número positivo.")]
    public decimal PayRate { get; set; }

    public Dictionary<string, string?> CustomFields { get; set; } = [];
    public IReadOnlyList<CustomFieldDefinitionModel> CustomFieldDefinitions { get; set; } = [];
    public bool IsNew => Id is null;
}

public sealed class DriverDetailViewModel
{
    public required DriverModel Driver { get; init; }
    public required DriverPerformanceModel Performance { get; init; }
    public required IReadOnlyList<TripListItemModel> RecentTrips { get; init; }
    public required PeriodSelection Period { get; init; }
}

public sealed class CustomerFormViewModel
{
    public Guid? Id { get; set; }

    [Display(Name = "Nombre o razón social")]
    [Required(ErrorMessage = "Capture el nombre del cliente.")]
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "RFC")]
    [StringLength(30)]
    public string? TaxId { get; set; }

    [Display(Name = "Contacto")]
    [StringLength(120)]
    public string? ContactName { get; set; }

    [Display(Name = "Teléfono")]
    [StringLength(30)]
    public string? Phone { get; set; }

    [Display(Name = "Correo")]
    [EmailAddress(ErrorMessage = "El correo no tiene un formato válido.")]
    public string? Email { get; set; }

    [Display(Name = "Dirección")]
    [StringLength(300)]
    public string? Address { get; set; }

    public bool IsNew => Id is null;
}

public sealed class FinanceViewModel
{
    public required FinanceReportModel Report { get; init; }
    public required PeriodSelection Period { get; init; }
    public required PagedResult<ExpenseModel> RecentExpenses { get; init; }
}

public sealed class SettingsViewModel
{
    public required TenantModel Tenant { get; init; }
    public required IReadOnlyList<VehicleTypeModel> VehicleTypes { get; init; }
    public required IReadOnlyList<ExpenseCategoryModel> ExpenseCategories { get; init; }
    public required IReadOnlyList<CustomFieldDefinitionModel> CustomFields { get; init; }
}
