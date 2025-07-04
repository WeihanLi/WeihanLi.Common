namespace WeihanLi.Common.Http;

public sealed class NoProxyHttpClientHandler : HttpClientHandler
{
    public NoProxyHttpClientHandler()
    {
        Proxy = null;
        UseProxy = false;
        UseCookies = false;
        AllowAutoRedirect = false;
    }
}
