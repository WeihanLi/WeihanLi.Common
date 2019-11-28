using WeihanLi.Common.Helpers;
using Xunit;

namespace WeihanLi.Common.Test.HelpersTest
{
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
            Assert.Equal(NetHelper.IsPrivateIP(ip), isPrivate);
        }
    }
}
