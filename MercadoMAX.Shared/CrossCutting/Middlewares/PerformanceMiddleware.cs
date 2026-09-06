using System.Diagnostics;
using MercadoMAX.Shared.CrossCutting.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MercadoMAX.Shared.CrossCutting.Middlewares;

/// <summary>
/// Detecta y registra requests lentos que superan <see cref="LoggingOptions.SlowRequestThresholdMs"/>.
/// Controlado por <see cref="LoggingOptions.EnablePerformanceLogging"/>.
/// </summary>
public class PerformanceMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<PerformanceMiddleware> _logger;
    private readonly LoggingOptions _options;

    public PerformanceMiddleware(
        RequestDelegate next,
        ILogger<PerformanceMiddleware> logger,
        IOptions<LoggingOptions> options)
    {
        _next = next;
        _logger = logger;
        _options = options.Value;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!_options.EnablePerformanceLogging)
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
            if (stopwatch.ElapsedMilliseconds >= _options.SlowRequestThresholdMs)
            {
                _logger.LogWarning(
                    "{Event} {Method} {Path} tardó {ElapsedMs}ms (umbral {ThresholdMs}ms, corr: {CorrelationId})",
                    LogConstants.Events.SlowRequest,
                    context.Request.Method,
                    context.Request.Path.Value,
                    stopwatch.ElapsedMilliseconds,
                    _options.SlowRequestThresholdMs,
                    context.GetCorrelationId());
            }
        }
    }
}
