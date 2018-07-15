using System;
using System.Collections.Generic;

#if NETSTANDARD2_0

using Microsoft.Extensions.Logging;

#endif

using WeihanLi.Common.Log;

namespace WeihanLi.Common.Helpers
{
    /// <summary>
    /// LogHelper
    /// Logger
    /// </summary>
    public static class LogHelper
    {
        private static ILogHelperFactory _loggerFactory;

        public static ILogHelper GetLogHelper<T>() => GetLogHelper(typeof(T));

        public static ILogHelper GetLogHelper(Type type) => GetLogHelper(type.FullName);

        public static ILogHelper GetLogHelper(string categoryName)
        {
            if (!_isInit)
            {
                throw new InvalidOperationException(Resource.LogHelperNotInitialized);
            }
            return _loggerFactory.CreateLogHelper(categoryName);
        }

        #region LogInit

        private static bool _isInit;

        public static void LogInit() => LogInit(ApplicationHelper.MapPath("log4net.config"));

        public static void LogInit(string configurationFilePath) => LogInit(configurationFilePath, null);

        public static void LogInit(ICollection<ILogHelperProvider> logProviders) => LogInit(ApplicationHelper.MapPath("log4net.config"), null);

        public static void LogInit(string configurationFilePath, ICollection<ILogHelperProvider> logProviders)
        {
            if (_isInit)
            {
                return;
            }
            _loggerFactory = logProviders == null ? new LogHelperFactory() : new LogHelperFactory(logProviders);

            _loggerFactory.AddProvider(new Log4NetLogHelperProvider(configurationFilePath));

            _isInit = true;
        }

        #endregion LogInit

#if NETSTANDARD2_0

        #region AddMicrosoftLogging

        public static void AddMicrosoftLogging(ILoggerFactory loggerFactory)
        {
            if (!_isInit)
            {
                throw new InvalidOperationException(Resource.LogHelperNotInitialized);
            }
            _loggerFactory.AddMicrosoftLogging(loggerFactory);
        }

        #endregion AddMicrosoftLogging

        #region Logger

        #region Info

        public static void Info(this ILogger logger, string msg, params object[] parameters) => logger.LogInformation(string.Format(msg, parameters), null);

        public static void Info(this ILogger logger, Exception ex, string msg) => logger.LogInformation(ex, msg);

        #endregion Info

        #region Trace

        public static void Trace(this ILogger logger, string msg, params object[] parameters) => logger.LogTrace(msg, parameters);

        public static void Trace(this ILogger logger, Exception ex, string msg) => logger.LogTrace(ex, msg);

        public static void Trace(this ILogger logger, Exception ex) => logger.LogTrace(ex, ex.Message);

        #endregion Trace

        #region Debug

        public static void Debug(this ILogger logger, string msg, params object[] parameters) => logger.LogDebug(msg, parameters);

        public static void Debug(this ILogger logger, Exception ex, string msg) => logger.LogDebug(ex, msg);

        public static void Debug(this ILogger logger, Exception ex) => logger.LogDebug(ex, ex.Message);

        #endregion Debug

        #region Warn

        public static void Warn(this ILogger logger, string msg, params object[] parameters) => logger.LogWarning(msg, parameters);

        public static void Warn(this ILogger logger, Exception ex, string msg) => logger.LogWarning(ex, msg);

        public static void Warn(this ILogger logger, Exception ex) => logger.LogWarning(ex, ex.Message);

        #endregion Warn

        #region Error

        public static void Error(this ILogger logger, string msg, params object[] parameters) => logger.LogError(msg, parameters);

        public static void Error(this ILogger logger, Exception ex, string msg) => logger.LogError(ex, msg);

        public static void Error(this ILogger logger, Exception ex) => logger.LogError(ex, ex?.Message);

        #endregion Error

        #region Fatal

        public static void Fatal(this ILogger logger, string msg, params object[] parameters) => logger.LogCritical(msg, parameters);

        public static void Fatal(this ILogger logger, Exception ex, string msg) => logger.LogCritical(ex, msg);

        public static void Fatal(this ILogger logger, Exception ex) => logger.LogCritical(ex, ex?.Message);

        #endregion Fatal

        #endregion Logger

#endif
    }
}
