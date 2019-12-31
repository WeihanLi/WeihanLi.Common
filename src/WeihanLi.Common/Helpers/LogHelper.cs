using System;
using WeihanLi.Common.Logging;

namespace WeihanLi.Common.Helpers
{
    /// <summary>
    /// LogHelper
    /// Logging
    /// </summary>
    public static class LogHelper
    {
        public static ILogHelperFactory LogFactory { get; private set; } = NullLogHelperFactory.Instance;

        public static void ConfigureLogging(Action<ILogHelperLoggingBuilder> configureAction)
        {
            var loggingBuilder = new LogHelperLoggingBuilder();
            configureAction?.Invoke(loggingBuilder);
            LogFactory = loggingBuilder.Build();
        }

        public static ILogHelperLogger GetLogger<T>() => LogFactory.GetLogger(typeof(T));

        public static ILogHelperLogger GetLogger(Type type) => LogFactory.GetLogger(type);

        public static ILogHelperLogger GetLogger(string categoryName)
        {
            return LogFactory.CreateLogger(categoryName);
        }
    }
}
