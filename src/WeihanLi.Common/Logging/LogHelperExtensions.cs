using System;

namespace WeihanLi.Common.Logging
{
    /// <summary>
    /// LogHelperExtensions
    /// </summary>
    public static class LogHelperExtensions
    {
        #region Info

        public static void Info(this ILogHelperLogger logger, string msg, params object[] parameters) => logger.Log(LogHelperLevel.Info, string.Format(msg, parameters), null);

        public static void Info(this ILogHelperLogger logger, Exception ex, string msg) => logger.Log(LogHelperLevel.Info, msg, ex);

        public static void Info(this ILogHelperLogger logger, Exception ex) => logger.Log(LogHelperLevel.Info, ex?.Message, ex);

        #endregion Info

        #region Trace

        public static void Trace(this ILogHelperLogger logger, string msg, params object[] parameters) => logger.Log(LogHelperLevel.Trace, string.Format(msg, parameters), null);

        public static void Trace(this ILogHelperLogger logger, Exception ex, string msg) => logger.Log(LogHelperLevel.Trace, msg, ex);

        public static void Trace(this ILogHelperLogger logger, Exception ex) => logger.Log(LogHelperLevel.Trace, ex?.Message, ex);

        #endregion Trace

        #region Debug

        public static void Debug(this ILogHelperLogger logger, string msg, params object[] parameters) => logger.Log(LogHelperLevel.Debug, string.Format(msg, parameters), null);

        public static void Debug(this ILogHelperLogger logger, Exception ex, string msg) => logger.Log(LogHelperLevel.Debug, msg, ex);

        public static void Debug(this ILogHelperLogger logger, Exception ex) => logger.Log(LogHelperLevel.Debug, ex?.Message, ex);

        #endregion Debug

        #region Warn

        public static void Warn(this ILogHelperLogger logger, string msg, params object[] parameters) => logger.Log(LogHelperLevel.Warn, string.Format(msg, parameters), null);

        public static void Warn(this ILogHelperLogger logger, Exception ex, string msg) => logger.Log(LogHelperLevel.Warn, msg, ex);

        public static void Warn(this ILogHelperLogger logger, Exception ex) => logger.Log(LogHelperLevel.Warn, ex?.Message, ex);

        #endregion Warn

        #region Error

        public static void Error(this ILogHelperLogger logger, string msg, params object[] parameters) => logger.Log(LogHelperLevel.Error, string.Format(msg, parameters), null);

        public static void Error(this ILogHelperLogger logger, Exception ex, string msg) => logger.Log(LogHelperLevel.Error, msg, ex);

        public static void Error(this ILogHelperLogger logger, Exception ex) => logger.Log(LogHelperLevel.Error, ex?.Message, ex);

        #endregion Error

        #region Fatal

        public static void Fatal(this ILogHelperLogger logger, string msg, params object[] parameters) => logger.Log(LogHelperLevel.Fatal, string.Format(msg, parameters), null);

        public static void Fatal(this ILogHelperLogger logger, Exception ex, string msg) => logger.Log(LogHelperLevel.Fatal, msg, ex);

        public static void Fatal(this ILogHelperLogger logger, Exception ex) => logger.Log(LogHelperLevel.Fatal, ex?.Message, ex);

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
        /// <typeparam name="type">logger type</typeparam>
        /// <returns>ILogHelperLogger</returns>
        public static ILogHelperLogger CreateLogger(this ILogHelperProvider logHelperProvider, Type type) => logHelperProvider.CreateLogger(type.FullName);

        #endregion LoggerHelperProvider
    }
}
