namespace FleetErp.Application.Common;

/// <summary>Página de resultados. Todo listado del ERP la usa para no traer tablas completas.</summary>
public sealed record PagedResult<T>(IReadOnlyList<T> Items, int Page, int PageSize, int TotalCount)
{
    public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPrevious => Page > 1;
    public bool HasNext => Page < TotalPages;

    public static PagedResult<T> Empty(int page, int pageSize) => new([], page, pageSize, 0);
}

/// <summary>Parámetros de paginación normalizados y acotados.</summary>
public sealed record PageQuery
{
    private const int MaxPageSize = 200;

    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;

    public int SafePage => Page < 1 ? 1 : Page;
    public int SafePageSize => PageSize switch { < 1 => 20, > MaxPageSize => MaxPageSize, _ => PageSize };
    public int Skip => (SafePage - 1) * SafePageSize;
}
