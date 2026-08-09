namespace FleetErp.Domain.Enums;

/// <summary>Ciclo de vida de un viaje. El orden es la máquina de estados.</summary>
public enum TripStatus
{
    Planned = 0,
    InProgress = 1,
    Completed = 2,
    Cancelled = 3
}

public enum VehicleStatus
{
    Available = 0,
    OnTrip = 1,
    InMaintenance = 2,
    OutOfService = 3
}

/// <summary>
/// Distingue lo que se conduce de lo que se arrastra. Es lo que permite modelar
/// tractocamión + caja/remolque (y también rabones, dollys, pipas…) con una sola
/// tabla de unidades, sin código específico por empresa.
/// </summary>
public enum VehicleCategory
{
    /// <summary>Unidad motriz: tractocamión, rabón, torton, camioneta.</summary>
    Motorized = 0,
    /// <summary>Unidad de arrastre: caja seca, refrigerada, plataforma, pipa, dolly.</summary>
    Towed = 1
}

public enum DriverStatus
{
    Active = 0,
    OnTrip = 1,
    OnLeave = 2,
    Inactive = 3
}

/// <summary>Esquema con el que se le paga al operador en un viaje.</summary>
public enum DriverPayScheme
{
    /// <summary>Tarifa por hora trabajada (el caso que pidió el cliente).</summary>
    PerHour = 0,
    PerKilometer = 1,
    FixedPerTrip = 2,
    /// <summary>Porcentaje sobre el flete cobrado.</summary>
    PercentageOfRevenue = 3
}

public enum WeightUnit { Kilogram = 0, Tonne = 1, Pound = 2 }

public enum DistanceUnit { Kilometer = 0, Mile = 1 }

public enum VolumeUnit { Liter = 0, Gallon = 1 }

public enum MaintenanceKind { Preventive = 0, Corrective = 1 }

public enum MaintenanceStatus { Open = 0, InProgress = 1, Closed = 2 }

public enum UserRole
{
    /// <summary>Alta de empresas y parametrización global.</summary>
    Administrator = 0,
    /// <summary>Opera viajes: despacha, cierra, captura gastos.</summary>
    Dispatcher = 1,
    /// <summary>Solo lectura de tableros y reportes.</summary>
    Viewer = 2
}

/// <summary>Tipo de dato de un campo configurable por empresa.</summary>
public enum CustomFieldType { Text = 0, Number = 1, Date = 2, Boolean = 3, Select = 4 }

/// <summary>Entidad a la que se le adjunta un campo configurable.</summary>
public enum CustomFieldTarget { Trip = 0, Vehicle = 1, Driver = 2, Customer = 3 }
