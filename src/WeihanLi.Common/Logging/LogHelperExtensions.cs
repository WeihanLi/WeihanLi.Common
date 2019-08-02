using System;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Logging
{
    /// <summary>
    /// LogHelperExtensions
    /// </summary>
    public static class LogHelperExtensions
    {
        public static void Log(this ILogHelperLogger logger, LogHelperLevel loggerLevel, string msg) => logger.Log(loggerLevel, null, msg);

        #region Info

        public static void Info(this ILogHelperLogger logger, string msg, params object[] parameters)
        {
            if (parameters == null || parameters.Length == 0)
            {
                logger.Log(LogHelperLevel.Info, msg);
            }
            else
            {
                logger.Log(LogHelperLevel.Info, null, msg.FormatWith(parameters));
            }
        }

        public static void Info(this ILogHelperLogger logger, Exception ex, string msg) => logger.Log(LogHelperLevel.Info, ex, msg);

        public static void Info(this ILogHelperLogger logger, Exception ex) => logger.Log(LogHelperLevel.Info, ex, ex?.Message);

        #endregion Info

        #region Trace

        public static void Trace(this ILogHelperLogger logger, string msg, params object[] parameters)
        {
            if (parameters == null || parameters.Length == 0)
            {
                logger.Log(LogHelperLevel.Trace, msg);
            }
            else
            {
                logger.Log(LogHelperLevel.Trace, null, msg.FormatWith(parameters));
            }
        }

        public static void Trace(this ILogHelperLogger logger, Exception ex, string msg) => logger.Log(LogHelperLevel.Trace, ex, msg);

        public static void Trace(this ILogHelperLogger logger, Exception ex) => logger.Log(LogHelperLevel.Trace, ex, ex?.Message);

        #endregion Trace

        #region Debug

        public static void Debug(this ILogHelperLogger logger, string msg, params object[] parameters)
        {
            if (parameters == null || parameters.Length == 0)
            {
                logger.Log(LogHelperLevel.Debug, msg);
            }
            else
            {
                logger.Log(LogHelperLevel.Debug, null, msg.FormatWith(parameters));
            }
        }

        public static void Debug(this ILogHelperLogger logger, Exception ex, string msg) => logger.Log(LogHelperLevel.Debug, ex, msg);

        public static void Debug(this ILogHelperLogger logger, Exception ex) => logger.Log(LogHelperLevel.Debug, ex, ex?.Message);

        #endregion Debug

        #region Warn

        public static void Warn(this ILogHelperLogger logger, string msg, params object[] parameters)
        {
            if (parameters == null || parameters.Length == 0)
            {
                logger.Log(LogHelperLevel.Warn, null, msg);
            }
            else
            {
                logger.Log(LogHelperLevel.Warn, null, msg.FormatWith(parameters));
            }
        }

        public static void Warn(this ILogHelperLogger logger, Exception ex, string msg) => logger.Log(LogHelperLevel.Warn, ex, msg);

        public static void Warn(this ILogHelperLogger logger, Exception ex) => logger.Log(LogHelperLevel.Warn, ex, ex?.Message);

        #endregion Warn

        #region Error

        public static void Error(this ILogHelperLogger logger, string msg, params object[] parameters)
        {
            if (parameters == null || parameters.Length == 0)
            {
                logger.Log(LogHelperLevel.Error, null, msg);
            }
            else
            {
                logger.Log(LogHelperLevel.Error, null, msg.FormatWith(parameters));
            }
        }

        public static void Error(this ILogHelperLogger logger, Exception ex, string msg) => logger.Log(LogHelperLevel.Error, ex, msg);

        public static void Error(this ILogHelperLogger logger, Exception ex) => logger.Log(LogHelperLevel.Error, ex, ex?.Message);

        #endregion Error

        #region Fatal

        public static void Fatal(this ILogHelperLogger logger, string msg, params object[] parameters)
        {
            if (parameters == null || parameters.Length == 0)
            {
                logger.Log(LogHelperLevel.Fatal, null, msg);
            }
            else
            {
                logger.Log(LogHelperLevel.Fatal, null, msg.FormatWith(parameters));
            }
        }

        public static void Fatal(this ILogHelperLogger logger, Exception ex, string msg) => logger.Log(LogHelperLevel.Fatal, ex, msg);

        public static void Fatal(this ILogHelperLogger logger, Exception ex) => logger.Log(LogHelperLevel.Fatal, ex, ex?.Message);

        #endregion Fatal

        #region LoggerHelperProvider

        /// <summary>
        /// Create generic logger
        /// </summary>
        /// <typeparam name="TCategory">logger type</typeparam>
        /// <param name="logHelperProvider">logHelperProvider</param>
        /// <returns>ILogHelperLogger</returns>
        public static ILogHelperLogger CreateLogger<TCategory>(this ILogHelperProvider logHelperProvider) => logHelperProvider.CreateLogger(typeof(TCategory));

        /// <summary>
        /// Create generic logger
        /// </summary>
        /// <param name="logHelperProvider">logHelperProvider</param>
        /// <param name="type">logger type</param>
        /// <returns>ILogHelperLogger</returns>
        public static ILogHelperLogger CreateLogger(this ILogHelperProvider logHelperProvider, Type type) => logHelperProvider.CreateLogger(type.FullName);

        #endregion LoggerHelperProvider
    }
}
