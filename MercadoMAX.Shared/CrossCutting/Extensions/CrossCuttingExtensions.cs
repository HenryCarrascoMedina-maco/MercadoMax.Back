using MercadoMAX.Shared.CrossCutting.Data;
using MercadoMAX.Shared.CrossCutting.Logging;
using MercadoMAX.Shared.CrossCutting.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MercadoMAX.Shared.CrossCutting.Extensions;

/// <summary>
/// Registro y wiring del cross-cutting de MercadoMAX (Fase 0).
/// ADITIVO: no modifica controllers, services, repositories ni SPs.
/// </summary>
public static class CrossCuttingExtensions
{
    /// <summary>
    /// Registra las opciones de logging del cross-cutting. Enlaza la sección
    /// "MercadoMaxLogging" si existe; si no, aplica los valores por defecto.
    /// </summary>
    public static IServiceCollection AddMercadoMaxCrossCutting(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<LoggingOptions>(configuration.GetSection(LoggingOptions.SectionName));
        // Ejecutor de SPs (Dapper). Aditivo: solo lo usan los repos que lo inyecten.
        services.AddScoped<ISpExecutor, SpExecutor>();
        return services;
    }

    /// <summary>
    /// Inserta los middlewares transversales en el pipeline, en orden:
    /// correlation-id → request-logging → performance → global-exception.
    /// Debe llamarse ANTES de UseAuthentication/UseAuthorization para envolver todo el pipeline.
    /// </summary>
    public static IApplicationBuilder UseMercadoMaxCrossCutting(this IApplicationBuilder app)
    {
        app.UseMiddleware<CorrelationIdMiddleware>();
        app.UseMiddleware<RequestLoggingMiddleware>();
        app.UseMiddleware<PerformanceMiddleware>();
        app.UseMiddleware<GlobalExceptionMiddleware>();
        return app;
    }
}
