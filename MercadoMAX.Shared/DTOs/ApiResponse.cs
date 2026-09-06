namespace MercadoMAX.Shared.DTOs;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }

    /// <summary>Correlation id del request (opcional; lo rellena el cross-cutting). Aditivo y no rompe clientes existentes.</summary>
    public string? CorrelationId { get; set; }

    public static ApiResponse<T> Ok(T data, string message = "OK")
        => new() { Success = true, Message = message, Data = data };

    public static ApiResponse<T> Fail(string message)
        => new() { Success = false, Message = message };

    /// <summary>
    /// Construye un <see cref="ApiResponse{T}"/> a partir de un <see cref="SpResult"/>
    /// (convención de SPs: Success = 1/0, Message, Id). Útil para eliminar el ternario repetido en los services.
    /// </summary>
    public static ApiResponse<T> FromSpResult(SpResult sp, T? data = default)
        => new() { Success = sp.Success == 1, Message = sp.Message, Data = data };
}

public class PagedResponse<T>
{
    public bool Success { get; set; } = true;
    public string? Message { get; set; }
    public List<T> Data { get; set; } = [];
    public int TotalRecords { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => PageSize == 0 ? 0 : (int)Math.Ceiling((double)TotalRecords / PageSize);

    /// <summary>Correlation id del request (opcional). Aditivo.</summary>
    public string? CorrelationId { get; set; }

    /// <summary>Helper para construir una respuesta paginada (forma plana de MercadoMAX).</summary>
    public static PagedResponse<T> Create(IEnumerable<T> items, int totalRecords, int pageNumber, int pageSize)
        => new()
        {
            Success = true,
            Data = items.ToList(),
            TotalRecords = totalRecords,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
}
