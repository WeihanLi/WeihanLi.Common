using JetBrains.Annotations;
using log4net;
using System.Threading.Tasks;
using WeihanLi.Common.Helpers;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Logging.Log4Net
{
    internal class Log4NetLogHelperProvider : ILogHelperProvider
    {
        public Log4NetLogHelperProvider() : this(ApplicationHelper.MapPath("log4net.config"))
        {
        }

        public Log4NetLogHelperProvider(string configurationFilePath) => Log4NetHelper.LogInit(configurationFilePath);

        public Task Log(LogHelperLoggingEvent loggingEvent)
        {
            var logger = LogManager.GetLogger(ApplicationHelper.ApplicationName, loggingEvent.CategoryName);
            if (IsEnabled(loggingEvent.LogLevel, logger))
            {
                if (loggingEvent.Message.IsNotNullOrEmpty() || loggingEvent.Exception != null)
                {
                    switch (loggingEvent.LogLevel)
                    {
                        case LogHelperLevel.Info:
                            logger.Info(loggingEvent.Message, loggingEvent.Exception);
                            break;

                        case LogHelperLevel.Debug:
                        case LogHelperLevel.Trace:
                            logger.Debug(loggingEvent.Message, loggingEvent.Exception);
                            break;

                        case LogHelperLevel.Warn:
                            logger.Warn(loggingEvent.Message, loggingEvent.Exception);
                            break;

                        case LogHelperLevel.Error:
                            logger.Error(loggingEvent.Message, loggingEvent.Exception);
                            break;

                        case LogHelperLevel.Fatal:
                            logger.Fatal(loggingEvent.Message, loggingEvent.Exception);
                            break;

                        default:
                            logger.Warn($"Encountered unknown log level {loggingEvent.LogLevel}, writing out as Info.");
                            logger.Info(loggingEvent.Message, loggingEvent.Exception);
                            break;
                    }
                }
            }

            return TaskHelper.CompletedTask;
        }

        private static bool IsEnabled(LogHelperLevel loggerHelperLevel, ILog logger)
        {
            switch (loggerHelperLevel)
            {
                case LogHelperLevel.Info:
                    return logger.IsInfoEnabled;

                case LogHelperLevel.Debug:
                case LogHelperLevel.Trace:
                    return logger.IsDebugEnabled;

                case LogHelperLevel.Warn:
                    return logger.IsWarnEnabled;

                case LogHelperLevel.Error:
                    return logger.IsErrorEnabled;

                case LogHelperLevel.Fatal:
                    return logger.IsFatalEnabled;

                default:
                    return false;
            }
        }
    }

    public static class LogHelperFactoryExtensions
    {
        public static ILogHelperFactory AddLog4Net([NotNull]this ILogHelperFactory logHelperFactory)
        {
            logHelperFactory.AddProvider(new Log4NetLogHelperProvider());
            return logHelperFactory;
        }

        public static ILogHelperFactory AddLog4Net([NotNull] this ILogHelperFactory logHelperFactory,
            string configFilePath)
        {
            logHelperFactory.AddProvider(new Log4NetLogHelperProvider(configFilePath));
            return logHelperFactory;
        }
    }
}
