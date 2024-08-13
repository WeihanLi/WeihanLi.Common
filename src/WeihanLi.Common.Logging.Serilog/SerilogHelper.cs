using Serilog;

namespace WeihanLi.Common.Logging.Serilog;

public static class SerilogHelper
{
    private static readonly object Locker = new();

    public static void LogInit(Action<LoggerConfiguration> configureAction)
    {
        Guard.NotNull(configureAction);
        var loggerConfiguration = new LoggerConfiguration();
        loggerConfiguration.Enrich.FromLogContext();
        configureAction(loggerConfiguration);
        LogInit(loggerConfiguration);
    }

    public static void LogInit(LoggerConfiguration loggerConfiguration)
    {
        lock (Locker)
        {
            Log.Logger = loggerConfiguration.CreateLogger();
        }
    }
}
