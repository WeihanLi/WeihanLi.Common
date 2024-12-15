// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

namespace WeihanLi.Common.Helpers.Hosting;

public abstract class BackgroundService : IHostedService, IDisposable
{
    private Task? _executeTask;
    private CancellationTokenSource? _stoppingCts;
    public virtual Task StartAsync(CancellationToken cancellationToken)
    {
        // Create linked token to allow cancelling executing task from provided token
        _stoppingCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        // Store the task we're executing
        _executeTask = ExecuteAsync(_stoppingCts.Token);

        // If the task is completed then return it, this will bubble cancellation and failure to the caller
        if (_executeTask.IsCompleted)
        {
            return _executeTask;
        }

        // Otherwise it's running
        return Task.CompletedTask;
    }

    public virtual async Task StopAsync(CancellationToken cancellationToken)
    {
        // Stop called without start
        if (_executeTask == null)
        {
            return;
        }

        try
        {
            // Signal cancellation to the executing method
#if NET
            await _stoppingCts!.CancelAsync();
#else
            _stoppingCts!.Cancel();
#endif
        }
        finally
        {
            // Wait until the task completes or the stop token triggers
            var tcs = new TaskCompletionSource<object>();
            using var registration = cancellationToken.Register(s => ((TaskCompletionSource<object>)s!).TrySetCanceled(), tcs);
            // Do not await the _executeTask because cancelling it will throw an OperationCanceledException which we are explicitly ignoring
            await Task.WhenAny(_executeTask, tcs.Task).ConfigureAwait(false);
        }
    }

    protected abstract Task ExecuteAsync(CancellationToken stoppingToken);

    public virtual void Dispose()
    {
        _stoppingCts?.Cancel(false);
    }
}

public abstract class BackgroundServiceWithLifecycle : BackgroundService, IHostedLifecycleService
{
    public virtual Task StartingAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public virtual Task StartedAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public virtual Task StoppingAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public virtual Task StoppedAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
