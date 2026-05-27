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
    /// <inheritdoc />
    public CancellationToken GetCancellationToken() => CancellationToken.None;
}
