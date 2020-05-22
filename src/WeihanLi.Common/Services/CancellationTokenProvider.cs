using System.Threading;

namespace WeihanLi.Common.Services
{
    public interface ICancellationTokenProvider
    {
        CancellationToken GetCancellationToken();
    }
}
