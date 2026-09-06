namespace MercadoMAX.Shared.CrossCutting.Responses;

/// <summary>
/// Envoltorio estándar de error devuelto por el middleware global de excepciones.
/// </summary>
public class ErrorResponse
{
    public bool Success { get; set; } = false;
    public string Message { get; set; } = string.Empty;
    public int StatusCode { get; set; }
    public string? ErrorCode { get; set; }
    public string? CorrelationId { get; set; }

    /// <summary>Detalle técnico; solo se rellena en entornos de desarrollo.</summary>
    public string? Detail { get; set; }
}

/// <summary>
/// Respuesta de error que desglosa las fallas de validación por campo.
/// </summary>
public class ValidationErrorResponse : ErrorResponse
{
    public IDictionary<string, string[]> Errors { get; set; } =
        new Dictionary<string, string[]>();
}
