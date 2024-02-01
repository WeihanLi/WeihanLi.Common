using System.Diagnostics.CodeAnalysis;
using WeihanLi.Common.Helpers;

namespace WeihanLi.Common.Logging;

/// <summary>
/// LogHelperExtensions
/// </summary>
public static class LogHelperExtensions
{
    public static void Log(this ILogHelperLogger logger, LogHelperLogLevel loggerLevel, string? msg) => logger.Log(loggerLevel, null, msg);

    #region Info

    public static void Info(this ILogHelperLogger logger, string? msg, params object[] parameters)
    {
        logger.Log(LogHelperLogLevel.Info, null, msg, parameters);
    }

    public static void Info(this ILogHelperLogger logger, Exception? ex, string? msg) => logger.Log(LogHelperLogLevel.Info, ex, msg);

    public static void Info(this ILogHelperLogger logger, Exception? ex) => logger.Log(LogHelperLogLevel.Info, ex, ex?.Message);

    #endregion Info

    #region Trace

    public static void Trace(this ILogHelperLogger logger, string? msg, params object[] parameters)
    {
        logger.Log(LogHelperLogLevel.Trace, null, msg, parameters);
    }

    public static void Trace(this ILogHelperLogger logger, Exception? ex, string? msg) => logger.Log(LogHelperLogLevel.Trace, ex, msg);

    public static void Trace(this ILogHelperLogger logger, Exception? ex) => logger.Log(LogHelperLogLevel.Trace, ex, ex?.Message);

    #endregion Trace

    #region Debug

    public static void Debug(this ILogHelperLogger logger, string? msg, params object[] parameters)
    {
        logger.Log(LogHelperLogLevel.Debug, null, msg, parameters);
    }

    public static void Debug(this ILogHelperLogger logger, Exception? ex, string? msg) => logger.Log(LogHelperLogLevel.Debug, ex, msg);

    public static void Debug(this ILogHelperLogger logger, Exception? ex) => logger.Log(LogHelperLogLevel.Debug, ex, ex?.Message);

    #endregion Debug

    #region Warn

    public static void Warn(this ILogHelperLogger logger, string? msg, params object[] parameters)
    {
        logger.Log(LogHelperLogLevel.Warn, null, msg, parameters);
    }

    public static void Warn(this ILogHelperLogger logger, Exception? ex, string? msg) => logger.Log(LogHelperLogLevel.Warn, ex, msg);

    public static void Warn(this ILogHelperLogger logger, Exception? ex) => logger.Log(LogHelperLogLevel.Warn, ex, ex?.Message);

    #endregion Warn

    #region Error

    public static void Error(this ILogHelperLogger logger, string? msg, params object[] parameters)
    {
        logger.Log(LogHelperLogLevel.Error, null, msg, parameters);
    }

    public static void Error(this ILogHelperLogger logger, Exception? ex, string? msg) => logger.Log(LogHelperLogLevel.Error, ex, msg);

    public static void Error(this ILogHelperLogger logger, Exception? ex) => logger.Log(LogHelperLogLevel.Error, ex, ex?.Message);

    #endregion Error

    #region Fatal

    public static void Fatal(this ILogHelperLogger logger, string? msg, params object[] parameters)
    {
        logger.Log(LogHelperLogLevel.Fatal, null, msg, parameters);
    }

    public static void Fatal(this ILogHelperLogger logger, Exception? ex, string? msg) => logger.Log(LogHelperLogLevel.Fatal, ex, msg);

    public static void Fatal(this ILogHelperLogger logger, Exception? ex) => logger.Log(LogHelperLogLevel.Fatal, ex, ex?.Message);

    #endregion Fatal

    #region ILogHelperFactory

    public static ILogHelperLogger GetLogger<T>(this ILogHelperFactory logHelperFactory) =>
        GetLogger(logHelperFactory, typeof(T));

