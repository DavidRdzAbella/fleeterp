namespace FleetErp.Domain.Common;

/// <summary>
/// Bolsa de campos definidos por cada empresa. Es la pieza que permite adaptar
/// el ERP a un cliente nuevo (p. ej. "número de póliza", "tipo de permiso SCT")
/// sin migraciones ni recompilar: se declara un <c>CustomFieldDefinition</c> y el
/// valor vive aquí, en una columna <c>jsonb</c>.
/// </summary>
public sealed class CustomFieldValues
{
    private readonly Dictionary<string, string?> _values;

    public CustomFieldValues() => _values = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

    public CustomFieldValues(IDictionary<string, string?> values) =>
        _values = new Dictionary<string, string?>(values, StringComparer.OrdinalIgnoreCase);

    public IReadOnlyDictionary<string, string?> Values => _values;

    public string? Get(string key) => _values.TryGetValue(key, out var v) ? v : null;

    public void Set(string key, string? value)
    {
        if (string.IsNullOrWhiteSpace(key)) return;
        if (value is null) _values.Remove(key);
        else _values[key.Trim()] = value;
    }

    public void Replace(IDictionary<string, string?>? values)
    {
        _values.Clear();
        if (values is null) return;
        foreach (var (k, v) in values) Set(k, v);
    }

    public CustomFieldValues Clone() => new(_values);
}
