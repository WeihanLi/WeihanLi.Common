using System;

namespace WeihanLi.Common.Logging
{
    /// <summary>
    /// LogHelperExtensions
    /// </summary>
    public static class LogHelperExtensions
    {
        #region Info

        public static void InfoFormat(this ILogHelperLogger logHelper, string msg, params object[] parameters) => logHelper.Log(LogHelperLevel.Info, string.Format(msg, parameters), null);

        public static void Info(this ILogHelperLogger logHelper, string msg) => logHelper.Log(LogHelperLevel.Info, msg, null);

        public static void Info(this ILogHelperLogger logHelper, string msg, Exception ex) => logHelper.Log(LogHelperLevel.Info, msg, ex);

        public static void Info(this ILogHelperLogger logHelper, Exception ex) => logHelper.Log(LogHelperLevel.Info, ex?.Message, ex);

        #endregion Info

        #region Trace

        public static void TraceFormat(this ILogHelperLogger logHelper, string msg, params object[] parameters) => logHelper.Log(LogHelperLevel.Trace, string.Format(msg, parameters), null);

        public static void Trace(this ILogHelperLogger logHelper, string msg) => logHelper.Log(LogHelperLevel.Trace, msg, null);

        public static void Trace(this ILogHelperLogger logHelper, string msg, Exception ex) => logHelper.Log(LogHelperLevel.Trace, msg, ex);

        public static void Trace(this ILogHelperLogger logHelper, Exception ex) => logHelper.Log(LogHelperLevel.Trace, ex?.Message, ex);

        #endregion Trace

        #region Debug

        public static void DebugFormat(this ILogHelperLogger logHelper, string msg, params object[] parameters) => logHelper.Log(LogHelperLevel.Debug, string.Format(msg, parameters), null);

        public static void Debug(this ILogHelperLogger logHelper, string msg) => logHelper.Log(LogHelperLevel.Debug, msg, null);

        public static void Debug(this ILogHelperLogger logHelper, string msg, Exception ex) => logHelper.Log(LogHelperLevel.Debug, msg, ex);

        public static void Debug(this ILogHelperLogger logHelper, Exception ex) => logHelper.Log(LogHelperLevel.Debug, ex?.Message, ex);

        #endregion Debug

        #region Warn

        public static void WarnFormat(this ILogHelperLogger logHelper, string msg, params object[] parameters) => logHelper.Log(LogHelperLevel.Warn, string.Format(msg, parameters), null);

        public static void Warn(this ILogHelperLogger logHelper, string msg) => logHelper.Log(LogHelperLevel.Warn, msg, null);

        public static void Warn(this ILogHelperLogger logHelper, string msg, Exception ex) => logHelper.Log(LogHelperLevel.Warn, msg, ex);

        public static void Warn(this ILogHelperLogger logHelper, Exception ex) => logHelper.Log(LogHelperLevel.Warn, ex?.Message, ex);

        #endregion Warn

        #region Error

        public static void ErrorFormat(this ILogHelperLogger logHelper, string msg, params object[] parameters) => logHelper.Log(LogHelperLevel.Error, string.Format(msg, parameters), null);

        public static void Error(this ILogHelperLogger logHelper, string msg) => logHelper.Log(LogHelperLevel.Error, msg, null);

        public static void Error(this ILogHelperLogger logHelper, string msg, Exception ex) => logHelper.Log(LogHelperLevel.Error, msg, ex);

        public static void Error(this ILogHelperLogger logHelper, Exception ex) => logHelper.Log(LogHelperLevel.Error, ex?.Message, ex);

        #endregion Error

        #region Fatal

        public static void FatalFormat(this ILogHelperLogger logHelper, string msg, params object[] parameters) => logHelper.Log(LogHelperLevel.Fatal, string.Format(msg, parameters), null);

        public static void Fatal(this ILogHelperLogger logHelper, string msg) => logHelper.Log(LogHelperLevel.Fatal, msg, null);

        public static void Fatal(this ILogHelperLogger logHelper, string msg, Exception ex) => logHelper.Log(LogHelperLevel.Fatal, msg, ex);

        public static void Fatal(this ILogHelperLogger logHelper, Exception ex) => logHelper.Log(LogHelperLevel.Fatal, ex?.Message, ex);

        #endregion Fatal
    }

    public static class LogHelperFactoryExtension
    {
    }
}
