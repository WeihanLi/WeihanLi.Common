using System;
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

        /// <summary>
        /// Get a log4net logger
        /// </summary>
        public static ILog GetLogger<TCategory>()
        {
            return LogManager.GetLogger(ApplicationHelper.ApplicationName, typeof(TCategory));
        }

        /// <summary>
        /// Get a log4net logger
        /// </summary>
        public static ILog GetLogger(Type type)
        {
            return LogManager.GetLogger(ApplicationHelper.ApplicationName, type);
        }

        /// <summary>
        /// Get a log4net logger
        /// </summary>
        public static ILog GetLogger(string loggerName)
        {
            return LogManager.GetLogger(ApplicationHelper.ApplicationName, loggerName);
        }
    }
}
