using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace WeihanLi.Common.Helpers
{
    public class ProcessExecutor : IDisposable
    {
        public event EventHandler<int> OnExited;

        public event EventHandler<string> OutputDataReceived;

        public event EventHandler<string> ErrorDataReceived;

        private readonly Process _process;

        public ProcessExecutor(string exePath, string arguments) : this(new ProcessStartInfo(exePath, arguments))
        {
        }

        public ProcessExecutor(ProcessStartInfo startInfo)
        {
            _process = new Process()
            {
                StartInfo = startInfo,
                EnableRaisingEvents = true,
            };
            _process.StartInfo.UseShellExecute = false;
            _process.StartInfo.RedirectStandardOutput = true;
            _process.StartInfo.RedirectStandardInput = true;
            _process.StartInfo.RedirectStandardError = true;

            _process.OutputDataReceived += (sender, args) =>
            {
                OutputDataReceived?.Invoke(sender, args.Data);
            };
            _process.ErrorDataReceived += (sender, args) =>
            {
                ErrorDataReceived?.Invoke(sender, args.Data);
            };
        }

        public async Task SendInput(string input)
        {
            try
            {
                await _process.StandardInput.WriteAsync(input!);
            }
            catch (Exception e)
            {
                //
            }
        }

        public virtual int Execute()
        {
            _process.WaitForExit();
            OnExited?.Invoke(_process, _process.ExitCode);
            return _process.ExitCode;
        }

        public virtual async Task<int> ExecuteAsync()
        {
            var tcs = new TaskCompletionSource<int>();
            await Task.Run(() =>
            {
                _process.WaitForExit();
                tcs.TrySetResult(_process.ExitCode);
                OnExited?.Invoke(_process, _process.ExitCode);
                return _process.ExitCode;
            }).ConfigureAwait(false);
            return tcs.Task.Result;
        }

        public void Dispose()
        {
            _process.Dispose();
        }
    }
}
