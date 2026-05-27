namespace WeihanLi.Common.Services;

/// <summary>
/// Represents a disposable scope.
/// </summary>
public interface IScope : IDisposable;

/// <summary>
/// Empty scope implementation.
/// </summary>
public sealed class NullScope : IScope
{
    /// <inheritdoc />
    public void Dispose()
    {
    }

    /// <summary>
    /// Gets the singleton instance.
    /// </summary>
    public static NullScope Instance { get; } = new();
}
