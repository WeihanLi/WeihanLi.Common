using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using WeihanLi.Common.Helpers;

namespace WeihanLi.Common.Logging
{
    internal class MicrosoftLoggingLogHelperProvider : ILogHelperProvider
    {
        private readonly ILoggerFactory _loggerFactory;

        public MicrosoftLoggingLogHelperProvider(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        public Task Log(LogHelperLoggingEvent loggingEvent)
        {
            var logger = _loggerFactory.CreateLogger(loggingEvent.CategoryName);
            LogInternal(logger, loggingEvent);
            return TaskHelper.CompletedTask;
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
                case LogHelperLevel.Debug:
                    logger.LogDebug(loggingEvent.Exception, loggingEvent.Message);
                    logged = true;
                    break;

                case LogHelperLevel.Trace:
                    logger.LogTrace(loggingEvent.Exception, loggingEvent.Message);
                    logged = true;
                    break;

                case LogHelperLevel.Info:
                    logger.LogInformation(loggingEvent.Exception, loggingEvent.Message);
                    logged = true;
                    break;

                case LogHelperLevel.Warn:
                    logger.LogWarning(loggingEvent.Exception, loggingEvent.Message);
                    logged = true;
                    break;

                case LogHelperLevel.Error:
                    logger.LogError(loggingEvent.Exception, loggingEvent.Message);
                    logged = true;
                    break;

                case LogHelperLevel.Fatal:
                    logger.LogError(loggingEvent.Exception, loggingEvent.Message);
                    logged = true;
                    break;
            }

            return logged;
        }

        private static LogLevel ConvertLogLevel(LogHelperLevel logHelperLevel)
        {
            switch (logHelperLevel)
            {
                case LogHelperLevel.All:
                    return LogLevel.Debug;

                case LogHelperLevel.Info:
                    return LogLevel.Information;

                case LogHelperLevel.Debug:
                    return LogLevel.Debug;

                case LogHelperLevel.Trace:
                    return LogLevel.Trace;

                case LogHelperLevel.Warn:
                    return LogLevel.Warning;

                case LogHelperLevel.Error:
                    return LogLevel.Error;

                case LogHelperLevel.Fatal:
                    return LogLevel.Critical;

                case LogHelperLevel.None:
                    return LogLevel.None;

                default:
                    return LogLevel.Warning;
            }
        }
    }

    internal static class MicrosoftLoggingExtensions
    {
        internal static void AddMicrosoftLogging(this ILogHelperFactory logHelperFactory, ILoggerFactory loggerFactory)
        {
            logHelperFactory.AddProvider(new MicrosoftLoggingLogHelperProvider(loggerFactory));
        }
    }
}
