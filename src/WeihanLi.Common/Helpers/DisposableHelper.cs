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

/// <summary>
/// Delegate-based Disposable implement
/// </summary>
/// <param name="disposeAction">dispose delegate</param>
public sealed class DisposableAction(Action? disposeAction) : IDisposable, IAsyncDisposable
{
    public void Dispose()
    {
        Interlocked.Exchange(ref disposeAction, null)?.Invoke();
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
public abstract class DisposableBase : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Set to <c>true</c> when the inherits place dispose logics in <c>Dispose</c> method other than <c>DisposeAsync</c>
    /// </summary>
    protected virtual bool UseDispose { get; }

    // To detect redundant calls
    private int _disposedStatus;

    public void Dispose()
    {
        if (Interlocked.CompareExchange(ref _disposedStatus, 1, 0) != 0) return;

        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        if (Interlocked.CompareExchange(ref _disposedStatus, 1, 0) != 0) return;

        // Perform async cleanup.
        await DisposeAsyncCore().ConfigureAwait(false);

        if (UseDispose)
        {
            Dispose();
        }
        else
        {
            // Dispose of unmanaged resources.
            Dispose(disposing: false);
        }

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
    }

    protected virtual ValueTask DisposeAsyncCore()
    {
        // dispose managed state in async way (managed objects)
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
