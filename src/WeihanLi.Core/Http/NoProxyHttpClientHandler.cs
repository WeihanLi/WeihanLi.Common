namespace WeihanLi.Common.Http;

/// <summary>
/// HTTP client handler configured without proxy, cookies, automatic redirects, or certificate revocation checks.
/// </summary>
public sealed class NoProxyHttpClientHandler : HttpClientHandler
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NoProxyHttpClientHandler"/> class.
    /// </summary>
    public NoProxyHttpClientHandler()
    {
        Proxy = null;
        UseProxy = false;
        UseCookies = false;
        AllowAutoRedirect = false;
        CheckCertificateRevocationList = false;
    }
}
