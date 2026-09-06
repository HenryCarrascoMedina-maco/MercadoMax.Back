using MercadoMAX.Shared.CrossCutting.Constants;
using Microsoft.AspNetCore.Http;

namespace MercadoMAX.Shared.CrossCutting.Middlewares;

/// <summary>
/// Garantiza que cada request tenga un correlation id: lee la cabecera
/// <c>X-Correlation-Id</c> entrante o genera uno nuevo, lo guarda en
/// <see cref="HttpContext.Items"/> y lo devuelve en la respuesta.
/// </summary>
public class CorrelationIdMiddleware
{
    public const string ItemsKey = "CorrelationId";

    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers[HttpConstants.CorrelationIdHeader].FirstOrDefault();

        if (string.IsNullOrWhiteSpace(correlationId))
            correlationId = Guid.NewGuid().ToString("N");

        context.Items[ItemsKey] = correlationId;

        context.Response.OnStarting(() =>
        {
            context.Response.Headers[HttpConstants.CorrelationIdHeader] = correlationId;
            return Task.CompletedTask;
        });

        await _next(context);
    }
}

/// <summary>Acceso conveniente al correlation id del request actual.</summary>
public static class CorrelationIdAccessor
{
    public static string? GetCorrelationId(this HttpContext context) =>
        context.Items.TryGetValue(CorrelationIdMiddleware.ItemsKey, out var value)
            ? value as string
            : null;
}
