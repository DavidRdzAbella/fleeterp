using FleetErp.Domain.Common;
using FleetErp.Domain.Enums;

namespace FleetErp.Domain.Entities;

/// <summary>
/// Viaje: la raíz de agregado del sistema y el documento donde se registra todo
/// lo que el cliente pidió en la "pantallita" de control — salida y llegada,
/// kilómetros, combustible inicial, si va a cargar o no, carga en kg/toneladas,
/// destino, conductor y unidad — más el resultado económico del recorrido.
/// </summary>
/// <remarks>
/// Las reglas de transición viven aquí, no en los servicios, para que la
/// secuencia Planeado → En ruta → Concluido sea imposible de romper desde
/// cualquier punto de entrada (API, importación masiva o pruebas).
/// </remarks>
public class Trip : TenantEntity
{
    private readonly List<FuelLog> _fuelLogs = [];
    private readonly List<Expense> _expenses = [];

    private Trip() { }

    public Trip(string folio, Guid driverId, Guid vehicleId, string origin, string destination,
                DateTimeOffset scheduledDepartureUtc)
    {
        DomainException.Require(!string.IsNullOrWhiteSpace(folio), "El folio del viaje es obligatorio.");
        DomainException.Require(driverId != Guid.Empty, "El conductor es obligatorio.");
        DomainException.Require(vehicleId != Guid.Empty, "La unidad es obligatoria.");

        Folio = folio.Trim().ToUpperInvariant();
        DriverId = driverId;
        VehicleId = vehicleId;
        SetRoute(origin, destination, 0m);
        ScheduledDepartureUtc = scheduledDepartureUtc;
        CustomFields = new CustomFieldValues();
    }

    public string Folio { get; private set; } = string.Empty;

    public Guid DriverId { get; private set; }
    public Driver? Driver { get; private set; }

    /// <summary>Unidad motriz (tractocamión).</summary>
    public Guid VehicleId { get; private set; }
    public Vehicle? Vehicle { get; private set; }

    /// <summary>Caja o remolque enganchado. Opcional: no todo viaje arrastra.</summary>
    public Guid? TrailerId { get; private set; }
    public Vehicle? Trailer { get; private set; }

    public Guid? CustomerId { get; private set; }
    public Customer? Customer { get; private set; }

    // ---- Ruta -------------------------------------------------------------
    public string Origin { get; private set; } = string.Empty;
    public string Destination { get; private set; } = string.Empty;

    /// <summary>"Kilómetros por recorrer": distancia estimada al planear el viaje.</summary>
    public decimal PlannedDistance { get; private set; }

    // ---- Tiempos ----------------------------------------------------------
    public DateTimeOffset ScheduledDepartureUtc { get; private set; }
    public DateTimeOffset? ScheduledArrivalUtc { get; private set; }
    public DateTimeOffset? ActualDepartureUtc { get; private set; }
    public DateTimeOffset? ActualArrivalUtc { get; private set; }

    // ---- Odómetro y combustible ------------------------------------------
    public decimal? OdometerStart { get; private set; }
    public decimal? OdometerEnd { get; private set; }

    /// <summary>Combustible con el que sale la unidad.</summary>
    public decimal InitialFuel { get; private set; }
    public decimal? FinalFuel { get; private set; }

    /// <summary>"Si va a cargar gasolina o no": se planea la carga en ruta.</summary>
    public bool RefuelPlanned { get; private set; }

    // ---- Carga ------------------------------------------------------------
    public decimal CargoWeight { get; private set; }

    /// <summary>Kilogramos o toneladas: se guarda la unidad usada al capturar.</summary>
    public WeightUnit CargoWeightUnit { get; private set; } = WeightUnit.Kilogram;
    public string? CargoDescription { get; private set; }

    // ---- Dinero -----------------------------------------------------------
    /// <summary>Flete cobrado al cliente: el "cuánto vendió" del conductor.</summary>
    public decimal FreightRevenue { get; private set; }

    /// <summary>
    /// Esquema y tarifa congelados al momento del viaje. Si mañana se le sube el
    /// sueldo al operador, la nómina histórica no se recalcula sola.
    /// </summary>
    public DriverPayScheme DriverPayScheme { get; private set; } = DriverPayScheme.PerHour;
    public decimal DriverPayRate { get; private set; }
    public decimal? DriverHours { get; private set; }
    public decimal DriverPayAmount { get; private set; }

    public TripStatus Status { get; private set; } = TripStatus.Planned;
    public string? Notes { get; private set; }
    public string? CancellationReason { get; private set; }

    public CustomFieldValues CustomFields { get; private set; } = new();

