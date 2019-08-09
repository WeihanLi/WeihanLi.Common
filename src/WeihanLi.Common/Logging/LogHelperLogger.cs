using System;
using System.Collections.Generic;
using System.Linq;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Logging
{
    public interface ILogHelperLogger
    {
        void Log(LogHelperLevel loggerLevel, Exception exception, string message);

        bool IsEnabled(LogHelperLevel loggerLevel);
    }

    public class NullLogHelperLogger : ILogHelperLogger
    {
        public static readonly ILogHelperLogger Instance = new NullLogHelperLogger();

        private NullLogHelperLogger()
        {
        }

        public void Log(LogHelperLevel loggerLevel, Exception exception, string message)
        {
        }

        public bool IsEnabled(LogHelperLevel loggerLevel) => false;
    }

    public interface ILogHelperLogger<TCategory> : ILogHelperLogger
    {
    }

    internal class LogHelper<TCategory> : ILogHelperLogger<TCategory>
    {
        private readonly IReadOnlyCollection<ILogHelperLogger> _loggers;

        public string CategoryName { get; }

        public LogHelper(ICollection<ILogHelperProvider> logHelperProviders)
        {
            CategoryName = typeof(TCategory).FullName;
            _loggers = logHelperProviders.Select(_ => _.CreateLogger(CategoryName)).ToArray();
        }

        public void Log(LogHelperLevel loggerLevel, Exception exception, string message)
        {
            _loggers.ForEach(logger =>
            {
                if (logger.IsEnabled(loggerLevel))
                {
                    logger.Log(loggerLevel, exception, message);
                }
            });
        }

        public bool IsEnabled(LogHelperLevel loggerLevel) => loggerLevel != LogHelperLevel.None;
    }

    internal class LogHelper : ILogHelperLogger
    {
        private readonly LogHelperFactory _logHelperFactory;

        public string CategoryName { get; }

        public LogHelper(LogHelperFactory logHelperFactory, string categoryName)
        {
            _logHelperFactory = logHelperFactory;
            CategoryName = categoryName;
        }

        public void Log(LogHelperLevel loggerLevel, Exception exception, string message)
        {
            var logProviders = new List<ILogHelperProvider>(_logHelperFactory._logHelperProviders.Values);
            foreach (var logHelperProvider in _logHelperFactory._logHelperProviders)
            {
                foreach (var logFilter in _logHelperFactory._logFilters)
                {
                    if (!logFilter.Invoke(logHelperProvider.Key, CategoryName, loggerLevel, exception))
                    {
                        logProviders.Remove(logHelperProvider.Value);
                    }
                }
            }

            if (logProviders.Count == 0)
                return;

            var loggers = logProviders.Select(_ => _.CreateLogger(CategoryName)).ToArray();
            loggers.ForEach(logger =>
            {
                if (logger.IsEnabled(loggerLevel))
                {
                    logger.Log(loggerLevel, exception, message);
                }
            });
        }

        public bool IsEnabled(LogHelperLevel loggerLevel) => loggerLevel != LogHelperLevel.None;
    }
}
