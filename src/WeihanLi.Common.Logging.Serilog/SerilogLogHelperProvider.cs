using System;
using System.Collections.Concurrent;
using JetBrains.Annotations;
using Serilog;

using SSerilog = Serilog;

namespace WeihanLi.Common.Logging.Serilog
{
    public class SerilogLogHelperProvider : ILogHelperProvider, IDisposable
    {
        private readonly ConcurrentDictionary<int, ILogHelperLogger> _loggers = new ConcurrentDictionary<int, ILogHelperLogger>();

        public ILogHelperLogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(1, t => new SerilogLogHelperLogger());
        }

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
            Log.CloseAndFlush();
        }
    }

    internal class SerilogLogHelperLogger : ILogHelperLogger
    {
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

        public void Log(LogHelperLevel loggerLevel, Exception exception, string message)
        {
            if (IsEnabled(loggerLevel))
            {
                switch (loggerLevel)
                {
                    case LogHelperLevel.All:
                    case LogHelperLevel.Trace:
                        SSerilog.Log.Verbose(exception, message);
                        break;

                    case LogHelperLevel.Debug:
                        SSerilog.Log.Debug(exception, message);
                        break;

                    case LogHelperLevel.Info:
                        SSerilog.Log.Information(exception, message);
                        break;

                    case LogHelperLevel.Warn:
                        SSerilog.Log.Warning(exception, message);
                        break;

                    case LogHelperLevel.Error:
                        SSerilog.Log.Error(exception, message);
                        break;

                    case LogHelperLevel.Fatal:
                        SSerilog.Log.Fatal(exception, message);
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
