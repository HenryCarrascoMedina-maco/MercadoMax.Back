namespace MercadoMAX.Shared.CrossCutting.Logging;

/// <summary>
/// Configuración del logging de requests y de performance. Se enlaza desde la
/// sección "MercadoMaxLogging" (distinta de la sección "Logging" del framework).
/// Si la sección no existe, aplican estos valores por defecto.
/// </summary>
public class LoggingOptions
{
    public const string SectionName = "MercadoMaxLogging";

    /// <summary>Emitir un log estructurado por cada request completado.</summary>
    public bool EnableRequestLogging { get; set; } = true;

    /// <summary>Emitir un warning cuando un request supere <see cref="SlowRequestThresholdMs"/>.</summary>
    public bool EnablePerformanceLogging { get; set; } = true;

    /// <summary>Umbral (ms) sobre el cual un request se considera lento.</summary>
    public int SlowRequestThresholdMs { get; set; } = 1000;
}
