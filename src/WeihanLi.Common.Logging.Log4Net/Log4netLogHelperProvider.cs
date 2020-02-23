using JetBrains.Annotations;
using log4net;
using log4net.Core;
using WeihanLi.Common.Helpers;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Logging.Log4Net
{
    internal class Log4NetLogHelperProvider : ILogHelperProvider
    {
        public Log4NetLogHelperProvider() : this(ApplicationHelper.MapPath("log4net.config"))
        {
        }

        public Log4NetLogHelperProvider(string configurationFilePath) => Log4NetHelper.LogInit(configurationFilePath);

        public void Log(LogHelperLoggingEvent loggingEvent)
        {
            var logger = LogManager.GetLogger(ApplicationHelper.ApplicationName, loggingEvent.CategoryName);
            if (IsEnabled(loggingEvent.LogLevel, logger))
            {
                if (loggingEvent.Message.IsNotNullOrEmpty() || loggingEvent.Exception != null)
                {
                    var logLevel = GetLog4NetLogLevel(loggingEvent.LogLevel);
                    var log4netEvent = new LoggingEvent(null, LogManager.GetRepository(ApplicationHelper.ApplicationName), loggingEvent.CategoryName, logLevel, loggingEvent.Message, loggingEvent.Exception);

                    if (loggingEvent.Properties != null)
                    {
                        foreach (var property in loggingEvent.Properties)
                        {
                            if (!log4netEvent.Properties.Contains(property.Key))
                            {
                                log4netEvent.Properties[property.Key] = property.Value;
                            }
                        }
                    }
                    logger.Logger.Log(log4netEvent);
                }
            }
        }

        private static Level GetLog4NetLogLevel(LogHelperLogLevel logHelperLevel)
        {
            switch (logHelperLevel)
            {
                case LogHelperLogLevel.Info:
                    return Level.Info;

                case LogHelperLogLevel.Debug:
                    return Level.Debug;

                case LogHelperLogLevel.Trace:
                    return Level.Trace;

                case LogHelperLogLevel.Warn:
                    return Level.Warn;

                case LogHelperLogLevel.Error:
                    return Level.Error;

                case LogHelperLogLevel.Fatal:
                    return Level.Fatal;

                default:
                    return Level.Alert;
            }
        }

        private static bool IsEnabled(LogHelperLogLevel loggerHelperLevel, ILog logger)
        {
            switch (loggerHelperLevel)
            {
                case LogHelperLogLevel.Info:
                    return logger.IsInfoEnabled;

                case LogHelperLogLevel.Debug:
                case LogHelperLogLevel.Trace:
                    return logger.IsDebugEnabled;

                case LogHelperLogLevel.Warn:
                    return logger.IsWarnEnabled;

                case LogHelperLogLevel.Error:
                    return logger.IsErrorEnabled;

                case LogHelperLogLevel.Fatal:
                    return logger.IsFatalEnabled;

                default:
                    return false;
            }
        }
    }

    public static class LogHelperFactoryExtensions
    {
        public static ILogHelperLoggingBuilder AddLog4Net([NotNull]this ILogHelperLoggingBuilder loggingBuilder)
        {
            loggingBuilder.AddProvider(new Log4NetLogHelperProvider());
            return loggingBuilder;
        }

        public static ILogHelperLoggingBuilder AddLog4Net([NotNull] this ILogHelperLoggingBuilder loggingBuilder,
            string configFilePath)
        {
            loggingBuilder.AddProvider(new Log4NetLogHelperProvider(configFilePath));
            return loggingBuilder;
        }
    }
}
