using Serilog;
using Serilog.Events;
using Serilog.Parsing;
using SSerilog = Serilog;

namespace WeihanLi.Common.Logging.Serilog;

internal sealed class SerilogLogHelperProvider : ILogHelperProvider, IDisposable
{
    private static readonly MessageTemplateParser _messageTemplateParser = new();

    public SerilogLogHelperProvider(LoggerConfiguration configuration)
    {
        SerilogHelper.LogInit(configuration);
    }

    public SerilogLogHelperProvider(Action<LoggerConfiguration> configurationAction)
    {
        SerilogHelper.LogInit(configurationAction);
    }

    public void Dispose()
    {
        SSerilog.Log.CloseAndFlush();
    }

    public void Log(LogHelperLoggingEvent loggingEvent)
    {
        var logger = SSerilog.Log.ForContext(SourceContextPropName, loggingEvent.CategoryName);

        var logLevel = GetSerilogEventLevel(loggingEvent.LogLevel);
        if (logger.IsEnabled(logLevel))
        {
            var messageTemplate = loggingEvent.MessageTemplate;
            var properties = new List<LogEventProperty>();
            if (loggingEvent.Properties != null)
            {
                foreach (var property in loggingEvent.Properties)
                {
                    if (logger.BindProperty(property.Key, property.Value, false, out var bound))
                        properties.Add(bound);
                }
            }
            var parsedTemplate = _messageTemplateParser.Parse(messageTemplate ?? "");
            logger.Write(new LogEvent(loggingEvent.DateTime, logLevel, loggingEvent.Exception, parsedTemplate, properties));
        }
    }

    private static LogEventLevel GetSerilogEventLevel(LogHelperLogLevel logHelperLevel)
    {
        return logHelperLevel switch
        {
            LogHelperLogLevel.All => LogEventLevel.Verbose,
            LogHelperLogLevel.Debug => LogEventLevel.Debug,
            LogHelperLogLevel.Info => LogEventLevel.Information,
            LogHelperLogLevel.Trace => LogEventLevel.Debug,
            LogHelperLogLevel.Warn => LogEventLevel.Warning,
            LogHelperLogLevel.Error => LogEventLevel.Error,
            LogHelperLogLevel.Fatal => LogEventLevel.Fatal,
            LogHelperLogLevel.None => LogEventLevel.Fatal,
            _ => LogEventLevel.Warning,
        };
    }

    private const string SourceContextPropName = "SourceContext";
}

public static class LogHelperFactoryExtensions
{
    public static ILogHelperLoggingBuilder AddSerilog(this ILogHelperLoggingBuilder loggingBuilder, Action<LoggerConfiguration> loggerConfigurationAction)
    {
        loggingBuilder.AddProvider(new SerilogLogHelperProvider(loggerConfigurationAction));
        return loggingBuilder;
    }

    public static ILogHelperLoggingBuilder AddSerilog(this ILogHelperLoggingBuilder loggingBuilder,
        LoggerConfiguration loggerConfiguration)
    {
        loggingBuilder.AddProvider(new SerilogLogHelperProvider(loggerConfiguration));
        return loggingBuilder;
    }
}
