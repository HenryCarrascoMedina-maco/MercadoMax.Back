namespace MercadoMAX.Shared.CrossCutting.Constants;

/// <summary>
/// Mensajes de error centralizados (en español, consistentes con la app).
/// </summary>
public static class ErrorMessages
{
    public const string Unexpected = "Ocurrió un error inesperado. Intente nuevamente más tarde.";
    public const string NotFound = "No se encontró el recurso solicitado.";
    public const string Unauthorized = "No está autenticado.";
    public const string Forbidden = "No tiene permisos para realizar esta acción.";
    public const string ValidationFailed = "Se produjeron uno o más errores de validación.";
    public const string Conflict = "El recurso ya existe o entra en conflicto con datos existentes.";
    public const string BusinessRule = "La operación viola una regla de negocio.";

    public static string NotFoundWith(string entity, object key) =>
        $"No se encontró {entity} con identificador '{key}'.";
}
