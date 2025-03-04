using System.Net;
using System.Runtime.InteropServices;
using WeihanLi.Common.Helpers;
using Xunit;

namespace WeihanLi.Common.Test.HelpersTest;

public class CommandExecutorTest
{
    [Fact]
    public void HostNameTest()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return;
        }

        var result = CommandExecutor.ExecuteAndCapture("hostname");

        var hostName = Dns.GetHostName();
        Assert.Equal(hostName, result.StandardOut.TrimEnd());
        Assert.Equal(0, result.ExitCode);
    }

    [Fact]
    public async Task HostNameTestAsync()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return;
        }

        var result = await CommandExecutor.ExecuteAndCaptureAsync("hostname", cancellationToken: TestContext.Current.CancellationToken);

        var hostName = Dns.GetHostName();
        Assert.Equal(hostName, result.StandardOut.TrimEnd());
        Assert.Equal(0, result.ExitCode);
    }
}
