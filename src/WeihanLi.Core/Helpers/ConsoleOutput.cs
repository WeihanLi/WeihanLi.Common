// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

namespace WeihanLi.Common.Helpers;

public sealed class ConsoleOutput : IDisposable
{
    private TextWriter? _originalOutputWriter;
    private TextWriter? _originalErrorWriter;
    private readonly StringWriter _outputWriter = new();
    private readonly StringWriter _errorWriter = new();

    private const int NotDisposed = 0;
    private const int Disposed = 1;

    private int _alreadyDisposed = NotDisposed;

    private static readonly SemaphoreSlim ConsoleLock = new(1, 1);

    private ConsoleOutput()
    {
    }

    public static ConsoleOutput Capture()
    {
        ConsoleLock.Wait();
        return GetConsoleOutputInternal();
    }

    public static async Task<ConsoleOutput> CaptureAsync()
    {
        await ConsoleLock.WaitAsync();
        return GetConsoleOutputInternal();
    }

    public void Dispose()
    {
        if (Interlocked.CompareExchange(ref _alreadyDisposed, Disposed, NotDisposed) == NotDisposed)
        {
            if (_originalOutputWriter != null)
            {
                Console.SetOut(_originalOutputWriter);
            }
            if (_originalErrorWriter != null)
            {
                Console.SetError(_originalErrorWriter);
            }
            if (ConsoleLock.CurrentCount < 1)
            {
                ConsoleLock.Release();
            }
        }
    }

    public string StandardOutput => _outputWriter.ToString();

    public string StandardError => _errorWriter.ToString();

    public void Clear()
    {
        _outputWriter.GetStringBuilder().Clear();
        _errorWriter.GetStringBuilder().Clear();
    }

    private static ConsoleOutput GetConsoleOutputInternal()
    {
        var outputCapture = new ConsoleOutput();
        try
        {
            outputCapture._originalOutputWriter = Console.Out;
            outputCapture._originalErrorWriter = Console.Error;

            Console.SetOut(outputCapture._outputWriter);
            Console.SetError(outputCapture._errorWriter);
        }
        catch
        {
            ConsoleLock.Release();
            throw;
        }

        return outputCapture;

    }
}
