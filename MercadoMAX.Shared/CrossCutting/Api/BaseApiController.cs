using MercadoMAX.Shared.CrossCutting.Middlewares;
using MercadoMAX.Shared.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MercadoMAX.Shared.CrossCutting.Api;

/// <summary>
/// Controlador base para los APIs de MercadoMAX. Centraliza el envoltorio de respuesta
/// (<see cref="ApiResponse{T}"/> plano) y estampa el CorrelationId del request.
/// El mapeo de errores NO se hace aquí: las excepciones (NotFound/Conflict/Business/...)
/// las traduce el GlobalExceptionMiddleware. Reutilizable por todos los maestros.
/// </summary>
[ApiController]
public abstract class BaseApiController : ControllerBase
{
    /// <summary>200 OK con el envelope estándar (estampa CorrelationId).</summary>
    protected IActionResult OkResponse<T>(ApiResponse<T> response)
    {
        response.CorrelationId = HttpContext.GetCorrelationId();
        return Ok(response);
    }

    /// <summary>201 Created con el envelope estándar (estampa CorrelationId).</summary>
    protected IActionResult CreatedResponse<T>(ApiResponse<T> response)
    {
        response.CorrelationId = HttpContext.GetCorrelationId();
        return StatusCode(StatusCodes.Status201Created, response);
    }
}
