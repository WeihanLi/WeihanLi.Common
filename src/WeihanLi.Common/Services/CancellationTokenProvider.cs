using System.Threading;

namespace WeihanLi.Common.Services
{
    public interface ICancellationTokenProvider
    {
        CancellationToken GetCancellationToken();
    }

    public sealed class NullCancellationTokenProvider : ICancellationTokenProvider
    {
        public CancellationToken GetCancellationToken() => default;
    }
}
