using WeihanLi.Common.Helpers;
using WeihanLi.Common.Log;

namespace DotNetCoreSample
{
    internal class LoggerTest
    {
        private static readonly ILogHelper Logger = LogHelper.GetLogHelper<LoggerTest>();

        public static void MainTest()
        {
            Logger.Info("122334334");
            Logger.Debug("12333");
            Logger.Trace("122334334");
            Logger.Warn("12333");
            Logger.Error("122334334");
            Logger.Fatal("12333");
        }
    }
}
