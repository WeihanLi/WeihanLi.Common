using System.Net;
using WeihanLi.Common.Helpers;
using Xunit;

namespace WeihanLi.Common.Test.HelpersTest
{
    public class CommandRunnerTest
    {
        [Fact]
        public void HostNameTest()
        {
            var result = CommandRunner.ExecuteAndCapture("hostname");

            var hostName = Dns.GetHostName();
            Assert.Equal(hostName, result.StandardOut.TrimEnd());
            Assert.Equal(0, result.ExitCode);
        }
    }
}
