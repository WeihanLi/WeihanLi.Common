using System;
using System.Collections.Generic;
using System.Linq;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Logging
{
    public interface ILogHelperLogger
    {
        void Log(LogHelperLevel loggerLevel, Exception exception, string message, params object[] parameters);

        bool IsEnabled(LogHelperLevel loggerLevel);
    }

    public class NullLogHelperLogger : ILogHelperLogger
    {
        public static readonly ILogHelperLogger Instance = new NullLogHelperLogger();

        private NullLogHelperLogger()
        {
        }

        public void Log(LogHelperLevel loggerLevel, Exception exception, string message, params object[] parameters)
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

        public void Log(LogHelperLevel loggerLevel, Exception exception, string message, params object[] parameters)
        {
            _loggers.ForEach(logger =>
            {
                if (logger.IsEnabled(loggerLevel))
                {
                    logger.Log(loggerLevel, exception, message, parameters);
                }
            });
        }

        public bool IsEnabled(LogHelperLevel loggerLevel) => loggerLevel != LogHelperLevel.None;
    }

    internal class LogHelper : ILogHelperLogger
    {
        private readonly IReadOnlyCollection<ILogHelperLogger> _loggers;

        public string CategoryName { get; }

        public LogHelper(ICollection<ILogHelperProvider> logHelperProviders, string categoryName)
        {
            CategoryName = categoryName;
            _loggers = logHelperProviders.Select(_ => _.CreateLogger(categoryName)).ToArray();
        }

        public void Log(LogHelperLevel loggerLevel, Exception exception, string message, params object[] parameters)
        {
            _loggers.ForEach(logger =>
            {
                if (logger.IsEnabled(loggerLevel))
                {
                    logger.Log(loggerLevel, exception, message, parameters);
                }
            });
        }

        public bool IsEnabled(LogHelperLevel loggerLevel) => loggerLevel != LogHelperLevel.None;
    }
}
