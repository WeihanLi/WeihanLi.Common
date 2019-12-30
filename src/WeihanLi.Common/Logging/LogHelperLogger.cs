using System;
using System.Linq;
using System.Threading.Tasks;
using WeihanLi.Common.Helpers;

namespace WeihanLi.Common.Logging
{
    public interface ILogHelperLogger
    {
        void Log(LogHelperLevel logLevel, Exception exception, string messageTemplate, params object[] parameters);

        bool IsEnabled(LogHelperLevel logLevel);
    }

    public class NullLogHelperLogger : ILogHelperLogger
    {
        public static readonly ILogHelperLogger Instance = new NullLogHelperLogger();

        private NullLogHelperLogger()
        {
        }

        public void Log(LogHelperLevel logLevel, Exception exception, string messageTemplate, params object[] parameters)
        {
        }

        public bool IsEnabled(LogHelperLevel logLevel) => false;
    }

    public interface ILogHelperLogger<TCategory> : ILogHelperLogger
    {
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

        public void Log(LogHelperLevel logLevel, Exception exception, string messageTemplate, params object[] parameters)
        {
            if (!IsEnabled(logLevel))
                return;

            if (!_logHelperFactory._logFilters.Any(x => x.Invoke(typeof(int), CategoryName, logLevel, exception)))
            {
                return;
            }

            var formattedLog = LoggingFormatter.Format(messageTemplate, parameters);
            var loggingEvent = new LogHelperLoggingEvent()
            {
                CategoryName = CategoryName,
                DateTime = DateTimeOffset.UtcNow,
                Exception = exception,
                LogLevel = logLevel,
                MessageTemplate = messageTemplate,
                Message = formattedLog.Msg,
                Properties = formattedLog.Values,
            };

            Task.WaitAll(_logHelperFactory._logHelperProviders.Select(logHelperProvider =>
                {
                    if (_logHelperFactory._logFilters.All(x => x.Invoke(logHelperProvider.Key,
                        loggingEvent.CategoryName, loggingEvent.LogLevel, loggingEvent.Exception)))
                    {
                        return logHelperProvider.Value.Log(loggingEvent);
                    }
                    return TaskHelper.CompletedTask;
                }
                    ).ToArray());
        }

        public bool IsEnabled(LogHelperLevel logLevel) => logLevel != LogHelperLevel.None;
    }
}
