namespace MercadoMAX.Shared.CrossCutting.Pagination;

/// <summary>Dirección de ordenamiento usada por los listados.</summary>
public enum SortDirection
{
    Ascending,
    Descending
}

/// <summary>Parámetros de ordenamiento.</summary>
public class SortParams
{
    /// <summary>Nombre de propiedad por la cual ordenar (insensible a mayúsculas).</summary>
    public string? SortBy { get; set; }

    public SortDirection SortDirection { get; set; } = SortDirection.Ascending;

    public bool IsDescending => SortDirection == SortDirection.Descending;
}

/// <summary>
/// Parámetros estándar para listados paginados, con búsqueda y ordenamiento.
/// (El contrato de salida sigue siendo el <c>PagedResponse&lt;T&gt;</c> plano de MercadoMAX.)
/// </summary>
public class PaginationParams : SortParams
{
    private const int MaxPageSize = 100;
    private int _pageSize = 10;
    private int _page = 1;

    /// <summary>Número de página (base 1).</summary>
    public int Page
    {
        get => _page;
        set => _page = value < 1 ? 1 : value;
    }

    /// <summary>Tamaño de página, acotado a <see cref="MaxPageSize"/>.</summary>
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value switch
        {
            < 1 => 10,
            > MaxPageSize => MaxPageSize,
            _ => value
        };
    }

    /// <summary>Término de búsqueda global.</summary>
    public string? Search { get; set; }

    public int Skip => (Page - 1) * PageSize;
}

/// <summary>
/// Modelo base de filtro. Los módulos derivan de aquí para agregar propiedades
/// tipadas heredando paginación, búsqueda y ordenamiento.
/// </summary>
public abstract class FilterParams : PaginationParams
{
    /// <summary>Filtro activo/inactivo. Null = todos.</summary>
    public bool? IsActive { get; set; }

    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }
}
