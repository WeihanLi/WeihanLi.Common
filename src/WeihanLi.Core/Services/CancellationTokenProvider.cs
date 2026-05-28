namespace WeihanLi.Common.Services;

/// <summary>
/// Provides a cancellation token for the current operation.
/// </summary>
public interface ICancellationTokenProvider
{
    /// <summary>
    /// Gets the cancellation token.
    /// </summary>
    /// <returns>The cancellation token for the current operation.</returns>
    CancellationToken GetCancellationToken();
}

/// <summary>
/// Cancellation token provider that always returns <see cref="CancellationToken.None"/>.
/// </summary>
public sealed class NullCancellationTokenProvider : ICancellationTokenProvider
{
    /// <summary>
    /// Gets <see cref="CancellationToken.None"/>.
    /// </summary>
    /// <returns><see cref="CancellationToken.None"/>.</returns>
    public CancellationToken GetCancellationToken() => CancellationToken.None;
}