    public IReadOnlyCollection<FuelLog> FuelLogs => _fuelLogs.AsReadOnly();
    public IReadOnlyCollection<Expense> Expenses => _expenses.AsReadOnly();

    // ---- Cálculos derivados ----------------------------------------------

    /// <summary>Distancia real por odómetro; si aún no cierra, cae a la planeada.</summary>
    public decimal ActualDistance =>
        OdometerStart is not null && OdometerEnd is not null
            ? Math.Max(0m, OdometerEnd.Value - OdometerStart.Value)
            : 0m;

    public decimal EffectiveDistance => ActualDistance > 0 ? ActualDistance : PlannedDistance;

    public decimal FuelPurchased => _fuelLogs.Sum(f => f.Quantity);

    /// <summary>Combustible consumido = con el que salió + lo que cargó − lo que le quedó.</summary>
    public decimal? FuelConsumed =>
        FinalFuel is null ? null : Math.Max(0m, InitialFuel + FuelPurchased - FinalFuel.Value);

    /// <summary>Rendimiento (distancia por unidad de volumen). Null si no hay datos para calcularlo.</summary>
    public decimal? FuelEfficiency =>
        FuelConsumed is > 0 && ActualDistance > 0
            ? Math.Round(ActualDistance / FuelConsumed.Value, 2, MidpointRounding.AwayFromZero)
            : null;

    public TimeSpan? Duration =>
        ActualDepartureUtc is not null && ActualArrivalUtc is not null
            ? ActualArrivalUtc.Value - ActualDepartureUtc.Value
            : null;

    public decimal FuelCost => _fuelLogs.Sum(f => f.TotalCost);
    public decimal OtherExpensesCost => _expenses.Sum(e => e.Amount);

    /// <summary>Costo directo del viaje: combustible + gastos de ruta + pago al operador.</summary>
    public decimal TotalCost => FuelCost + OtherExpensesCost + DriverPayAmount;

    /// <summary>Ganancia de la empresa en el viaje.</summary>
    public decimal Profit => FreightRevenue - TotalCost;

    public decimal ProfitMargin => FreightRevenue == 0 ? 0m
        : Math.Round(Profit / FreightRevenue * 100m, 2, MidpointRounding.AwayFromZero);

    /// <summary>Llegó después de lo comprometido con el cliente.</summary>
    public bool IsLate =>
        ScheduledArrivalUtc is not null && ActualArrivalUtc is not null &&
        ActualArrivalUtc.Value > ScheduledArrivalUtc.Value;

    // ---- Comportamiento ---------------------------------------------------

    public void SetRoute(string origin, string destination, decimal plannedDistance)
    {
        DomainException.Require(!string.IsNullOrWhiteSpace(origin), "El origen es obligatorio.");
        DomainException.Require(!string.IsNullOrWhiteSpace(destination), "El destino es obligatorio.");
        DomainException.Require(plannedDistance >= 0, "La distancia planeada no puede ser negativa.");
        Origin = origin.Trim();
        Destination = destination.Trim();
        PlannedDistance = plannedDistance;
    }

    public void SetSchedule(DateTimeOffset scheduledDepartureUtc, DateTimeOffset? scheduledArrivalUtc)
    {
        DomainException.Require(scheduledArrivalUtc is null || scheduledArrivalUtc > scheduledDepartureUtc,
            "La llegada programada debe ser posterior a la salida programada.");
        ScheduledDepartureUtc = scheduledDepartureUtc;
        ScheduledArrivalUtc = scheduledArrivalUtc;
    }

    public void SetAssignment(Guid driverId, Guid vehicleId, Guid? trailerId, Guid? customerId)
    {
        DomainException.Require(Status == TripStatus.Planned,
            "Solo un viaje en planeación puede reasignar conductor o unidad.");
        DomainException.Require(driverId != Guid.Empty, "El conductor es obligatorio.");
        DomainException.Require(vehicleId != Guid.Empty, "La unidad es obligatoria.");
        DomainException.Require(trailerId != vehicleId, "El remolque no puede ser la misma unidad motriz.");
        DriverId = driverId;
        VehicleId = vehicleId;
        TrailerId = trailerId;
        CustomerId = customerId;
    }

    public void SetCargo(decimal weight, WeightUnit unit, string? description)
    {
        DomainException.Require(weight >= 0, "El peso de la carga no puede ser negativo.");
        CargoWeight = weight;
        CargoWeightUnit = unit;
        CargoDescription = description?.Trim();
    }

    public void SetFuelPlan(decimal initialFuel, bool refuelPlanned)
    {
        DomainException.Require(initialFuel >= 0, "El combustible inicial no puede ser negativo.");
        InitialFuel = initialFuel;
        RefuelPlanned = refuelPlanned;
    }

