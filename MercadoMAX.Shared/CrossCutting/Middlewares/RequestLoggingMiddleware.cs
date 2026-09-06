using System.Diagnostics;
using MercadoMAX.Shared.CrossCutting.Extensions;
using MercadoMAX.Shared.CrossCutting.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MercadoMAX.Shared.CrossCutting.Middlewares;

/// <summary>
/// Registra cada request completado (método, ruta, status, duración, usuario, correlation id)
/// como log estructurado. Controlado por <see cref="LoggingOptions.EnableRequestLogging"/>.
/// </summary>
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;
    private readonly LoggingOptions _options;

    public RequestLoggingMiddleware(
        RequestDelegate next,
        ILogger<RequestLoggingMiddleware> logger,
        IOptions<LoggingOptions> options)
    {
        _next = next;
        _logger = logger;
        _options = options.Value;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!_options.EnableRequestLogging)
        {
            await _next(context);
            return;
        }

        var stopwatch = Stopwatch.StartNew();
        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();
            _logger.LogInformation(
                "{Event} {Method} {Path} -> {StatusCode} en {ElapsedMs}ms (user: {UserId}, corr: {CorrelationId})",
                LogConstants.Events.RequestCompleted,
                context.Request.Method,
                context.Request.Path.Value,
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds,
                context.User.GetUserId(),
                context.GetCorrelationId());
        }
    }
}
