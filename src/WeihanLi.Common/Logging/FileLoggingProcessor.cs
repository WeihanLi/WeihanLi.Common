// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Text;
using WeihanLi.Common.Helpers;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Logging;

internal sealed class FileLoggingProcessor : DisposableBase
{
    private readonly FileLoggingOptions _options;
    private readonly BlockingCollection<(string log, DateTimeOffset timestamp)> _messageQueue = [];
    private readonly Thread _outputThread;

    private StreamWriter? _fileStream;
    private string? _logFileName;

    public FileLoggingProcessor(FileLoggingOptions options)
    {
        if (!Directory.Exists(options.LogsDirectory))
        {
            try
            {
                Directory.CreateDirectory(options.LogsDirectory);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Failed to create log directory", e);
            }
        }

        _options = options;
        _outputThread = new Thread(ProcessLogQueue)
        {
            IsBackground = true,
            Priority = ThreadPriority.BelowNormal,
            Name = "FileLoggingProcessor"
        };
        _outputThread.Start();
    }

    public void EnqueueLog(string log, DateTimeOffset dateTimeOffset)
    {
        if (_messageQueue.IsAddingCompleted) return;

        try
        {
            _messageQueue.Add((log, dateTimeOffset));
        }
        catch (InvalidOperationException) { }
    }

    protected override void Dispose(bool disposing)
    {
        if (!disposing) return;

        _messageQueue.CompleteAdding();
        _fileStream?.Flush();
        _fileStream?.Dispose();
        _messageQueue.Dispose();
    }

    private void ProcessLogQueue()
    {
        try
        {
            foreach (var message in _messageQueue.GetConsumingEnumerable())
            {
                WriteLoggingEvent(message.log, message.timestamp);
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

    private void WriteLoggingEvent(string log, DateTimeOffset timestamp)
    {
        var fileName = _options.FileFormat.Replace("{date}", timestamp.ToString("yyyyMMdd"));
        var fileInfo = new FileInfo(Path.Combine(_options.LogsDirectory, fileName));

        try
        {
            var previousFileName = Interlocked.CompareExchange(ref _logFileName, fileInfo.FullName, _logFileName);
            if (_logFileName != previousFileName)
            {
                // file name changed
                var fs = File.CreateText(fileInfo.FullName);
                var originalWriter = Interlocked.Exchange(ref _fileStream, fs);
                if (originalWriter is not null)
                {
                    originalWriter.Flush();
                    originalWriter.Dispose();
                }
            }

            Guard.NotNull(_fileStream);
            _fileStream.Write(log);
            _fileStream.Flush();
        }
        catch (Exception ex)
        {
            Console.WriteLine($@"Error when trying to log to file({fileInfo.FullName}) \n" + log + Environment.NewLine + ex);
        }
    }
}
