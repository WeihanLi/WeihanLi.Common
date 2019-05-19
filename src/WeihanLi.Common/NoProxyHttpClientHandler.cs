using System.Net.Http;

namespace WeihanLi.Common
{
    public class NoProxyHttpClientHandler : HttpClientHandler
    {
        public NoProxyHttpClientHandler()
        {
            Proxy = null;
            UseProxy = false;
        }
    }
}
