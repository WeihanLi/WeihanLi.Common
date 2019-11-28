using System;
using Serilog;

namespace WeihanLi.Common.Logging.Serilog
{
    public static class SerilogHelper
    {
        private static readonly object Locker = new object();

        public static void LogInit(Action<LoggerConfiguration> configAction)
        {
            var loggerConfiguration = new LoggerConfiguration();
            loggerConfiguration.Enrich.FromLogContext();
            configAction?.Invoke(loggerConfiguration);
            LogInit(loggerConfiguration);
        }

        public static void LogInit(LoggerConfiguration loggerConfiguration)
        {
            lock (Locker)
            {
                Log.Logger = loggerConfiguration.CreateLogger();
            }
        }
    }
}
