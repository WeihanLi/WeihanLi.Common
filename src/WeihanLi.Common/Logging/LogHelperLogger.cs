using System;
using System.Collections.Generic;
using System.Linq;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Logging
{
    public interface ILogHelperLogger
    {
        void Log(LogHelperLevel loggerLevel, string message, Exception exception);

        bool IsEnabled(LogHelperLevel loggerLevel);
    }

    public interface ILogHelperLogger<out T> : ILogHelperLogger
    {
    }

    public class NullLogHelperLogger : ILogHelperLogger
    {
        public static readonly ILogHelperLogger Instance = new NullLogHelperLogger();

        private NullLogHelperLogger()
        {
        }

        public void Log(LogHelperLevel loggerLevel, string message, Exception exception)
        {
        }

        public bool IsEnabled(LogHelperLevel loggerLevel) => false;
    }

    internal class LogHelper : ILogHelperLogger
    {
        private readonly IReadOnlyCollection<ILogHelperLogger> _loggers;

        public LogHelper(ICollection<ILogHelperProvider> logHelperProviders, string categoryName)
        {
            _loggers = logHelperProviders.Select(_ => _.CreateLogger(categoryName)).ToArray();
        }

        public void Log(LogHelperLevel loggerLevel, string message, Exception exception)
        {
            _loggers.ForEach(logger =>
            {
                if (logger.IsEnabled(loggerLevel))
                {
                    logger.Log(loggerLevel, message, exception);
                }
            });
        }

        public bool IsEnabled(LogHelperLevel loggerLevel) => true;
    }
}
