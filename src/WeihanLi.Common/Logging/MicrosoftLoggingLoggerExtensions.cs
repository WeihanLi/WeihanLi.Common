using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using System.Diagnostics;
using WeihanLi.Common;
using WeihanLi.Common.Helpers;
using WeihanLi.Common.Logging;
using WeihanLi.Common.Services;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Logging;

[ProviderAlias("Delegate")]
public sealed class DelegateLoggerProvider(Action<string, LogLevel, Exception?, string> logAction) : ILoggerProvider
{
    internal static ILoggerProvider Default { get; } = new DelegateLoggerProvider((category, level, exception, msg) =>
    {
        var (foregroundColor, backgroundColor) = GetConsoleColorForLogLevel(level);
        var levelText = GetLogLevelText(level);
        var dateTime = DateTimeOffset.Now;
        var message = @$"[{levelText}][{category}] {dateTime} {msg}";
        if (exception is not null)
        {
            message = $"{message}{Environment.NewLine}{exception}";
        }

        ConsoleHelper.WriteLineWithColor(message, foregroundColor, backgroundColor);
        if (level is LogLevel.Trace)
        {
            Trace.WriteLine(message);
        }
        
        return;

        static (ConsoleColor? ForegroundColor, ConsoleColor? BackgroundColor) GetConsoleColorForLogLevel(LogLevel logLevel)
            => logLevel switch
            {
                LogLevel.Trace or LogLevel.Debug => (ConsoleColor.DarkGray, ConsoleColor.Black),
                LogLevel.Information => (ConsoleColor.DarkGreen, ConsoleColor.Black),
                LogLevel.Warning => (ConsoleColor.Yellow, ConsoleColor.Black),
                LogLevel.Error => (ConsoleColor.Black, ConsoleColor.DarkRed),
                LogLevel.Critical => (ConsoleColor.White, ConsoleColor.DarkRed),
                _ => (null, null)
            };

        static string GetLogLevelText(LogLevel logLevel)
            => logLevel switch
            {
                LogLevel.Trace => "trce",
                LogLevel.Debug => "dbug",
                LogLevel.Information => "info",
                LogLevel.Warning => "warn",
                LogLevel.Error => "fail",
                LogLevel.Critical => "crit",
                _ => logLevel.ToString().ToLowerInvariant()
            };
    });

    private readonly ConcurrentDictionary<string, DelegateLogger> _loggers = new();

    public void Dispose()
    {
        _loggers.Clear();
    }

    public ILogger CreateLogger(string categoryName)
    {
        return _loggers.GetOrAdd(categoryName, category => new DelegateLogger(category, logAction));
    }

    private sealed class DelegateLogger(string categoryName, Action<string, LogLevel, Exception?, string> logAction) : ILogger
    {
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            var msg = formatter(state, exception);
            logAction.Invoke(categoryName, logLevel, exception, msg);
        }

        public bool IsEnabled(LogLevel logLevel) => true;

#if NET7_0_OR_GREATER
        IDisposable?
#else
        IDisposable
#endif
                ILogger.BeginScope<TState>(TState state) => NullScope.Instance;
    }
}

public static class LoggerExtensions
{
    #region Info

    public static void Info(this ILogger logger, string msg, params object[] parameters) => logger.LogInformation(msg, parameters);

    public static void Info(this ILogger logger, Exception ex, string msg) => logger.LogInformation(ex, msg);

    #endregion Info

    #region Trace

    public static void Trace(this ILogger logger, string msg, params object[] parameters) => logger.LogTrace(msg, parameters);

    public static void Trace(this ILogger logger, Exception ex, string msg) => logger.LogTrace(ex, msg);

    public static void Trace(this ILogger logger, Exception ex) => logger.LogTrace(ex, ex.Message);

    #endregion Trace

    #region Debug

    public static void Debug(this ILogger logger, string msg, params object[] parameters) => logger.LogDebug(msg, parameters);

    public static void Debug(this ILogger logger, Exception ex, string msg) => logger.LogDebug(ex, msg);

    public static void Debug(this ILogger logger, Exception ex) => logger.LogDebug(ex, ex.Message);

    #endregion Debug

    #region Warn

    public static void Warn(this ILogger logger, string msg, params object[] parameters) => logger.LogWarning(msg, parameters);

    public static void Warn(this ILogger logger, Exception ex, string msg) => logger.LogWarning(ex, msg);

    public static void Warn(this ILogger logger, Exception ex) => logger.LogWarning(ex, ex.Message);

    #endregion Warn

    #region Error

    public static void Error(this ILogger logger, string msg, params object[] parameters) => logger.LogError(msg, parameters);

    public static void Error(this ILogger logger, Exception ex, string msg) => logger.LogError(ex, msg);

    public static void Error(this ILogger logger, Exception ex) => logger.LogError(ex, ex.Message);

    #endregion Error

    #region Fatal

    public static void Fatal(this ILogger logger, string msg, params object[] parameters) => logger.LogCritical(msg, parameters);

    public static void Fatal(this ILogger logger, Exception ex, string msg) => logger.LogCritical(ex, msg);

    public static void Fatal(this ILogger logger, Exception ex) => logger.LogCritical(ex, ex.Message);

    #endregion Fatal

    #region LoggerFactory

    /// <summary>
    /// AddDelegateLoggerProvider
    /// </summary>
    /// <param name="loggerFactory">loggerFactory</param>
    /// <param name="logAction">logAction</param>
    /// <returns>loggerFactory</returns>
    public static ILoggerFactory AddDelegateLogger(this ILoggerFactory loggerFactory, Action<string, LogLevel, Exception?, string> logAction)
    {
        loggerFactory.AddProvider(new DelegateLoggerProvider(logAction));
        return loggerFactory;
    }

    #endregion LoggerFactory

    #region ILoggingBuilder

    public static ILoggingBuilder AddDefaultDelegateLogger(this ILoggingBuilder loggingBuilder)
    {
        return loggingBuilder.AddProvider(DelegateLoggerProvider.Default);
    }

    public static ILoggingBuilder AddDelegateLogger(this ILoggingBuilder loggingBuilder,
        Action<string, LogLevel, Exception?, string> logAction)
    {
        return loggingBuilder.AddProvider(new DelegateLoggerProvider(logAction));
    }

    public static ILoggingBuilder UseCustomGenericLogger(this ILoggingBuilder loggingBuilder, Action<GenericLoggerOptions> genericLoggerConfig)
    {
        Guard.NotNull(loggingBuilder, nameof(loggingBuilder));
        Guard.NotNull(genericLoggerConfig, nameof(genericLoggerConfig));
        loggingBuilder.Services.Configure(genericLoggerConfig);
        loggingBuilder.Services.AddSingleton(typeof(ILogger<>), typeof(GenericLogger<>));
        return loggingBuilder;
    }

    #endregion ILoggingBuilder
}
