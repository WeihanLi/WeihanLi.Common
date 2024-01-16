using Microsoft.Extensions.Logging;

namespace WeihanLi.Common.Logging;

internal class MicrosoftLoggingLogHelperProvider(ILoggerFactory loggerFactory) : ILogHelperProvider
{
    private readonly ILoggerFactory _loggerFactory = loggerFactory;

    public void Log(LogHelperLoggingEvent loggingEvent)
    {
        var logger = _loggerFactory.CreateLogger(loggingEvent.CategoryName);
        _ = LogInternal(logger, loggingEvent);
    }

    private static bool LogInternal(ILogger logger, LogHelperLoggingEvent loggingEvent)
    {
        var logLevel = ConvertLogLevel(loggingEvent.LogLevel);
        if (!logger.IsEnabled(logLevel))
        {
            return false;
        }
        var logged = false;
        switch (loggingEvent.LogLevel)
        {
            case LogHelperLogLevel.Debug:
                logger.LogDebug(loggingEvent.Exception, loggingEvent.Message);
                logged = true;
                break;

            case LogHelperLogLevel.Trace:
                logger.LogTrace(loggingEvent.Exception, loggingEvent.Message);
                logged = true;
                break;

            case LogHelperLogLevel.Info:
                logger.LogInformation(loggingEvent.Exception, loggingEvent.Message);
                logged = true;
                break;

            case LogHelperLogLevel.Warn:
                logger.LogWarning(loggingEvent.Exception, loggingEvent.Message);
                logged = true;
                break;

            case LogHelperLogLevel.Error:
                logger.LogError(loggingEvent.Exception, loggingEvent.Message);
                logged = true;
                break;

            case LogHelperLogLevel.Fatal:
                logger.LogCritical(loggingEvent.Exception, loggingEvent.Message);
                logged = true;
                break;
        }

        return logged;
    }

    private static LogLevel ConvertLogLevel(LogHelperLogLevel logHelperLevel)
    {
        return logHelperLevel switch
        {
            LogHelperLogLevel.All => LogLevel.Debug,
            LogHelperLogLevel.Info => LogLevel.Information,
            LogHelperLogLevel.Debug => LogLevel.Debug,
            LogHelperLogLevel.Trace => LogLevel.Trace,
            LogHelperLogLevel.Warn => LogLevel.Warning,
            LogHelperLogLevel.Error => LogLevel.Error,
            LogHelperLogLevel.Fatal => LogLevel.Critical,
            LogHelperLogLevel.None => LogLevel.None,
            _ => LogLevel.Warning,
        };
    }
}

internal static class MicrosoftLoggingExtensions
{
    internal static void AddMicrosoftLogging(this ILogHelperLoggingBuilder logHelperFactory, ILoggerFactory loggerFactory)
    {
        logHelperFactory.AddProvider(new MicrosoftLoggingLogHelperProvider(loggerFactory));
    }
}
