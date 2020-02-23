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
            LogHelper.ConfigureLogging(builder =>
            {
                builder
                    .AddLog4Net()
                    //.AddSerilog(loggerConfig => loggerConfig.WriteTo.Console())
                    .WithMinimumLevel(LogHelperLogLevel.Info)
                    .WithFilter((category, level) => level > LogHelperLogLevel.Error && category.StartsWith("System"))
                    .EnrichWithProperty("Entry0", ApplicationHelper.ApplicationName)
                    .EnrichWithProperty("Entry1", ApplicationHelper.ApplicationName, e => e.LogLevel >= LogHelperLogLevel.Error)
                    ;
            });

            Logger.Debug("12333 {abc}", abc);
            Logger.Trace("122334334");
            Logger.Info($"122334334 {abc}");

            Logger.Warn("12333, err:{err}", "hahaha");
            Logger.Error("122334334");
            Logger.Fatal("12333");
        }
    }
}