    public static ILogHelperLogger GetLogger(this ILogHelperFactory logHelperFactory, Type type)
    {
        Guard.NotNull(logHelperFactory, nameof(logHelperFactory));

        return logHelperFactory.CreateLogger(type.FullName ?? type.Name);
    }

    #endregion ILogHelperFactory

    #region ILogHelperLoggingBuilder

    public static ILogHelperLoggingBuilder WithMinimumLevel(this ILogHelperLoggingBuilder loggingBuilder, LogHelperLogLevel logLevel)
    {
        Guard.NotNull(loggingBuilder, nameof(loggingBuilder));

        return loggingBuilder.WithFilter(level => level >= logLevel);
    }

    public static ILogHelperLoggingBuilder WithFilter(this ILogHelperLoggingBuilder loggingBuilder, Func<LogHelperLogLevel, bool> filterFunc)
    {
        loggingBuilder.AddFilter((_, e) => filterFunc.Invoke(e.LogLevel));
        return loggingBuilder;
    }

    public static ILogHelperLoggingBuilder WithFilter(this ILogHelperLoggingBuilder loggingBuilder, Func<string, LogHelperLogLevel, bool> filterFunc)
    {
        Guard.NotNull(loggingBuilder, nameof(loggingBuilder));

        loggingBuilder.AddFilter((_, e) => filterFunc.Invoke(e.CategoryName, e.LogLevel));
        return loggingBuilder;
    }

    public static ILogHelperLoggingBuilder WithFilter(this ILogHelperLoggingBuilder loggingBuilder, Func<Type, string, LogHelperLogLevel, bool> filterFunc)
    {
        Guard.NotNull(loggingBuilder, nameof(loggingBuilder));

        loggingBuilder.AddFilter((type, e) => filterFunc.Invoke(type, e.CategoryName, e.LogLevel));
        return loggingBuilder;
    }

    public static ILogHelperLoggingBuilder WithFilter(this ILogHelperLoggingBuilder loggingBuilder, Func<Type, string, LogHelperLogLevel, Exception?, bool> filterFunc)
    {
        Guard.NotNull(loggingBuilder, nameof(loggingBuilder));

        loggingBuilder.AddFilter((type, e) => filterFunc.Invoke(type, e.CategoryName, e.LogLevel, e.Exception));
        return loggingBuilder;
    }

    public static ILogHelperLoggingBuilder WithFilter(this ILogHelperLoggingBuilder loggingBuilder, Func<Type, LogHelperLoggingEvent, bool> filterFunc)
    {
        Guard.NotNull(loggingBuilder, nameof(loggingBuilder));

        loggingBuilder.AddFilter(filterFunc);
        return loggingBuilder;
    }

    public static ILogHelperLoggingBuilder WithProvider(this ILogHelperLoggingBuilder loggingBuilder, ILogHelperProvider logHelperProvider)
    {
        Guard.NotNull(loggingBuilder, nameof(loggingBuilder));

        loggingBuilder.AddProvider(logHelperProvider);
        return loggingBuilder;
    }

    public static ILogHelperLoggingBuilder WithProvider<TLogProvider>(this ILogHelperLoggingBuilder loggingBuilder) where TLogProvider : ILogHelperProvider, new()
    {
        Guard.NotNull(loggingBuilder, nameof(loggingBuilder));

        loggingBuilder.AddProvider(new TLogProvider());
        return loggingBuilder;
    }

    public static ILogHelperLoggingBuilder WithProvider<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TLogProvider>(this ILogHelperLoggingBuilder loggingBuilder, params object[] ctorParams) where TLogProvider : ILogHelperProvider
    {
        Guard.NotNull(loggingBuilder, nameof(loggingBuilder));

        loggingBuilder.AddProvider(ActivatorHelper.CreateInstance<TLogProvider>(ctorParams));
        return loggingBuilder;
    }

