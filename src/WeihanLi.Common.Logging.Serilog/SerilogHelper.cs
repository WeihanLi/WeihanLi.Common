using Serilog;

namespace WeihanLi.Common.Logging.Serilog;

public static class SerilogHelper
{
    private static readonly object _locker = new object();

    public static void LogInit(Action<LoggerConfiguration> configAction)
    {
        var loggerConfiguration = new LoggerConfiguration();
        loggerConfiguration.Enrich.FromLogContext();
        configAction?.Invoke(loggerConfiguration);
        LogInit(loggerConfiguration);
    }

    public static void LogInit(LoggerConfiguration loggerConfiguration)
    {
        lock (_locker)
        {
            Log.Logger = loggerConfiguration.CreateLogger();
        }
    }
}
