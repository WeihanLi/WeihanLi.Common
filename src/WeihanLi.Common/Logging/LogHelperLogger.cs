using System;

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

    internal class LogHelper : ILogHelperLogger
    {
        private readonly LogHelperFactory _logHelperFactory;
        private static volatile PeriodBatchingLoggingService _loggingService = null;
        private static readonly object _loggingServiceLock = new object();

        public string CategoryName { get; }

        public LogHelper(LogHelperFactory logHelperFactory, string categoryName)
        {
            _logHelperFactory = logHelperFactory;
            CategoryName = categoryName;
        }

        public void Log(LogHelperLevel loggerLevel, Exception exception, string message)
        {
            if (!IsEnabled(loggerLevel))
                return;

            if (null == _loggingService)
            {
                lock (_loggingServiceLock)
                {
                    if (null == _loggingService)
                    {
                        _loggingService = new PeriodBatchingLoggingService(_logHelperFactory.BatchSize, _logHelperFactory.Period, _logHelperFactory);
                    }
                }
            }

            _loggingService.Emit(new LogHelperLoggingEvent()
            {
                CategoryName = CategoryName,
                DateTime = DateTimeOffset.UtcNow,
                Exception = exception,
                LogLevel = loggerLevel,
                Message = message,
            });
        }

        public bool IsEnabled(LogHelperLevel loggerLevel) => loggerLevel != LogHelperLevel.None;
    }
}