    public static ILogHelperLoggingBuilder WithEnricher<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TEnricher>(this ILogHelperLoggingBuilder loggingBuilder,
        TEnricher enricher) where TEnricher : ILogHelperLoggingEnricher
    {
        Guard.NotNull(loggingBuilder, nameof(loggingBuilder));

        loggingBuilder.AddEnricher(enricher);
        return loggingBuilder;
    }

    public static ILogHelperLoggingBuilder WithEnricher<TEnricher>(this ILogHelperLoggingBuilder loggingBuilder) where TEnricher : ILogHelperLoggingEnricher, new()
    {
        Guard.NotNull(loggingBuilder, nameof(loggingBuilder));

        loggingBuilder.AddEnricher(new TEnricher());
        return loggingBuilder;
    }

    public static ILogHelperLoggingBuilder WithEnricher<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TEnricher>(this ILogHelperLoggingBuilder loggingBuilder, params object[] ctorParams) where TEnricher : ILogHelperLoggingEnricher
    {
        loggingBuilder.AddEnricher(ActivatorHelper.CreateInstance<TEnricher>(ctorParams));
        return loggingBuilder;
    }

    public static ILogHelperLoggingBuilder EnrichWithProperty(this ILogHelperLoggingBuilder loggingBuilder, string propertyName, object value, bool overwrite = false)
    {
        loggingBuilder.AddEnricher(new PropertyLoggingEnricher(propertyName, value, overwrite));
        return loggingBuilder;
    }

    public static ILogHelperLoggingBuilder EnrichWithProperty(this ILogHelperLoggingBuilder loggingBuilder, string propertyName, Func<LogHelperLoggingEvent> valueFactory, bool overwrite = false)
    {
        Guard.NotNull(loggingBuilder, nameof(loggingBuilder));

        loggingBuilder.AddEnricher(new PropertyLoggingEnricher(propertyName, valueFactory, overwrite));
        return loggingBuilder;
    }

    public static ILogHelperLoggingBuilder EnrichWithProperty(this ILogHelperLoggingBuilder loggingBuilder, string propertyName, object value, Func<LogHelperLoggingEvent, bool> predict, bool overwrite = false)
    {
        Guard.NotNull(loggingBuilder, nameof(loggingBuilder));

        loggingBuilder.AddEnricher(new PropertyLoggingEnricher(propertyName, _ => value, predict, overwrite));
        return loggingBuilder;
    }

    public static ILogHelperLoggingBuilder EnrichWithProperty(this ILogHelperLoggingBuilder loggingBuilder, string propertyName, Func<LogHelperLoggingEvent, object> valueFactory, Func<LogHelperLoggingEvent, bool> predict, bool overwrite = false)
    {
        Guard.NotNull(loggingBuilder, nameof(loggingBuilder));

        loggingBuilder.AddEnricher(new PropertyLoggingEnricher(propertyName, valueFactory, predict, overwrite));
        return loggingBuilder;
    }

    #endregion ILogHelperLoggingBuilder

    #region LoggingEnricher

    public static void AddProperty(this LogHelperLoggingEvent loggingEvent, string propertyName,
        object propertyValue, bool overwrite = false)
    {
        Guard.NotNull(loggingEvent, nameof(loggingEvent));

        loggingEvent.Properties ??= [];
        if (loggingEvent.Properties.ContainsKey(propertyName) && !overwrite)
        {
            return;
        }

        loggingEvent.Properties[propertyName] = propertyValue;
    }

    public static void AddProperty(this LogHelperLoggingEvent loggingEvent, string propertyName,
        Func<LogHelperLoggingEvent, object> propertyValueFactory, bool overwrite = false)
    {
        Guard.NotNull(loggingEvent, nameof(loggingEvent));
        Guard.NotNull(propertyValueFactory, nameof(propertyValueFactory));

        loggingEvent.Properties ??= [];

        if (loggingEvent.Properties.ContainsKey(propertyName) && !overwrite)
        {
            return;
        }

        loggingEvent.Properties[propertyName] = propertyValueFactory.Invoke(loggingEvent);
    }

    #endregion LoggingEnricher
}
