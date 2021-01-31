using Microsoft.Extensions.Logging;

namespace WeihanLi.Common.Logging
{
    internal class MicrosoftLoggingLogHelperProvider : ILogHelperProvider
    {
        private readonly ILoggerFactory _loggerFactory;

        public MicrosoftLoggingLogHelperProvider(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

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
            switch (logHelperLevel)
            {
                case LogHelperLogLevel.All:
                    return LogLevel.Debug;

                case LogHelperLogLevel.Info:
                    return LogLevel.Information;

                case LogHelperLogLevel.Debug:
                    return LogLevel.Debug;

                case LogHelperLogLevel.Trace:
                    return LogLevel.Trace;

                case LogHelperLogLevel.Warn:
                    return LogLevel.Warning;

                case LogHelperLogLevel.Error:
                    return LogLevel.Error;

                case LogHelperLogLevel.Fatal:
                    return LogLevel.Critical;

                case LogHelperLogLevel.None:
                    return LogLevel.None;

                default:
                    return LogLevel.Warning;
            }
        }
    }

    internal static class MicrosoftLoggingExtensions
    {
        internal static void AddMicrosoftLogging(this ILogHelperLoggingBuilder logHelperFactory, ILoggerFactory loggerFactory)
        {
            logHelperFactory.AddProvider(new MicrosoftLoggingLogHelperProvider(loggerFactory));
        }
    }
}
