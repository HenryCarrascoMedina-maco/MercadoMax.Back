namespace MercadoMAX.Shared.CrossCutting.Logging;

/// <summary>Nombres de eventos de log centralizados.</summary>
public static class LogConstants
{
    public static class Events
    {
        public const string RequestCompleted = "RequestCompleted";
        public const string SlowRequest = "SlowRequest";
        public const string UnhandledException = "UnhandledException";
    }
}
