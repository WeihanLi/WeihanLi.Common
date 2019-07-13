using System;
using System.Collections.Concurrent;

namespace WeihanLi.Common.Logging.Serilog
{
    public class SerilogLogHelperProvider : ILogHelperProvider
    {
        private readonly ConcurrentDictionary<int, ILogHelperLogger> _loggers = new ConcurrentDictionary<int, ILogHelperLogger>();

        public ILogHelperLogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(1, t => new SerilogLogHelperLogger());
        }
    }

    internal class SerilogLogHelperLogger : ILogHelperLogger
    {
        public void Log(LogHelperLevel loggerLevel, string message, Exception exception)
        {
            throw new NotImplementedException();
        }

        public bool IsEnabled(LogHelperLevel loggerLevel)
        {
            throw new NotImplementedException();
        }
    }
}
