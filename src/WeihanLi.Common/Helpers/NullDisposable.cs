namespace WeihanLi.Common.Helpers;

/// <summary>
/// A singleton disposable that does nothing when disposed.
/// </summary>
public sealed class NullDisposable : IDisposable
#if ValueTaskSupport
    , IAsyncDisposable
#endif
{
    private NullDisposable()
    {
    }

    public void Dispose()
    {
    }

#if ValueTaskSupport
    public ValueTask DisposeAsync() =>
#if NET6_0_OR_GREATER
        ValueTask.CompletedTask
#else
        default
#endif    
    ;
#endif

    /// <summary>
    /// Gets the instance of <see cref="NullDisposable"/>.
    /// </summary>
    public static NullDisposable Instance { get; } = new();
}

public sealed class DisposableAction : IDisposable
#if ValueTaskSupport
    , IAsyncDisposable
#endif
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
    
#if ValueTaskSupport
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
#endif
}
