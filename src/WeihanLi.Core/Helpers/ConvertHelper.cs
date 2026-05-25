using System.Net;

namespace WeihanLi.Common.Helpers;

public static class ConvertHelper
{
    public static EndPoint ToEndPoint(string ipOrHost, int port)
    {
        if (IPAddress.TryParse(ipOrHost, out var address))
        {
            return new IPEndPoint(address, port);
        }
        return new DnsEndPoint(ipOrHost, port);
    }
}
