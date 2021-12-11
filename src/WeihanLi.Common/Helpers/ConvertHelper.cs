using System.Net;

namespace WeihanLi.Common.Helpers;

public static class ConvertHelper
{
    /// <summary>
    /// ip或域名转换为 EndPoint
    /// </summary>
    /// <param name="ipOrHost">ipOrHost</param>
    /// <param name="port">port</param>
    /// <returns>EndPoint</returns>
    public static EndPoint ToEndPoint(string ipOrHost, int port)
    {
        if (IPAddress.TryParse(ipOrHost, out var address))
        {
            return new IPEndPoint(address, port);
        }
        return new DnsEndPoint(ipOrHost, port);
    }
}
