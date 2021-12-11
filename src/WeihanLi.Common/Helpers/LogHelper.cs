using System;
using WeihanLi.Common.Logging;

namespace WeihanLi.Common.Helpers;

/// <summary>
/// LogHelper
/// Logging
/// </summary>
public static class LogHelper
{
    private static ILogHelperFactory LogFactory { get; set; } = NullLogHelperFactory.Instance;

    public static void ConfigureLogging(Action<ILogHelperLoggingBuilder> configureAction)
    {
        var loggingBuilder = new LogHelperLoggingBuilder();
        Guard.NotNull(configureAction, nameof(configureAction)).Invoke(loggingBuilder);
        LogFactory = loggingBuilder.Build();
    }

    public static ILogHelperLogger GetLogger<T>() => LogFactory.GetLogger(typeof(T));

    public static ILogHelperLogger GetLogger(Type type) => LogFactory.GetLogger(type);

    public static ILogHelperLogger GetLogger(string categoryName)
    {
        Guard.NotNullOrEmpty(categoryName, nameof(categoryName));
        return LogFactory.CreateLogger(categoryName);
    }
}