    public void SetCommercialTerms(decimal freightRevenue, DriverPayScheme payScheme, decimal payRate)
    {
        DomainException.Require(freightRevenue >= 0, "El flete no puede ser negativo.");
        DomainException.Require(payRate >= 0, "La tarifa del conductor no puede ser negativa.");
        FreightRevenue = freightRevenue;
        DriverPayScheme = payScheme;
        DriverPayRate = payRate;
        RecalculateDriverPay();
    }

    public void SetNotes(string? notes) => Notes = notes?.Trim();

    /// <summary>Salida a ruta: registra hora real, odómetro y combustible de salida.</summary>
    public void Dispatch(DateTimeOffset departureUtc, decimal odometerStart, decimal? initialFuel = null)
    {
        DomainException.Require(Status == TripStatus.Planned,
            $"Solo se puede despachar un viaje en planeación (estado actual: {Status}).");
        DomainException.Require(odometerStart >= 0, "El odómetro de salida no puede ser negativo.");

        ActualDepartureUtc = departureUtc;
        OdometerStart = odometerStart;
        if (initialFuel is not null)
        {
            DomainException.Require(initialFuel.Value >= 0, "El combustible inicial no puede ser negativo.");
            InitialFuel = initialFuel.Value;
        }
        Status = TripStatus.InProgress;
    }

    /// <summary>Llegada: cierra tiempos, distancia real, combustible y pago del operador.</summary>
    public void Complete(DateTimeOffset arrivalUtc, decimal odometerEnd, decimal? finalFuel, decimal? driverHours)
    {
        DomainException.Require(Status == TripStatus.InProgress,
            $"Solo se puede concluir un viaje en ruta (estado actual: {Status}).");
        DomainException.Require(ActualDepartureUtc is not null, "El viaje no tiene hora de salida registrada.");
        DomainException.Require(arrivalUtc >= ActualDepartureUtc!.Value,
            "La hora de llegada no puede ser anterior a la de salida.");
        DomainException.Require(OdometerStart is not null, "El viaje no tiene odómetro de salida registrado.");
        DomainException.Require(odometerEnd >= OdometerStart!.Value,
            $"El odómetro de llegada ({odometerEnd:N0}) no puede ser menor al de salida ({OdometerStart.Value:N0}).");
        DomainException.Require(finalFuel is null or >= 0, "El combustible final no puede ser negativo.");
        DomainException.Require(driverHours is null or >= 0, "Las horas del conductor no pueden ser negativas.");

        ActualArrivalUtc = arrivalUtc;
        OdometerEnd = odometerEnd;
        FinalFuel = finalFuel;
        DriverHours = driverHours ?? (decimal)(arrivalUtc - ActualDepartureUtc.Value).TotalHours;
        Status = TripStatus.Completed;
        RecalculateDriverPay();
    }

    public void Cancel(string reason)
    {
        DomainException.Require(Status is TripStatus.Planned or TripStatus.InProgress,
            "Un viaje concluido o ya cancelado no se puede cancelar.");
        DomainException.Require(!string.IsNullOrWhiteSpace(reason), "El motivo de cancelación es obligatorio.");
        CancellationReason = reason.Trim();
        Status = TripStatus.Cancelled;
    }

    public void AddFuelLog(FuelLog log)
    {
        DomainException.Require(Status is TripStatus.Planned or TripStatus.InProgress or TripStatus.Completed,
            "No se pueden registrar cargas de combustible en un viaje cancelado.");
        _fuelLogs.Add(log);
    }

    public void AddExpense(Expense expense)
    {
        DomainException.Require(Status != TripStatus.Cancelled,
            "No se pueden registrar gastos en un viaje cancelado.");
        _expenses.Add(expense);
    }

    /// <summary>
    /// Traduce el esquema de pago a un importe. Está aquí y no en un servicio
    /// porque es una regla del viaje: cualquier cambio de horas, kilómetros o
    /// flete debe reflejarse en la nómina del operador sin que nadie lo invoque.
    /// </summary>
    private void RecalculateDriverPay()
    {
        DriverPayAmount = DriverPayScheme switch
        {
            DriverPayScheme.PerHour => Math.Round(DriverPayRate * (DriverHours ?? 0m), 2, MidpointRounding.AwayFromZero),
            DriverPayScheme.PerKilometer => Math.Round(DriverPayRate * EffectiveDistance, 2, MidpointRounding.AwayFromZero),
            DriverPayScheme.FixedPerTrip => Math.Round(DriverPayRate, 2, MidpointRounding.AwayFromZero),
            DriverPayScheme.PercentageOfRevenue => Math.Round(FreightRevenue * DriverPayRate / 100m, 2, MidpointRounding.AwayFromZero),
            _ => 0m
        };
    }
}
