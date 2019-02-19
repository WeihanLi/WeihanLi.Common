using System;
using System.Collections.Generic;
using System.Linq;

#if NETSTANDARD2_0

using Microsoft.Extensions.Logging;

#endif

using WeihanLi.Common.Logging;

namespace WeihanLi.Common.Helpers
{
    /// <summary>
    /// LogHelper
    /// Logging
    /// </summary>
    public static class LogHelper
    {
        private static readonly Lazy<ILogHelperFactory> _loggerFactory = new Lazy<ILogHelperFactory>(() => new LogHelperFactory());

        public static ILogHelperLogger GetLogger<T>() => GetLogger(typeof(T));

        public static ILogHelperLogger GetLogger(Type type) => GetLogger(type.FullName);

        public static ILogHelperLogger GetLogger(string categoryName)
        {
            return _loggerFactory.Value.CreateLogHelper(categoryName);
        }

        public static bool AddLogProvider(ILogHelperProvider logHelperProvider)
        {
            return _loggerFactory.Value.AddProvider(logHelperProvider);
        }

        public static int AddLogProvider(ICollection<ILogHelperProvider> logProviders)
        {
            if (logProviders != null && logProviders.Count > 0)
            {
                var results = new bool[logProviders.Count];
                var idx = 0;
                foreach (var provider in logProviders)
                {
                    if (provider != null)
                    {
                        results[idx] = _loggerFactory.Value.AddProvider(provider);
                    }
                    idx++;
                }
                return results.Count(_ => _);
            }

            return 0;
        }

#if NETSTANDARD2_0

        #region LoggerExrtensions

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

        #endregion LoggerExrtensions

#endif
    }
}
