using FleetErp.Web.Models;

namespace FleetErp.Web.Services;

/// <summary>
/// Formato y etiquetas para las vistas. Centralizarlo evita que cada plantilla
/// invente su propia forma de escribir "En ruta" o de redondear un importe, y es
/// donde se respetan las unidades que cada empresa configuró.
/// </summary>
public static class Display
{
    /// <summary>Marca de dato ausente. Una ficha nunca debe mostrar un hueco en blanco.</summary>
    public const string Missing = "—";

    public static string Or(this string? value, string fallback = Missing) =>
        string.IsNullOrWhiteSpace(value) ? fallback : value;

    public static string Money(decimal value, TenantSettingsModel settings) =>
        $"{settings.CurrencySymbol}{value:N2}";

    public static string MoneyShort(decimal value, TenantSettingsModel settings) => value switch
    {
        >= 1_000_000 or <= -1_000_000 => $"{settings.CurrencySymbol}{value / 1_000_000:N1} M",
        >= 1_000 or <= -1_000 => $"{settings.CurrencySymbol}{value / 1_000:N1} k",
        _ => $"{settings.CurrencySymbol}{value:N0}"
    };

    public static string DistanceUnitLabel(TenantSettingsModel settings) =>
        settings.DistanceUnit == DistanceUnit.Mile ? "mi" : "km";

    public static string VolumeUnitLabel(TenantSettingsModel settings) =>
        settings.VolumeUnit == VolumeUnit.Gallon ? "gal" : "L";

    public static string Distance(decimal value, TenantSettingsModel settings) =>
        $"{value:N0} {DistanceUnitLabel(settings)}";

    public static string Volume(decimal value, TenantSettingsModel settings) =>
        $"{value:N0} {VolumeUnitLabel(settings)}";

    public static string Efficiency(decimal value, TenantSettingsModel settings) =>
        value <= 0 ? "—" : $"{value:N2} {DistanceUnitLabel(settings)}/{VolumeUnitLabel(settings)}";

    public static string Weight(decimal value, WeightUnit unit) => unit switch
    {
        WeightUnit.Tonne => $"{value:N2} t",
        WeightUnit.Pound => $"{value:N0} lb",
        _ => $"{value:N0} kg"
    };

    public static string Percent(decimal value) => $"{value:N1} %";

    /// <summary>Convierte un instante UTC a la zona horaria de la empresa.</summary>
    public static DateTimeOffset ToLocal(DateTimeOffset utc, TenantSettingsModel settings)
    {
        try
        {
            var zone = TimeZoneInfo.FindSystemTimeZoneById(settings.TimeZoneId);
            return TimeZoneInfo.ConvertTime(utc, zone);
        }
        catch (Exception e) when (e is TimeZoneNotFoundException or InvalidTimeZoneException)
        {
            // Un identificador IANA puede no existir en Windows y viceversa: antes
            // de romper la pantalla, se muestra la hora universal.
            return utc;
        }
    }

    public static string DateTimeLabel(DateTimeOffset? utc, TenantSettingsModel settings) =>
        utc is null ? "—" : ToLocal(utc.Value, settings).ToString("dd/MM/yy HH:mm");

    public static string TimeLabel(DateTimeOffset? utc, TenantSettingsModel settings) =>
        utc is null ? "—" : ToLocal(utc.Value, settings).ToString("HH:mm");

    public static string DateLabel(DateTimeOffset? utc, TenantSettingsModel settings) =>
        utc is null ? "—" : ToLocal(utc.Value, settings).ToString("dd/MM/yyyy");

    public static string TripStatusLabel(TripStatus status) => status switch
    {
        TripStatus.Planned => "Planeado",
        TripStatus.InProgress => "En ruta",
        TripStatus.Completed => "Concluido",
        TripStatus.Cancelled => "Cancelado",
        _ => status.ToString()
    };

    public static string TripStatusTone(TripStatus status) => status switch
    {
        TripStatus.Planned => "planned",
        TripStatus.InProgress => "active",
        TripStatus.Completed => "done",
        _ => "void"
    };

    public static string VehicleStatusLabel(VehicleStatus status) => status switch
    {
        VehicleStatus.Available => "Disponible",
        VehicleStatus.OnTrip => "En viaje",
        VehicleStatus.InMaintenance => "En taller",
        VehicleStatus.OutOfService => "Fuera de servicio",
        _ => status.ToString()
    };

    public static string VehicleStatusTone(VehicleStatus status) => status switch
    {
        VehicleStatus.Available => "free",
        VehicleStatus.OnTrip => "active",
        VehicleStatus.InMaintenance => "shop",
        _ => "void"
    };

    public static string DriverStatusLabel(DriverStatus status) => status switch
    {
        DriverStatus.Active => "Disponible",
        DriverStatus.OnTrip => "En viaje",
        DriverStatus.OnLeave => "Incapacidad o permiso",
        DriverStatus.Inactive => "Baja",
        _ => status.ToString()
    };

    public static string DriverStatusTone(DriverStatus status) => status switch
    {
        DriverStatus.Active => "free",
        DriverStatus.OnTrip => "active",
        DriverStatus.OnLeave => "shop",
        _ => "void"
    };

    public static string PaySchemeLabel(DriverPayScheme scheme) => scheme switch
    {
        DriverPayScheme.PerHour => "Por hora",
        DriverPayScheme.PerKilometer => "Por distancia",
        DriverPayScheme.FixedPerTrip => "Fijo por viaje",
        DriverPayScheme.PercentageOfRevenue => "% del flete",
        _ => scheme.ToString()
    };

    public static string PayRateLabel(DriverPayScheme scheme, decimal rate, TenantSettingsModel settings) => scheme switch
    {
        DriverPayScheme.PerHour => $"{Money(rate, settings)} / h",
        DriverPayScheme.PerKilometer => $"{Money(rate, settings)} / {DistanceUnitLabel(settings)}",
        DriverPayScheme.FixedPerTrip => $"{Money(rate, settings)} / viaje",
        DriverPayScheme.PercentageOfRevenue => $"{rate:N1} % del flete",
        _ => Money(rate, settings)
    };

    public static string RoleLabel(UserRole role) => role switch
    {
        UserRole.Administrator => "Administrador",
        UserRole.Dispatcher => "Despachador",
        _ => "Consulta"
    };

    public static string RoleTone(UserRole role) => role switch
    {
        UserRole.Administrator => "done",
        UserRole.Dispatcher => "active",
        _ => "planned"
    };

    public static string CategoryLabel(VehicleCategory category) =>
        category == VehicleCategory.Motorized ? "Motriz" : "Arrastre";

    public static string MaintenanceStatusLabel(MaintenanceStatus status) => status switch
    {
        MaintenanceStatus.Open => "Abierta",
        MaintenanceStatus.InProgress => "En proceso",
        _ => "Cerrada"
    };

    public static string MaintenanceKindLabel(MaintenanceKind kind) =>
        kind == MaintenanceKind.Preventive ? "Preventivo" : "Correctivo";

    public static string RankingCriteriaLabel(DriverRankingCriteria criteria) => criteria switch
    {
        DriverRankingCriteria.Distance => "Distancia recorrida",
        DriverRankingCriteria.Revenue => "Flete vendido",
        DriverRankingCriteria.Profit => "Utilidad generada",
        DriverRankingCriteria.Trips => "Viajes realizados",
        DriverRankingCriteria.FuelEfficiency => "Rendimiento de combustible",
        _ => criteria.ToString()
    };
}
