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

/// <summary>
/// Base class for sinks that log events in batches. Batching is
/// triggered asynchronously on a timer.
/// </summary>
/// <remarks>
/// To avoid unbounded memory growth, events are discarded after attempting
/// to send a batch, regardless of whether the batch succeeded or not. Implementers
/// that want to change this behavior need to either implement from scratch, or
/// embed retry logic in the batch emitting functions.
/// </remarks>
public abstract class PeriodicBatching<TEvent> : IDisposable where TEvent : class
{
    private readonly int _batchSizeLimit;
    private readonly BoundedConcurrentQueue<TEvent> _queue;
    private readonly BatchedConnectionStatus _status;
    private readonly Queue<TEvent> _waitingBatch = new();
    private readonly object _stateLock = new();
    private readonly PortableTimer _timer;
    private bool _unloading;
    private bool _started;

    /// <summary>
    /// Construct a sink posting to the specified database.
    /// </summary>
    /// <param name="batchSizeLimit">The maximum number of events to include in a single batch.</param>
    /// <param name="period">The time to wait between checking for event batches.</param>
    protected PeriodicBatching(int batchSizeLimit, TimeSpan period)
    {
        _batchSizeLimit = batchSizeLimit;
        _queue = new BoundedConcurrentQueue<TEvent>();
        _status = new BatchedConnectionStatus(period);

        _timer = new PortableTimer(_ => OnTick());
    }

    /// <summary>
    /// Construct a sink posting to the specified database.
    /// </summary>
    /// <param name="batchSizeLimit">The maximum number of events to include in a single batch.</param>
    /// <param name="period">The time to wait between checking for event batches.</param>
    /// <param name="queueLimit">Maximum number of events in the queue.</param>
    protected PeriodicBatching(int batchSizeLimit, TimeSpan period, int queueLimit)
        : this(batchSizeLimit, period)
    {
        _queue = new BoundedConcurrentQueue<TEvent>(queueLimit);
    }

    private void CloseAndFlush()
    {
        lock (_stateLock)
        {
            if (!_started || _unloading)
                return;

            _unloading = true;
        }

        _timer.Dispose();

        // This is the place where SynchronizationContext.Current is unknown and can be != null
        // so we prevent possible deadlocks here for sync-over-async downstream implementations
        ResetSyncContextAndWait(OnTick);
    }

    private void ResetSyncContextAndWait(Func<Task> taskFactory)
    {
        var prevContext = SynchronizationContext.Current;
        SynchronizationContext.SetSynchronizationContext(null);
        try
        {
            taskFactory().Wait();
        }
        finally
        {
            SynchronizationContext.SetSynchronizationContext(prevContext);
        }
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    /// <filterpriority>2</filterpriority>
    public void Dispose()
    {
        Dispose(true);
    }

    /// <summary>
    /// Free resources held by the sink.
    /// </summary>
    /// <param name="disposing">If true, called because the object is being disposed; if false,
    /// the object is being disposed from the finalizer.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposing)
            return;

        CloseAndFlush();
    }

    /// <summary>
    /// Emit a batch of log events, running to completion synchronously.
    /// </summary>
    /// <param name="events">The events to emit.</param>
    /// <remarks>Override either <see cref="EmitBatch"/> or <see cref="EmitBatchAsync"/>,
    /// not both.</remarks>
    protected virtual void EmitBatch(IEnumerable<TEvent> events)
    {
    }

    /// <summary>
    /// Emit a batch of log events, running asynchronously.
    /// </summary>
    /// <param name="events">The events to emit.</param>
    /// <remarks>Override either <see cref="EmitBatchAsync"/> or <see cref="EmitBatch"/>,
    /// not both. </remarks>
#pragma warning disable 1998

    protected virtual async Task EmitBatchAsync(IEnumerable<TEvent> events)
#pragma warning restore 1998
    {
        EmitBatch(events);
    }

    private async Task OnTick()
    {
        try
        {
            bool batchWasFull;
            do
            {
                while (_waitingBatch.Count < _batchSizeLimit &&
                    _queue.TryDequeue(out var next))
                {
                    if (CanInclude(next))
                        _waitingBatch.Enqueue(next);
                }

                if (_waitingBatch.Count == 0)
                {
                    await OnEmptyBatchAsync();
                    return;
                }

                await EmitBatchAsync(_waitingBatch);

                batchWasFull = _waitingBatch.Count >= _batchSizeLimit;
                _waitingBatch.Clear();
                _status.MarkSuccess();
            }
            while (batchWasFull); // Otherwise, allow the period to elapse
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Exception while emitting periodic batch from {0}: {1}", this, ex);
            _status.MarkFailure();
        }
        finally
        {
            if (_status.ShouldDropBatch)
                _waitingBatch.Clear();

            if (_status.ShouldDropQueue)
            {
                while (_queue.TryDequeue(out _)) { }
            }

            lock (_stateLock)
            {
                if (!_unloading)
                    SetTimer(_status.NextInterval);
            }
        }
    }

    private void SetTimer(TimeSpan interval)
    {
        _timer.Start(interval);
    }

    /// <summary>
    /// Emit the provided log event to the sink. If the sink is being disposed or
    /// the app domain unloaded, then the event is ignored.
    /// </summary>
    /// <param name="event">Log event to emit.</param>
    /// <exception cref="ArgumentNullException">The event is null.</exception>
    /// <remarks>
    /// The sink implements the contract that any events whose Emit() method has
    /// completed at the time of sink disposal will be flushed (or attempted to,
    /// depending on app domain state).
    /// </remarks>
    public void Emit(TEvent @event)
    {
        Guard.NotNull(@event);

        if (_unloading)
            return;

        if (!_started)
        {
            lock (_stateLock)
            {
                if (_unloading) return;
                if (!_started)
                {
                    // Special handling to try to get the first event across as quickly
                    // as possible to show we're alive!
                    _queue.TryEnqueue(@event);
                    _started = true;
                    SetTimer(TimeSpan.Zero);
                    return;
                }
            }
        }

        _queue.TryEnqueue(@event);
    }

    /// <summary>
    /// Determine whether a queued log event should be included in the batch. If
    /// an override returns false, the event will be dropped.
    /// </summary>
    /// <param name="evt"></param>
    /// <returns></returns>
    protected virtual bool CanInclude(TEvent? evt)
    {
        return true;
    }

    /// <summary>
    /// Allows derived sinks to perform periodic work without requiring additional threads
    /// or timers (thus avoiding additional flush/shut-down complexity).
    /// </summary>
    /// <remarks>Override either <see cref="OnEmptyBatch"/> or <see cref="OnEmptyBatchAsync"/>,
    /// not both. </remarks>
    protected virtual void OnEmptyBatch()
    {
    }

    /// <summary>
    /// Allows derived sinks to perform periodic work without requiring additional threads
    /// or timers (thus avoiding additional flush/shut-down complexity).
    /// </summary>
    /// <remarks>Override either <see cref="OnEmptyBatchAsync"/> or <see cref="OnEmptyBatch"/>,
    /// not both. </remarks>
#pragma warning disable 1998

    protected virtual async Task OnEmptyBatchAsync()
#pragma warning restore 1998
    {
        OnEmptyBatch();
    }
}
