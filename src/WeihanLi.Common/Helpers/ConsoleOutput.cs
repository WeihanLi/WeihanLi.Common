namespace WeihanLi.Common.Helpers;

public sealed class ConsoleOutput : IDisposable
{
    private TextWriter? _originalOutputWriter;
    private TextWriter? _originalErrorWriter;
    private readonly StringWriter _outputWriter = new();
    private readonly StringWriter _errorWriter = new();

    private const int NOT_DISPOSED = 0;
    private const int DISPOSED = 1;

    private int _alreadyDisposed = NOT_DISPOSED;

    private static readonly SemaphoreSlim _consoleLock = new(1, 1);

    private ConsoleOutput()
    {
    }

    public static ConsoleOutput Capture()
    {
        var outputCapture = new ConsoleOutput();
        _consoleLock.Wait();

        try
        {
            outputCapture._originalOutputWriter = Console.Out;
            outputCapture._originalErrorWriter = Console.Error;

            Console.SetOut(outputCapture._outputWriter);
            Console.SetError(outputCapture._errorWriter);
        }
        catch
        {
            _consoleLock.Release();
            throw;
        }

        return outputCapture;
    }

    public static async Task<ConsoleOutput> CaptureAsync()
    {
        var outputCapture = new ConsoleOutput();
        await _consoleLock.WaitAsync();

        try
        {
            outputCapture._originalOutputWriter = Console.Out;
            outputCapture._originalErrorWriter = Console.Error;

            Console.SetOut(outputCapture._outputWriter);
            Console.SetError(outputCapture._errorWriter);
            return outputCapture;
        }
        finally
        {
            _consoleLock.Release();
        }
    }

    public void Dispose()
    {
        if (Interlocked.CompareExchange(ref _alreadyDisposed, DISPOSED, NOT_DISPOSED) == NOT_DISPOSED)
        {
            if (_originalOutputWriter != null)
            {
                Console.SetOut(_originalOutputWriter);
            }
            if (_originalErrorWriter != null)
            {
                Console.SetError(_originalErrorWriter);
            }
            if (_consoleLock.CurrentCount < 1)
            {
                _consoleLock.Release();
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
}
