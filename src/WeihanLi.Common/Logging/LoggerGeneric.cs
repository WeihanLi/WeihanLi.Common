using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WeihanLi.Common.Helpers;

namespace WeihanLi.Common.Logging;

public sealed class GenericLoggerOptions
{
    public Func<Type, bool>? FullNamePredict { get; set; }
}

internal sealed class GenericLogger<T> : ILogger<T>
{
    private readonly ILogger _logger;

    /// <summary>
    /// Creates a new <see cref="GenericLogger{T}"/>.
    /// </summary>
    /// <param name="factory">The factory.</param>
    /// <param name="options">GenericLoggerOptions</param>
    public GenericLogger(ILoggerFactory factory, IOptions<GenericLoggerOptions> options)
    {
        if (factory == null)
        {
            throw new ArgumentNullException(nameof(factory));
        }

        var includeGenericParameters = options.Value.FullNamePredict?.Invoke(typeof(T)) == true;
        _logger = factory.CreateLogger(TypeHelper.GetTypeDisplayName(typeof(T), includeGenericParameters: includeGenericParameters, nestedTypeDelimiter: '.'));
    }

    /// <inheritdoc />
#if NET7_0_OR_GREATER
    IDisposable?
#else
    IDisposable
#endif
        ILogger.BeginScope<TState>(TState state)
    {
        return _logger.BeginScope(state);
    }

    /// <inheritdoc />
    bool ILogger.IsEnabled(LogLevel logLevel)
    {
        return _logger.IsEnabled(logLevel);
    }

    /// <inheritdoc />
    void ILogger.Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        _logger.Log(logLevel, eventId, state, exception, formatter);
    }
}
