using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace WeihanLi.Common.Log
{
    internal class MicrosoftLoggingLogHelperProvider : ILogHelperProvider
    {
        private readonly ConcurrentDictionary<string, MicrosoftLoggingLogHelper> _loggers = new ConcurrentDictionary<string, MicrosoftLoggingLogHelper>();

        private readonly ILoggerFactory _loggerFactory;

        public MicrosoftLoggingLogHelperProvider(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        public ILogHelper CreateLogHelper(string categoryName) => _loggers.GetOrAdd(categoryName, _ => new MicrosoftLoggingLogHelper(_loggerFactory.CreateLogger(_)));
    }

    internal class MicrosoftLoggingLogHelper : ILogHelper
    {
        private readonly ILogger _logger;

        public MicrosoftLoggingLogHelper(ILogger logger)
        {
            _logger = logger;
        }

        public bool IsEnabled(LogHelperLevel loggerLevel)
        {
            switch (loggerLevel)
            {
                case LogHelperLevel.All:
                    return true;

                case LogHelperLevel.Info:
                    return _logger.IsEnabled(LogLevel.Information);

                case LogHelperLevel.Debug:
                    return _logger.IsEnabled(LogLevel.Debug);

                case LogHelperLevel.Trace:
                    return _logger.IsEnabled(LogLevel.Trace);

                case LogHelperLevel.Warn:
                    return _logger.IsEnabled(LogLevel.Warning);

                case LogHelperLevel.Error:
                    return _logger.IsEnabled(LogLevel.Error);

                case LogHelperLevel.Fatal:
                    return _logger.IsEnabled(LogLevel.Critical);

                case LogHelperLevel.None:
                    return false;

                default:
                    throw new ArgumentOutOfRangeException(nameof(loggerLevel), loggerLevel, null);
            }
        }

        public void Log(LogHelperLevel loggerLevel, string message, Exception exception)
        {
            if (IsEnabled(loggerLevel))
            {
                switch (loggerLevel)
                {
                    case LogHelperLevel.Info:
                        _logger.LogInformation(exception, message);
                        break;

                    case LogHelperLevel.Debug:
                        _logger.LogDebug(exception, message);
                        break;

                    case LogHelperLevel.Trace:
                        _logger.LogTrace(exception, message);
                        break;

                    case LogHelperLevel.Warn:
                        _logger.LogWarning(exception, message);
                        break;

                    case LogHelperLevel.Error:
                        _logger.LogError(exception, message);
                        break;

                    case LogHelperLevel.Fatal:
                        _logger.LogError(exception, message);
                        break;

                    default:
                        _logger.LogWarning($"Encountered unknown log level {loggerLevel}, writing out as Info.");
                        _logger.LogInformation(exception, message);
                        break;
                }
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
