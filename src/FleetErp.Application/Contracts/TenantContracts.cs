using FleetErp.Domain.Entities;
using FleetErp.Domain.Enums;

namespace FleetErp.Application.Contracts;

public sealed record TenantDto(
    Guid Id, string Name, string Slug, string? TaxId, string? ContactEmail, string? Phone,
    bool IsActive, TenantSettingsDto Settings);

/// <summary>
/// Espejo transportable de <see cref="TenantSettings"/>. El portal lo lee una vez
/// al iniciar sesión y con eso rotula unidades, moneda y colores sin recompilar.
/// </summary>
public sealed record TenantSettingsDto(
    string CurrencyCode,
    string CurrencySymbol,
    string TimeZoneId,
    string Locale,
    DistanceUnit DistanceUnit,
    VolumeUnit VolumeUnit,
    WeightUnit WeightUnit,
    DriverPayScheme DefaultDriverPayScheme,
    decimal DefaultDriverPayRate,
    decimal DefaultFuelPricePerUnit,
    string TripFolioPrefix,
    string BrandPrimaryColor,
    string? LogoUrl,
    int LicenseExpiryAlertDays,
    decimal MinAcceptableFuelEfficiency)
{
    public static TenantSettingsDto From(TenantSettings s) => new(
        s.CurrencyCode, s.CurrencySymbol, s.TimeZoneId, s.Locale,
        s.DistanceUnit, s.VolumeUnit, s.WeightUnit,
        s.DefaultDriverPayScheme, s.DefaultDriverPayRate, s.DefaultFuelPricePerUnit,
        s.TripFolioPrefix, s.BrandPrimaryColor, s.LogoUrl,
        s.LicenseExpiryAlertDays, s.MinAcceptableFuelEfficiency);

    public TenantSettings ToEntity() => new()
    {
        CurrencyCode = CurrencyCode,
        CurrencySymbol = CurrencySymbol,
        TimeZoneId = TimeZoneId,
        Locale = Locale,
        DistanceUnit = DistanceUnit,
        VolumeUnit = VolumeUnit,
        WeightUnit = WeightUnit,
        DefaultDriverPayScheme = DefaultDriverPayScheme,
        DefaultDriverPayRate = DefaultDriverPayRate,
        DefaultFuelPricePerUnit = DefaultFuelPricePerUnit,
        TripFolioPrefix = TripFolioPrefix,
        BrandPrimaryColor = BrandPrimaryColor,
        LogoUrl = LogoUrl,
        LicenseExpiryAlertDays = LicenseExpiryAlertDays,
        MinAcceptableFuelEfficiency = MinAcceptableFuelEfficiency
    };
}

public sealed record UpdateTenantRequest(string Name, string? TaxId, string? ContactEmail, string? Phone);
