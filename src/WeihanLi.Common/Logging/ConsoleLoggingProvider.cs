using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Concurrent;
using System.Threading;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Logging
{
    public interface IConsoleLogFormatter
    {
        string FormatAsString(LogHelperLoggingEvent loggingEvent);
    }

    internal sealed class DefaultConsoleLogFormatter : IConsoleLogFormatter
    {
        private static readonly JsonSerializerSettings _serializerSettings = new()
        {
            Converters =
            {
                new StringEnumConverter()
            },
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        };

        public string FormatAsString(LogHelperLoggingEvent loggingEvent)
        {
            return loggingEvent.ToJson(_serializerSettings);
        }
    }

    internal sealed class ConsoleLoggingProvider : ILogHelperProvider
    {
        private readonly IConsoleLogFormatter _formatter;

        private readonly BlockingCollection<LogHelperLoggingEvent> _messageQueue = new();
        private readonly Thread _outputThread;

        public ConsoleLoggingProvider(IConsoleLogFormatter formatter)
        {
            _formatter = formatter;

            // Start Console message queue processor
            _outputThread = new Thread(ProcessLogQueue)
            {
                IsBackground = true,
                Name = "Console logger queue processing thread"
            };
            _outputThread.Start();
        }

        public void EnqueueMessage(LogHelperLoggingEvent message)
        {
            if (!_messageQueue.IsAddingCompleted)
            {
                try
                {
                    _messageQueue.Add(message);
                    return;
                }
                catch (InvalidOperationException) { }
            }

            // Adding is completed so just log the message
            try
            {
                WriteLoggingEvent(message);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public void Log(LogHelperLoggingEvent loggingEvent)
        {
            EnqueueMessage(loggingEvent);
        }

        private void ProcessLogQueue()
        {
            try
            {
                foreach (LogHelperLoggingEvent message in _messageQueue.GetConsumingEnumerable())
                {
                    WriteLoggingEvent(message);
                }
            }
            catch
            {
                try
                {
                    _messageQueue.CompleteAdding();
                }
                catch
                {
                    // ignored
                }
            }
        }

        private void WriteLoggingEvent(LogHelperLoggingEvent loggingEvent)
        {
            try
            {
                var originalColor = Console.ForegroundColor;
                try
                {
                    var log = _formatter.FormatAsString(loggingEvent);
                    var logLevelColor = GetLogLevelConsoleColor(loggingEvent.LogLevel);
                    Console.ForegroundColor = logLevelColor.GetValueOrDefault(originalColor);

                    if (loggingEvent.LogLevel == LogHelperLogLevel.Error
                        || loggingEvent.LogLevel == LogHelperLogLevel.Fatal)
                    {
                        Console.Error.WriteLine(log);
                    }
                    else
                    {
                        Console.WriteLine(log);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                finally
                {
                    Console.ForegroundColor = originalColor;
                }
            }
            catch
            {
                Console.WriteLine(loggingEvent.ToJson());
            }
        }

        private static ConsoleColor? GetLogLevelConsoleColor(LogHelperLogLevel logLevel)
        {
            return logLevel switch
            {
                LogHelperLogLevel.Trace => ConsoleColor.Gray,
                LogHelperLogLevel.Debug => ConsoleColor.Gray,
                LogHelperLogLevel.Info => ConsoleColor.DarkGreen,
                LogHelperLogLevel.Warn => ConsoleColor.Yellow,
                LogHelperLogLevel.Error => ConsoleColor.Red,
                LogHelperLogLevel.Fatal => ConsoleColor.DarkRed,
                _ => null
            };
        }
    }

    public static class ConsoleLoggingProviderExtensions
    {
        public static ILogHelperLoggingBuilder AddConsole(this ILogHelperLoggingBuilder loggingBuilder, IConsoleLogFormatter? consoleLogFormatter = null)
        {
            loggingBuilder.AddProvider(new ConsoleLoggingProvider(
                consoleLogFormatter ?? new DefaultConsoleLogFormatter()));
            return loggingBuilder;
        }
    }
}
