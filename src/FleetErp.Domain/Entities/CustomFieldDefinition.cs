using FleetErp.Domain.Common;
using FleetErp.Domain.Enums;

namespace FleetErp.Domain.Entities;

/// <summary>
/// Declaración de un campo extra que una empresa necesita capturar y el producto
/// base no trae. El formulario del portal lo dibuja solo a partir de esta
/// definición, así que adaptar el ERP a un cliente nuevo no requiere desarrollo.
/// </summary>
public class CustomFieldDefinition : TenantEntity, ISoftDeletable
{
    private CustomFieldDefinition() { }

    public CustomFieldDefinition(CustomFieldTarget target, string key, string label, CustomFieldType type,
                                 bool isRequired = false, string? options = null, int displayOrder = 0)
    {
        Target = target;
        Update(key, label, type, isRequired, options, displayOrder);
    }

    public CustomFieldTarget Target { get; private set; }

    /// <summary>Llave técnica con la que se guarda el valor dentro del <c>jsonb</c>.</summary>
    public string Key { get; private set; } = string.Empty;

    /// <summary>Etiqueta que ve el usuario.</summary>
    public string Label { get; private set; } = string.Empty;

    public CustomFieldType Type { get; private set; }
    public bool IsRequired { get; private set; }

    /// <summary>Opciones separadas por <c>|</c> cuando el tipo es <see cref="CustomFieldType.Select"/>.</summary>
    public string? Options { get; private set; }

    public int DisplayOrder { get; private set; }
    public bool IsActive { get; private set; } = true;

    public IReadOnlyList<string> OptionList =>
        string.IsNullOrWhiteSpace(Options)
            ? []
            : Options.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    public void Update(string key, string label, CustomFieldType type, bool isRequired, string? options, int displayOrder)
    {
        DomainException.Require(!string.IsNullOrWhiteSpace(key), "La llave del campo es obligatoria.");
        DomainException.Require(!string.IsNullOrWhiteSpace(label), "La etiqueta del campo es obligatoria.");
        DomainException.Require(type != CustomFieldType.Select || !string.IsNullOrWhiteSpace(options),
            "Un campo de tipo lista requiere sus opciones.");

        Key = key.Trim().Replace(' ', '_').ToLowerInvariant();
        Label = label.Trim();
        Type = type;
        IsRequired = isRequired;
        Options = options?.Trim();
        DisplayOrder = displayOrder;
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}
