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

    public class NullLogHelperLogger : ILogHelperLogger
    {
        public void Log(LogHelperLevel loggerLevel, string message, Exception exception)
        {
        }

        public bool IsEnabled(LogHelperLevel loggerLevel)
        {
            return false;
        }
    }

    internal class LogHelper : ILogHelperLogger
    {
        private readonly IReadOnlyCollection<ILogHelperLogger> _logHelpers;

        public LogHelper(ICollection<ILogHelperProvider> logHelperProviders, string categoryName)
        {
            _logHelpers = logHelperProviders.Select(_ => _.CreateLogger(categoryName)).ToArray();
        }

        public void Log(LogHelperLevel loggerLevel, string message, Exception exception)
        {
            _logHelpers.ForEach(logHelper =>
            {
                logHelper.Log(loggerLevel, message, exception);
            });
        }

        public bool IsEnabled(LogHelperLevel loggerLevel) => true;
    }
}
