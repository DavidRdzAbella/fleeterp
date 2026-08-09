namespace FleetErp.Web.ViewModels;

/// <summary>En qué está el panel de la derecha en este momento.</summary>
public enum WorkbenchMode
{
    /// <summary>No hay nada seleccionado: se invita a elegir de la lista o a dar de alta.</summary>
    Empty = 0,
    /// <summary>Ficha en lectura.</summary>
    View = 1,
    /// <summary>Ficha abierta para modificar.</summary>
    Edit = 2,
    /// <summary>Formulario en blanco para un registro nuevo.</summary>
    New = 3
}

/// <summary>
/// Renglón de la lista maestra. Se queda en cuatro datos a propósito: nombre,
/// una línea de apoyo, un dato duro alineado a la derecha y una etiqueta de
/// estado. Más que eso convierte la lista en la tabla que se quería evitar.
/// </summary>
public sealed record WorkbenchItem(
    Guid Id,
    string Title,
    string? Subtitle = null,
    string? Meta = null,
    string? Badge = null,
    string? BadgeTone = null,
    bool IsMuted = false);

/// <summary>
/// Estado del panel maestro: qué se busca, qué se encontró y qué está
/// seleccionado. La vista de cada módulo lo llena y el partial lo dibuja igual
/// en todos, que es lo que hace que el portal se sienta de una sola pieza.
/// </summary>
public sealed class WorkbenchList
{
    public required string SearchPlaceholder { get; init; }
    public string? Search { get; init; }
    public required IReadOnlyList<WorkbenchItem> Items { get; init; }
    public Guid? SelectedId { get; init; }
    public int TotalCount { get; init; }

    /// <summary>Qué decir cuando la búsqueda no devuelve nada.</summary>
    public string EmptyMessage { get; init; } = "No hay registros que coincidan con la búsqueda.";

    /// <summary>
    /// Filtros vigentes que deben sobrevivir al seleccionar un elemento. Sin
    /// esto, hacer clic en un renglón reiniciaría la búsqueda del usuario.
    /// </summary>
    public Dictionary<string, string?> Filters { get; init; } = [];
}

/// <summary>
/// Botonera de la ficha. Muestra solo lo que aplica al modo actual: en lectura
/// se puede editar o dar de baja; en captura, guardar o cancelar.
/// </summary>
public sealed class WorkbenchToolbar
{
    public required WorkbenchMode Mode { get; init; }
    public Guid? SelectedId { get; init; }
    public bool CanWrite { get; init; }

    /// <summary>Identificador del formulario que envía el botón Guardar.</summary>
    public string FormId { get; init; } = "detail-form";

    public bool CanDelete { get; init; } = true;
    public string DeleteLabel { get; init; } = "Dar de baja";
    public string DeleteAction { get; init; } = "Delete";
    public string DeleteConfirm { get; init; } = "¿Confirma dar de baja este registro?";

    /// <summary>Filtros a conservar al cancelar y volver a la lectura.</summary>
    public Dictionary<string, string?> Filters { get; init; } = [];
}
