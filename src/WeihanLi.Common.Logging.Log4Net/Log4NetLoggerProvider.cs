using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using log4net;
using log4net.Config;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions.Internal;
using WeihanLi.Common.Helpers;
using WeihanLi.Common.Logging.Log4Net;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace WeihanLi.Common.Logging.Log4Net
{
    
    [ProviderAlias("log4net")]
    internal class Log4NetLoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, Log4NetLogger> _loggers =
            new ConcurrentDictionary<string, Log4NetLogger>(StringComparer.Ordinal);

        public Log4NetLoggerProvider(string confFilePath)
        {
            if (null == LogManager.GetAllRepositories()?.FirstOrDefault(_ => _.Name == ApplicationHelper.ApplicationName))
            {
                XmlConfigurator.ConfigureAndWatch(LogManager.CreateRepository(ApplicationHelper.ApplicationName), new FileInfo(confFilePath));
            }
        }

        public void Dispose() => _loggers.Clear();

        public ILogger CreateLogger(string categoryName) => _loggers.GetOrAdd(categoryName, loggerName => new Log4NetLogger(loggerName));
    }
    
    internal class Log4NetLogger : ILogger
    {
        private readonly ILog _logger;

        public Log4NetLogger(string name) => _logger = LogManager.GetLogger(ApplicationHelper.ApplicationName, name);

        public IDisposable BeginScope<TState>(TState state) => NullScope.Instance;

        public bool IsEnabled(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Critical:
                    return _logger.IsFatalEnabled;

                case LogLevel.Debug:
                case LogLevel.Trace:
                    return _logger.IsDebugEnabled;

                case LogLevel.Error:
                    return _logger.IsErrorEnabled;

                case LogLevel.Information:
                    return _logger.IsInfoEnabled;

                case LogLevel.Warning:
                    return _logger.IsWarnEnabled;

                case LogLevel.None:
                    return false;

                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel));
            }
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state,
            Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            var message = formatter(state, exception);

            if (!(string.IsNullOrEmpty(message) && exception == null))
            {
                switch (logLevel)
                {
                    case LogLevel.Critical:
                        _logger.Fatal(message, exception);
                        break;

                    case LogLevel.Debug:
                    case LogLevel.Trace:
                        _logger.Debug(message, exception);
                        break;

                    case LogLevel.Error:
                        _logger.Error(message, exception);
                        break;

                    case LogLevel.Information:
                        _logger.Info(message, exception);
                        break;

                    case LogLevel.Warning:
                        _logger.Warn(message, exception);
                        break;

                    default:
                        _logger.Warn($"Encountered unknown log level {logLevel}, writing out as Info.");
                        _logger.Info(message, exception);
                        break;
                }
            }
        }
    }
}

namespace Microsoft.Extensions.Logging
{
    public static class Log4NetLoggerFactoryExtensions
    {
        public static ILoggerFactory AddLog4Net(this ILoggerFactory factory)
        {
            factory.AddProvider(new Log4NetLoggerProvider(ApplicationHelper.MapPath("log4net.config")));

            return factory;
        }

        public static ILoggerFactory AddLog4Net(this ILoggerFactory factory, string configFile)
        {
            factory.AddProvider(new Log4NetLoggerProvider(configFile));

            return factory;
        }
    }
}
