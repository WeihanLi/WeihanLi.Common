using WeihanLi.Common.Logging.Log4Net;

namespace DotNetCoreSample;

public static class Log4NetTest
{
    public static void MainTest()
    {
        Log4NetHelper.LogInit();
        var logger = Log4NetHelper.GetLogger(typeof(Log4NetTest));
        logger.Info("adccc");
    }
}
