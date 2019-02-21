using System;
using log4net;
using WeihanLi.Common.Helpers;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Logging.Log4Net
{
    public class Log4NetLogHelper : ILogHelperLogger
    {
        private readonly ILog _logger;

        public Log4NetLogHelper(string loggerName) =>
            _logger = LogManager.GetLogger(ApplicationHelper.ApplicationName, loggerName);

        public bool IsEnabled(LogHelperLevel loggerHelperLevel)
        {
            switch (loggerHelperLevel)
            {
                case LogHelperLevel.All:
                    return true;

                case LogHelperLevel.Info:
                    return _logger.IsInfoEnabled;

                case LogHelperLevel.Debug:
                case LogHelperLevel.Trace:
                    return _logger.IsDebugEnabled;

                case LogHelperLevel.Warn:
                    return _logger.IsWarnEnabled;

                case LogHelperLevel.Error:
                    return _logger.IsErrorEnabled;

                case LogHelperLevel.Fatal:
                    return _logger.IsFatalEnabled;

                case LogHelperLevel.None:
                    return false;

                default:
                    throw new ArgumentOutOfRangeException(nameof(loggerHelperLevel), loggerHelperLevel, "UnSupportedLogHelperLevel");
            }
        }

        public void Log(LogHelperLevel loggerLevel, string message, Exception exception)
        {
            if (IsEnabled(loggerLevel))
            {
                if (message.IsNotNullOrEmpty() || exception != null)
                {
                    switch (loggerLevel)
                    {
                        case LogHelperLevel.Info:
                            _logger.Info(message, exception);
                            break;

                        case LogHelperLevel.Debug:
                        case LogHelperLevel.Trace:
                            _logger.Debug(message, exception);
                            break;

                        case LogHelperLevel.Warn:
                            _logger.Warn(message, exception);
                            break;

                        case LogHelperLevel.Error:
                            _logger.Error(message, exception);
                            break;

                        case LogHelperLevel.Fatal:
                            _logger.Fatal(message, exception);
                            break;

                        default:
                            _logger.Warn($"Encountered unknown log level {loggerLevel}, writing out as Info.");
                            _logger.Info(message, exception);
                            break;
                    }
                }
            }
        }
    }
}
