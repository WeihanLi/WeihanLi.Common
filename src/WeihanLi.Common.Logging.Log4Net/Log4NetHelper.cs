using System.IO;
using System.Linq;
using log4net;
using log4net.Config;
using WeihanLi.Common.Helpers;

namespace WeihanLi.Common.Logging.Log4Net
{
    public static class Log4NetHelper
    {
        /// <summary>
        /// log4net init
        /// use the default log4net.config config file
        /// </summary>
        /// <returns>1 config success,0 config has existed</returns>
        public static int LogInit() => LogInit(ApplicationHelper.MapPath("log4net.config"));

        /// <summary>
        /// log4net init
        /// </summary>
        /// <param name="configFilePath">log4net config file path</param>
        /// <returns>1 config success,0 config has existed</returns>
        public static int LogInit(string configFilePath)
        {
            if (null == LogManager.GetAllRepositories()?.FirstOrDefault(_ => _.Name == ApplicationHelper.ApplicationName))
            {
                XmlConfigurator.ConfigureAndWatch(LogManager.CreateRepository(ApplicationHelper.ApplicationName), new FileInfo(configFilePath));
                return 1;
            }
            return 0;
        }
    }
}
