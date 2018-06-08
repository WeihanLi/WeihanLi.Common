using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using log4net;
using log4net.Config;
using WeihanLi.Common.Helpers;

namespace WeihanLi.Common.Log
{
    public interface ILogHelperProvider
    {
        ILogHelper CreateLogHelper(string categoryName);
    }

    internal class Log4NetLogHelperProvider : ILogHelperProvider
    {
        private readonly ConcurrentDictionary<string, Log4NetLogHelper> _loggers =
            new ConcurrentDictionary<string, Log4NetLogHelper>(StringComparer.Ordinal);

        public Log4NetLogHelperProvider(string configuartionFilePath)
        {
            if (null == LogManager.GetAllRepositories()?.FirstOrDefault(_ => _.Name == ApplicationHelper.ApplicationName))
            {
                XmlConfigurator.ConfigureAndWatch(LogManager.CreateRepository(ApplicationHelper.ApplicationName),
                    new FileInfo(configuartionFilePath));
            }
        }

        public ILogHelper CreateLogHelper(string categoryName) => _loggers.GetOrAdd(categoryName, loggerName => new Log4NetLogHelper(loggerName));
    }
}
