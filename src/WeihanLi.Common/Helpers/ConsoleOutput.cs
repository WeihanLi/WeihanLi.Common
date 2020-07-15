using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace WeihanLi.Common.Helpers
{
    public class ConsoleOutput : IDisposable
    {
        private TextWriter _originalOutputWriter;
        private TextWriter _originalErrorWriter;
        private readonly StringWriter _outputWriter = new StringWriter();
        private readonly StringWriter _errorWriter = new StringWriter();

        private const int NOT_DISPOSED = 0;
        private const int DISPOSED = 1;

        private int _alreadyDisposed = NOT_DISPOSED;

        private static readonly SemaphoreSlim _consoleLock = new SemaphoreSlim(1, 1);

        private ConsoleOutput()
        {
        }

        public static async Task<ConsoleOutput> Capture()
        {
            var redirector = new ConsoleOutput();
            await _consoleLock.WaitAsync();

            try
            {
                redirector._originalOutputWriter = Console.Out;
                redirector._originalErrorWriter = Console.Error;

                Console.SetOut(redirector._outputWriter);
                Console.SetError(redirector._errorWriter);
            }
            finally
            {
                _consoleLock.Release();
            }

            return redirector;
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

                _consoleLock.Release();
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
}
