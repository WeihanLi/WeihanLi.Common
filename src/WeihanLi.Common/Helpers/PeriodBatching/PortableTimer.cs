// Copyright 2013-2016 Serilog Contributors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Diagnostics;

namespace WeihanLi.Common.Helpers.PeriodBatching;

internal sealed class PortableTimer : IDisposable
{
    private readonly Lock _stateLock = new();
    private readonly Func<CancellationToken, Task> _onTick;
    private readonly CancellationTokenSource _cancel = new();
    private readonly Timer _timer;
    private bool _running;
    private bool _disposed;

    public PortableTimer(Func<CancellationToken, Task> onTick)
    {
        _onTick = Guard.NotNull(onTick);

        _timer = new Timer(_ => OnTick(), null, Timeout.Infinite, Timeout.Infinite);
    }

    public void Start(TimeSpan interval)
    {
        if (interval < TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(interval));

        lock (_stateLock)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(PortableTimer));

            _timer.Change(interval, Timeout.InfiniteTimeSpan);
        }
    }

    private async void OnTick()
    {
        try
        {
            lock (_stateLock)
            {
                if (_disposed)
                {
                    return;
                }

                // There's a little bit of raciness here, but it's needed to support the
                // current API, which allows the tick handler to reenter and set the next interval.

                if (_running)
                {
                    Monitor.Wait(_stateLock);

                    if (_disposed)
                    {
                        return;
                    }
                }

                _running = true;
            }

            if (!_cancel.Token.IsCancellationRequested)
            {
                await _onTick(_cancel.Token);
            }
        }
        catch (OperationCanceledException tcx)
        {
            Debug.WriteLine("The timer was canceled during invocation: {0}", tcx);
        }
        finally
        {
            lock (_stateLock)
            {
                _running = false;
                Monitor.PulseAll(_stateLock);
            }
        }
    }

    public void Dispose()
    {
        _cancel.Cancel();

        lock (_stateLock)
        {
            if (_disposed)
            {
                return;
            }

            while (_running)
            {
                Monitor.Wait(_stateLock);
            }

            _timer.Dispose();
            _disposed = true;
        }
    }
}
