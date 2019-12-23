using JetBrains.Annotations;
using Serilog;
using Serilog.Events;
using Serilog.Parsing;
using System;
using System.Threading.Tasks;
using WeihanLi.Common.Helpers;

using SSerilog = Serilog;

namespace WeihanLi.Common.Logging.Serilog
{
    internal class SerilogLogHelperProvider : ILogHelperProvider, IDisposable
    {
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

        public Task Log(LogHelperLoggingEvent loggingEvent)
        {
            var logger = SSerilog.Log.ForContext(SourceContextPropName, loggingEvent.CategoryName);
            if (IsEnabled(loggingEvent.LogLevel))
                logger.Write(new LogEvent(loggingEvent.DateTime, GetSerilogEventLevel(loggingEvent.LogLevel), loggingEvent.Exception, new MessageTemplate(loggingEvent.Message, new MessageTemplateToken[0]),
                    new[] { new LogEventProperty(SourceContextPropName, new ScalarValue(loggingEvent.CategoryName)), }));

            return TaskHelper.CompletedTask;
        }

        public bool IsEnabled(LogHelperLevel loggerHelperLevel)
        {
            if (loggerHelperLevel == LogHelperLevel.None)
                return false;

            return SSerilog.Log.IsEnabled(GetSerilogEventLevel(loggerHelperLevel));
        }

        private SSerilog.Events.LogEventLevel GetSerilogEventLevel(LogHelperLevel logHelperLevel)
        {
            switch (logHelperLevel)
            {
                case LogHelperLevel.All:
                    return SSerilog.Events.LogEventLevel.Verbose;

                case LogHelperLevel.Debug:
                    return SSerilog.Events.LogEventLevel.Debug;

                case LogHelperLevel.Info:
                    return SSerilog.Events.LogEventLevel.Information;

                case LogHelperLevel.Trace:
                    return SSerilog.Events.LogEventLevel.Debug;

                case LogHelperLevel.Warn:
                    return SSerilog.Events.LogEventLevel.Warning;

                case LogHelperLevel.Error:
                    return SSerilog.Events.LogEventLevel.Error;

                case LogHelperLevel.Fatal:
                    return SSerilog.Events.LogEventLevel.Fatal;

                case LogHelperLevel.None:
                    return SSerilog.Events.LogEventLevel.Fatal;

                default:
                    return SSerilog.Events.LogEventLevel.Warning;
            }
        }

        private const string SourceContextPropName = "SourceContext";

        public void Log(SSerilog.ILogger logger, LogHelperLevel loggerLevel, Exception exception, string message)
        {
            if (IsEnabled(loggerLevel))
            {
                switch (loggerLevel)
                {
                    case LogHelperLevel.All:
                    case LogHelperLevel.Trace:
                        logger.Verbose(exception, message);
                        break;

                    case LogHelperLevel.Debug:
                        logger.Debug(exception, message);
                        break;

                    case LogHelperLevel.Info:
                        logger.Information(exception, message);
                        break;

                    case LogHelperLevel.Warn:
                        logger.Warning(exception, message);
                        break;

                    case LogHelperLevel.Error:
                        logger.Error(exception, message);
                        break;

                    case LogHelperLevel.Fatal:
                        logger.Fatal(exception, message);
                        break;
                }
            }
        }
    }

    public static class LogHelperFactoryExtensions
    {
        public static ILogHelperFactory AddSerilog([NotNull]this ILogHelperFactory logHelperFactory, Action<LoggerConfiguration> loggerConfigurationAction)
        {
            logHelperFactory.AddProvider(new SerilogLogHelperProvider(loggerConfigurationAction));
            return logHelperFactory;
        }

        public static ILogHelperFactory AddSerilog([NotNull] this ILogHelperFactory logHelperFactory,
            LoggerConfiguration loggerConfiguration)
        {
            logHelperFactory.AddProvider(new SerilogLogHelperProvider(loggerConfiguration));
            return logHelperFactory;
        }
    }
}
