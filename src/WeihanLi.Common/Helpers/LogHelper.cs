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

#endif
    }
}
