using System;
using System.Linq;
using System.Threading.Tasks;

namespace WeihanLi.Common.Logging
{
    public interface ILogHelperLogger
    {
        void Log(LogHelperLogLevel logLevel, Exception exception, string messageTemplate, params object[] parameters);

        bool IsEnabled(LogHelperLogLevel logLevel);
    }

    internal class NullLogHelperLogger : ILogHelperLogger
    {
        public static readonly ILogHelperLogger Instance = new NullLogHelperLogger();

        private NullLogHelperLogger()
        {
        }

        public void Log(LogHelperLogLevel logLevel, Exception exception, string messageTemplate, params object[] parameters)
        {
        }

        public bool IsEnabled(LogHelperLogLevel logLevel) => false;
    }

    public interface ILogHelperLogger<TCategory> : ILogHelperLogger
    {
    }

    internal class LogHelperGenericLogger<TCategory> : LogHelper, ILogHelperLogger<TCategory>
    {
        public LogHelperGenericLogger(LogHelperFactory logHelperFactory, string categoryName) : base(logHelperFactory, categoryName)
        {
        }
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

        public void Log(LogHelperLogLevel logLevel, Exception exception, string messageTemplate, params object[] parameters)
        {
            if (!IsEnabled(logLevel))
                return;

            var loggingEvent = new LogHelperLoggingEvent()
            {
                CategoryName = CategoryName,
                DateTime = DateTimeOffset.UtcNow,
                Exception = exception,
                LogLevel = logLevel,
                MessageTemplate = messageTemplate,
            };

            if (!_logHelperFactory._logFilters.Any(x => x.Invoke(typeof(int), loggingEvent)))
            {
                return;
            }

            var formattedLog = LoggingFormatter.Format(messageTemplate, parameters);
            loggingEvent.Message = formattedLog.Msg;
            loggingEvent.Properties = formattedLog.Values;

            foreach (var enricher in _logHelperFactory._logHelperEnrichers)
            {
                enricher.Enrich(loggingEvent);
            }

            Parallel.ForEach(_logHelperFactory._logHelperProviders, logHelperProvider =>
            {
                if (_logHelperFactory._logFilters.All(x => x.Invoke(logHelperProvider.Key, loggingEvent)))
                {
                    logHelperProvider.Value.Log(loggingEvent);
                }
            });
        }

        public bool IsEnabled(LogHelperLogLevel logLevel) => logLevel != LogHelperLogLevel.None;
    }
}
