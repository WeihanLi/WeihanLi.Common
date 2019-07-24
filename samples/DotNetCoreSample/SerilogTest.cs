using Microsoft.Extensions.Logging;
using Serilog;
using WeihanLi.Common.Logging.Serilog;

namespace DotNetCoreSample
{
    public static class SerilogTest
    {
        public static void MainTest()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            SerilogHelper.LogInit(config => config.WriteTo.Console());
            loggerFactory.AddSerilog();

            var logger = loggerFactory.CreateLogger(typeof(SerilogTest));
            logger.LogTrace("1111111");
            logger.LogDebug("1111111");
            logger.LogInformation("121212331");
            logger.LogWarning("121212331");
            logger.LogError("121212331");
            logger.LogCritical("1213131");
        }
    }
}
