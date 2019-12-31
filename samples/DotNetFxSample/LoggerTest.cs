using WeihanLi.Common.Helpers;
using WeihanLi.Common.Logging;
using WeihanLi.Common.Logging.Log4Net;

namespace DotNetFxSample
{
    public class LoggerTest
    {
        private static readonly ILogHelperLogger Logger = LogHelper.GetLogger<LoggerTest>();

        public static void Test()
        {
            LogHelper.ConfigureLogging(builder => builder.AddLog4Net());

            Logger.Info("info message");
            Logger.Debug("debug message");
            Logger.Trace("Trace message");
            Logger.Error("Error messsage");
            Logger.Fatal("Fatal message");

            log4net.LogManager.GetLogger("testTtTLogger").Info("121313");
        }
    }
}
