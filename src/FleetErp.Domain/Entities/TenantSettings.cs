using FleetErp.Domain.Enums;

namespace FleetErp.Domain.Entities;

/// <summary>
/// Toda la variabilidad esperable entre transportistas expresada como datos y no
/// como código: unidades de medida, moneda, tarifas por defecto, marca visual y
/// umbrales de alerta. Implantar en una empresa nueva es llenar esta tabla.
/// </summary>
public sealed class TenantSettings
{
    public string CurrencyCode { get; set; } = "MXN";
    public string CurrencySymbol { get; set; } = "$";
    public string TimeZoneId { get; set; } = "America/Mexico_City";
    public string Locale { get; set; } = "es-MX";

    public DistanceUnit DistanceUnit { get; set; } = DistanceUnit.Kilometer;
    public VolumeUnit VolumeUnit { get; set; } = VolumeUnit.Liter;
    public WeightUnit WeightUnit { get; set; } = WeightUnit.Kilogram;

    /// <summary>Valores precargados en el alta de viaje para no recapturarlos.</summary>
    public DriverPayScheme DefaultDriverPayScheme { get; set; } = DriverPayScheme.PerHour;
    public decimal DefaultDriverPayRate { get; set; } = 90m;
    public decimal DefaultFuelPricePerUnit { get; set; } = 25.50m;

    /// <summary>Plantilla del folio de viaje. <c>{seq}</c> se sustituye por el consecutivo.</summary>
    public string TripFolioPrefix { get; set; } = "VJ";

    /// <summary>Marca visual del portal para el cliente en turno.</summary>
    public string BrandPrimaryColor { get; set; } = "#0E7C66";
    public string? LogoUrl { get; set; }

    /// <summary>Días de anticipación con que se avisa el vencimiento de licencias.</summary>
    public int LicenseExpiryAlertDays { get; set; } = 30;

    /// <summary>Rendimiento mínimo aceptable (distancia por unidad de volumen) antes de marcar el viaje.</summary>
    public decimal MinAcceptableFuelEfficiency { get; set; } = 2.0m;

    public static TenantSettings Default() => new();
}
