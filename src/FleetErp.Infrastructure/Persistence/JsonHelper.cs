using System.Text.Json;
using FleetErp.Domain.Common;
using FleetErp.Domain.Entities;

namespace FleetErp.Infrastructure.Persistence;

/// <summary>Serialización compartida por los convertidores de valor del contexto.</summary>
internal static class JsonHelper
{
    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web);

    public static string Serialize(CustomFieldValues? value) =>
        JsonSerializer.Serialize(value?.Values ?? new Dictionary<string, string?>(), Options);

    public static CustomFieldValues Deserialize(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return new CustomFieldValues();
        var dict = JsonSerializer.Deserialize<Dictionary<string, string?>>(json, Options);
        return dict is null ? new CustomFieldValues() : new CustomFieldValues(dict);
    }

    public static string SerializeSettings(TenantSettings settings) => JsonSerializer.Serialize(settings, Options);

    public static TenantSettings DeserializeSettings(string? json) =>
        string.IsNullOrWhiteSpace(json)
            ? TenantSettings.Default()
            : JsonSerializer.Deserialize<TenantSettings>(json, Options) ?? TenantSettings.Default();
}
