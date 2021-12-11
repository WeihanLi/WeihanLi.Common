using System.Net.Http;

namespace WeihanLi.Common.Http;

public sealed class NoProxyHttpClientHandler : HttpClientHandler
{
    public NoProxyHttpClientHandler()
    {
        Proxy = null;
        UseProxy = false;
    }
}
