using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
namespace WeihanLi.Common;

/// <summary>
/// A singleton disposable that does nothing when disposed.
/// </summary>
public sealed class NullDisposable : IDisposable
#if NET6_0_OR_GREATER
    , IAsyncDisposable
#endif
{
    private NullDisposable()
    {
    }

    public void Dispose()
    {
    }
#if NET6_0_OR_GREATER
    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
#endif
    /// <summary>
    /// Gets the instance of <see cref="NullDisposable"/>.
    /// </summary>
    public static NullDisposable Instance { get; } = new();
}

public sealed class DisposableAction : IDisposable
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
}
