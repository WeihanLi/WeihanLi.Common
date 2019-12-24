using System;
using System.Linq;
using System.Threading.Tasks;
using WeihanLi.Common.Helpers;

namespace WeihanLi.Common.Logging
{
    public interface ILogHelperLogger
    {
        void Log(LogHelperLevel logLevel, Exception exception, string message);

        bool IsEnabled(LogHelperLevel logLevel);
    }

    public class NullLogHelperLogger : ILogHelperLogger
    {
        public static readonly ILogHelperLogger Instance = new NullLogHelperLogger();

        private NullLogHelperLogger()
        {
        }

        public void Log(LogHelperLevel logLevel, Exception exception, string message)
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
        //private static volatile PeriodBatchingLoggingService _loggingService = null;
        //private static readonly object _loggingServiceLock = new object();

        public string CategoryName { get; }

        public LogHelper(LogHelperFactory logHelperFactory, string categoryName)
        {
            _logHelperFactory = logHelperFactory;
            CategoryName = categoryName;
        }

        public void Log(LogHelperLevel logLevel, Exception exception, string message)
        {
            if (!IsEnabled(logLevel))
                return;

            var loggingEvent = new LogHelperLoggingEvent()
            {
                CategoryName = CategoryName,
                DateTime = DateTimeOffset.UtcNow,
                Exception = exception,
                LogLevel = logLevel,
                Message = message,
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

            //if (null == _loggingService)
            //{
            //    lock (_loggingServiceLock)
            //    {
            //        if (null == _loggingService)
            //        {
            //            _loggingService = new PeriodBatchingLoggingService(_logHelperFactory.BatchSize, _logHelperFactory.Period, _logHelperFactory);
            //        }
            //    }
            //}
            //
            //_loggingService.Emit(loggingEvent);
        }

        public bool IsEnabled(LogHelperLevel logLevel) => logLevel != LogHelperLevel.None;
    }
}
