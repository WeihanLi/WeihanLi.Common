using System;
using System.Collections.Concurrent;
using JetBrains.Annotations;
using WeihanLi.Common.Helpers;

namespace WeihanLi.Common.Logging.Log4Net
{
    internal class Log4NetLogHelperProvider : ILogHelperProvider
    {
        private readonly ConcurrentDictionary<string, Log4NetLogHelperLogger> _loggers =
            new ConcurrentDictionary<string, Log4NetLogHelperLogger>(StringComparer.Ordinal);

        public Log4NetLogHelperProvider() : this(ApplicationHelper.MapPath("log4net.config"))
        {
        }

        public Log4NetLogHelperProvider(string configurationFilePath) => Log4NetHelper.LogInit(configurationFilePath);

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
