namespace WeihanLi.Common.Helpers;

/// <summary>
/// A singleton disposable that does nothing when disposed.
/// </summary>
public sealed class NullDisposable : IDisposable, IAsyncDisposable
{
    private NullDisposable()
    {
    }

    public void Dispose()
    {
    }

    public ValueTask DisposeAsync() => 
#if NET6_0_OR_GREATER
        ValueTask.CompletedTask
#else
        default
#endif
        ;

    /// <summary>
    /// Gets the instance of <see cref="NullDisposable"/>.
    /// </summary>
    public static NullDisposable Instance { get; } = new();
}

public sealed class DisposableAction : IDisposable, IAsyncDisposable
{
    public static readonly DisposableAction Empty = new(null);

    private Action? _disposeAction;

    public DisposableAction(Action? disposeAction)
    {
        _disposeAction = disposeAction;
    }

    public void Dispose()
    {
        Interlocked.Exchange(ref _disposeAction, null)?.Invoke();
    }

    public ValueTask DisposeAsync()
    {
        Dispose();
        return 
#if NET6_0_OR_GREATER
            ValueTask.CompletedTask
#else
            default
#endif
        ;
    }
}

/// <summary>
/// Disposable base class
/// https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-dispose
/// https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-disposeasync
/// https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-disposeasync#implement-both-dispose-and-async-dispose-patterns
/// </summary>
public class DisposableBase : IDisposable, IAsyncDisposable
{
    // To detect redundant calls
    private bool _disposed;
    public void Dispose()
    {
        if (_disposed) return;

        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;
        // Perform async cleanup.
        await DisposeAsyncCore().ConfigureAwait(false);
        // Dispose of unmanaged resources.
        Dispose(disposing: false);
        // Suppress finalization.
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            // dispose managed state (managed objects)
        }

        // free unmanaged resources (unmanaged objects) and override finalizer
        // set large fields to null
        _disposed = true;
    }

    protected virtual ValueTask DisposeAsyncCore()
    {
        return 
#if NET6_0_OR_GREATER
            ValueTask.CompletedTask
#else
            default
#endif
        ;
    }

    ~DisposableBase() => Dispose(false);
}
