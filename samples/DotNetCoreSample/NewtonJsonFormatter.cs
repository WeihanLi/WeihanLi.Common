// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace DotNetCoreSample;

public sealed class NewtonJsonFormatterOptions : ConsoleFormatterOptions
{
}

public sealed class NewtonJsonFormatter(IOptions<NewtonJsonFormatterOptions> options) : ConsoleFormatter(FormatterName)
{
    public const string FormatterName = "NewtonJson";

    private readonly NewtonJsonFormatterOptions _options = options.Value;

    public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider? scopeProvider, TextWriter textWriter)
    {
        var message = logEntry.Formatter(logEntry.State, logEntry.Exception);
        if (logEntry.Exception == null && message == null)
        {
            return;
        }

        JsonWriter writer = new JsonTextWriter(textWriter);
        writer.WriteStartObject();
        if (_options.TimestampFormat != null)
        {
            writer.WritePropertyName("Timestamp");
            var timestamp = _options.UseUtcTimestamp ? DateTimeOffset.UtcNow : DateTimeOffset.Now;
            var timestampText = timestamp.ToString(_options.TimestampFormat);
            writer.WriteValue(timestampText);
        }
        writer.WritePropertyName("Level");
        writer.WriteValue(logEntry.LogLevel.ToString());
        writer.WritePropertyName("EventId");
        writer.WriteValue(logEntry.EventId.ToString());
        writer.WritePropertyName(nameof(logEntry.Category));
        writer.WriteValue(logEntry.Category);
        writer.WritePropertyName("Message");
        writer.WriteValue(message);
        if (logEntry.Exception != null)
        {
            writer.WritePropertyName("Exception");
            writer.WriteValue(logEntry.Exception);
        }

        if (logEntry.State != null)
        {
            writer.WritePropertyName(nameof(logEntry.State));
            writer.WriteStartObject();
            writer.WritePropertyName("Message");
            writer.WriteValue(logEntry.State.ToString());
            if (logEntry.State is System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<string, object>> stateProperties)
            {
                foreach (var item in stateProperties)
                {
                    writer.WritePropertyName(item.Key);
                    writer.WriteValue(item.Value);
                }
            }
            writer.WriteEndObject();
        }

        writer.WriteEndObject();
        writer.Flush();
        textWriter.WriteLine();
    }
}

public static partial class LoggingBuilderExtensions
{
    public static ILoggingBuilder AddNewtonJsonConsole(this ILoggingBuilder loggingBuilder,
        Action<NewtonJsonFormatterOptions>? optionsConfigure = null)
    {
        loggingBuilder.AddConsole(options => options.FormatterName = NewtonJsonFormatter.FormatterName);
        loggingBuilder.AddConsoleFormatter<NewtonJsonFormatter, NewtonJsonFormatterOptions>();
        if (optionsConfigure != null)
        {
            loggingBuilder.Services.Configure(optionsConfigure);
        }
        return loggingBuilder;
    }
}

public static class NewtonJsonFormatterTest
{
    public static void MainTest()
    {
        var builder = Host.CreateEmptyApplicationBuilder(null);
        // builder.Logging.AddJsonConsole();
        builder.Logging.AddNewtonJsonConsole();
        using var host = builder.Build();
        host.Run();
    }
}
