namespace WeihanLi.Common.Logging;

public interface ILogHelperLogger
{
    void Log(LogHelperLogLevel logLevel, Exception? exception, string? messageTemplate, params object?[] parameters);

    bool IsEnabled(LogHelperLogLevel logLevel);
}

internal sealed class NullLogHelperLogger : ILogHelperLogger
{
    public static readonly ILogHelperLogger Instance = new NullLogHelperLogger();

    private NullLogHelperLogger()
    {
    }

    public void Log(LogHelperLogLevel logLevel, Exception? exception, string? messageTemplate, params object?[] parameters)
    {
        // empty
    }

    public bool IsEnabled(LogHelperLogLevel logLevel) => false;
}

// ReSharper disable once UnusedTypeParameter
public interface ILogHelperLogger<TCategory> : ILogHelperLogger;

internal sealed class LogHelperGenericLogger<TCategory>(LogHelperFactory logHelperFactory) : LogHelper(logHelperFactory, typeof(TCategory).FullName ?? typeof(TCategory).Name), ILogHelperLogger<TCategory>;

internal class LogHelper(LogHelperFactory logHelperFactory, string categoryName) : ILogHelperLogger
{
    public string CategoryName { get; } = categoryName;

    public void Log(LogHelperLogLevel logLevel, Exception? exception, string? messageTemplate, params object?[] parameters)
    {
        if (!IsEnabled(logLevel))
            return;

        var loggingEvent = new LogHelperLoggingEvent()
        {
            CategoryName = CategoryName,
            DateTime = DateTimeOffset.UtcNow,
            Exception = exception,
            LogLevel = logLevel,
            MessageTemplate = messageTemplate ?? string.Empty,
        };

        if (logHelperFactory._logFilters.Count > 0 &&
            !logHelperFactory._logFilters.Any(x => x.Invoke(typeof(int), loggingEvent))
            )
        {
            return;
        }

        var formattedLog = LoggingFormatter.Format(loggingEvent.MessageTemplate, parameters);
        loggingEvent.Message = formattedLog.Msg;
        loggingEvent.Properties = formattedLog.Values;

        foreach (var enricher in logHelperFactory._logHelperEnrichers)
        {
            enricher.Enrich(loggingEvent);
        }

        Parallel.ForEach(logHelperFactory._logHelperProviders, logHelperProvider =>
        {
            if (logHelperFactory._logFilters.Count == 0
                || logHelperFactory._logFilters.All(x => x.Invoke(logHelperProvider.Key, loggingEvent)))
            {
                logHelperProvider.Value.Log(loggingEvent);
            }
        });
    }

    public bool IsEnabled(LogHelperLogLevel logLevel) => logLevel != LogHelperLogLevel.None;
}
