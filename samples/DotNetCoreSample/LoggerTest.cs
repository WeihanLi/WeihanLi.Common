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
            LogHelper.LogFactory.WithMinimumLevel(LogHelperLevel.Info);

            Logger.Debug($"12333 {abc}");
            Logger.Trace("122334334");
            Logger.Info($"122334334 {abc}");

            Logger.Warn("12333");
            Logger.Error("122334334");
            Logger.Fatal("12333");
        }
    }
}
