using System.Net;
using WeihanLi.Common.Helpers;
using Xunit;

namespace WeihanLi.Common.Test.HelpersTest;

public class NetHelperTest
{
    [Theory]
    [InlineData("10.0.0.135", true)]
    [InlineData("192.168.0.185", true)]
    [InlineData("172.16.0.125", true)]
    [InlineData("172.105.192.135", false)]
    [InlineData("23.100.91.85", false)]
    public void PrivateIPTest(string ip, bool isPrivate)
    {
        Assert.Equal(isPrivate, NetHelper.IsPrivateIP(ip));
    }

    [Theory]
    [InlineData("192.168.0.0/16", "192.168.0.185", true)]
    [InlineData("192.168.0.0/16", "172.16.0.125", false)]
    [InlineData("23.100.91.85", "23.100.91.85", true)]
    public void IPNetworkTest(string cidr, string ip, bool contains)
    {
        Assert.Equal(contains, new WeihanLi.Common.Helpers.IPNetwork(cidr).Contains(IPAddress.Parse(ip)));
    }
}
