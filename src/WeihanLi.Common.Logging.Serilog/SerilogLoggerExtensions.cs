using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;
using ILogger = Serilog.ILogger;

namespace WeihanLi.Common.Logging.Serilog;

public static class SerilogLoggerExtensions
{
    /// <summary>
    /// Add Serilog to the logging pipeline.
    /// </summary>
    /// <param name="factory">The logger factory to configure.</param>
    /// <param name="logger">The Serilog logger; if not supplied, the static <see cref="Log"/> will be used.</param>
    /// <param name="dispose">When true, dispose <paramref name="logger"/> when the framework disposes the provider. If the
    /// logger is not specified but <paramref name="dispose"/> is true, the <see cref="Log.CloseAndFlush()"/> method will be
    /// called on the static <see cref="Log"/> class instead.</param>
    /// <returns>The logger factory.</returns>
    public static ILoggerFactory AddSerilog(
        this ILoggerFactory factory,
        ILogger? logger = null,
        bool dispose = false)
    {
        if (factory == null) throw new ArgumentNullException(nameof(factory));

        factory.AddProvider(new SerilogLoggerProvider(logger, dispose));

        return factory;
    }

    /// <summary>
    /// Add Serilog to the logging pipeline.
    /// </summary>
    /// <param name="builder">The <see cref="T:Microsoft.Extensions.Logging.ILoggingBuilder" /> to add logging provider to.</param>
    /// <param name="logger">The Serilog logger; if not supplied, the static <see cref="Log"/> will be used.</param>
    /// <param name="dispose">When true, dispose <paramref name="logger"/> when the framework disposes the provider. If the
    /// logger is not specified but <paramref name="dispose"/> is true, the <see cref="Log.CloseAndFlush()"/> method will be
    /// called on the static <see cref="Log"/> class instead.</param>
    /// <returns>The logging builder.</returns>
    public static ILoggingBuilder AddSerilog(this ILoggingBuilder builder, ILogger? logger = null, bool dispose = false)
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        if (dispose)
        {
            builder.Services.AddSingleton<ILoggerProvider, SerilogLoggerProvider>(services => new SerilogLoggerProvider(logger, true));
        }
        else
        {
            builder.AddProvider(new SerilogLoggerProvider(logger));
        }

        return builder;
    }
}
