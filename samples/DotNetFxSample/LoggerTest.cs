using System;
using WeihanLi.Common.Helpers;
using WeihanLi.Common.Logging;

namespace DotNetFxSample
{
    public class LoggerTest
    {
        private static readonly ILogHelperLogger Logger = LogHelper.GetLogger<LoggerTest>();

        public static void Test()
        {
            Logger.Info("info message");
            Logger.Debug("debug message");
            Logger.Trace("Trace message");
            Logger.Error("Error messsage");
            Logger.Fatal("Fatal message");

            throw new ArgumentNullException();
        }
    }
}
