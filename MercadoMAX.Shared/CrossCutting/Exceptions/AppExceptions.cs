using System.Net;
using MercadoMAX.Shared.CrossCutting.Constants;

namespace MercadoMAX.Shared.CrossCutting.Exceptions;

/// <summary>
/// Excepción base de aplicación. Lleva un código HTTP y un código de error opcional
/// para que el middleware global produzca una respuesta de error consistente.
/// </summary>
public class AppException : Exception
{
    public HttpStatusCode StatusCode { get; }
    public string? ErrorCode { get; }

    public AppException(
        string message,
        HttpStatusCode statusCode = HttpStatusCode.BadRequest,
        string? errorCode = null,
        Exception? innerException = null)
        : base(message, innerException)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
    }
}

/// <summary>Regla de negocio violada. HTTP 400.</summary>
public class BusinessException : AppException
{
    public BusinessException(string message = "", string? errorCode = null)
        : base(string.IsNullOrWhiteSpace(message) ? ErrorMessages.BusinessRule : message,
               HttpStatusCode.BadRequest, errorCode ?? "BUSINESS_RULE")
    {
    }
}

/// <summary>Datos duplicados o en conflicto. HTTP 409.</summary>
public class ConflictException : AppException
{
    public ConflictException(string message = "")
        : base(string.IsNullOrWhiteSpace(message) ? ErrorMessages.Conflict : message,
               HttpStatusCode.Conflict, "CONFLICT")
    {
    }
}

/// <summary>Usuario autenticado sin permiso. HTTP 403.</summary>
public class ForbiddenException : AppException
{
    public ForbiddenException(string message = "")
        : base(string.IsNullOrWhiteSpace(message) ? ErrorMessages.Forbidden : message,
               HttpStatusCode.Forbidden, "FORBIDDEN")
    {
    }
}

/// <summary>Recurso inexistente. HTTP 404.</summary>
public class NotFoundException : AppException
{
    public NotFoundException(string message = "")
        : base(string.IsNullOrWhiteSpace(message) ? ErrorMessages.NotFound : message,
               HttpStatusCode.NotFound, "NOT_FOUND")
    {
    }

    public NotFoundException(string entity, object key)
        : base(ErrorMessages.NotFoundWith(entity, key), HttpStatusCode.NotFound, "NOT_FOUND")
    {
    }
}

/// <summary>Usuario no autenticado. HTTP 401.</summary>
public class UnauthorizedException : AppException
{
    public UnauthorizedException(string message = "")
        : base(string.IsNullOrWhiteSpace(message) ? ErrorMessages.Unauthorized : message,
               HttpStatusCode.Unauthorized, "UNAUTHORIZED")
    {
    }
}

/// <summary>Falla de validación de entrada con mapa por campo. HTTP 422.</summary>
public class ValidationException : AppException
{
    public IDictionary<string, string[]> Errors { get; }

    public ValidationException(IDictionary<string, string[]> errors)
        : base(ErrorMessages.ValidationFailed, HttpStatusCode.UnprocessableEntity, "VALIDATION_ERROR")
    {
        Errors = errors;
    }

    public ValidationException(string field, string error)
        : this(new Dictionary<string, string[]> { [field] = new[] { error } })
    {
    }
}
