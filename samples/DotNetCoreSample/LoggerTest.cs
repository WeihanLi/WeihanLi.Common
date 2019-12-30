using WeihanLi.Common.Helpers;
using WeihanLi.Common.Logging;
using WeihanLi.Common.Logging.Log4Net;

namespace DotNetCoreSample
{
    internal class LoggerTest
    {
        private static readonly ILogHelperLogger Logger = LogHelper.GetLogger<LoggerTest>();

        public static void MainTest()
        {
            var abc = "1233";
            LogHelper.LogFactory.AddLog4Net();
            // LogHelper.LogFactory.AddSerilog(loggerConfig => loggerConfig.WriteTo.Console());

            LogHelper.LogFactory.WithMinimumLevel(LogHelperLogLevel.Info);
            LogHelper.LogFactory.EnrichWithProperty("Entry", ApplicationHelper.ApplicationName);

            Logger.Debug("12333 {abc}", abc);
            Logger.Trace("122334334");
            Logger.Info($"122334334 {abc}");

            Logger.Warn("12333, err:{err}", "hahaha");
            Logger.Error("122334334");
            Logger.Fatal("12333");
        }
    }
}
