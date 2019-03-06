using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using log4net;
using log4net.Config;
using WeihanLi.Common.Helpers;

namespace WeihanLi.Common.Logging.Log4Net
{
    public class Log4NetLogHelperProvider : ILogHelperProvider
    {
        private readonly ConcurrentDictionary<string, Log4NetLogHelperLogger> _loggers =
            new ConcurrentDictionary<string, Log4NetLogHelperLogger>(StringComparer.Ordinal);

        public Log4NetLogHelperProvider() : this(ApplicationHelper.MapPath("log4net.config"))
        {
        }

        public Log4NetLogHelperProvider(string configurationFilePath)
        {
            if (null == LogManager.GetAllRepositories().FirstOrDefault(_ => _.Name == ApplicationHelper.ApplicationName))
            {
                XmlConfigurator.ConfigureAndWatch(LogManager.CreateRepository(ApplicationHelper.ApplicationName),
                    new FileInfo(configurationFilePath));
            }
        }

        public ILogHelperLogger CreateLogger(string categoryName) => _loggers.GetOrAdd(categoryName, loggerName => new Log4NetLogHelperLogger(loggerName));
    }

    public static class LogHelperFactoryExtensions
    {
        public static ILogHelperFactory AddLog4Net([NotNull]this ILogHelperFactory logHelperFactory)
        {
            logHelperFactory.AddProvider(new Log4NetLogHelperProvider());
            return logHelperFactory;
        }

        public static ILogHelperFactory AddLog4Net([NotNull] this ILogHelperFactory logHelperFactory,
            string configFilePath)
        {
            logHelperFactory.AddProvider(new Log4NetLogHelperProvider(configFilePath));
            return logHelperFactory;
        }
    }
}
