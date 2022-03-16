using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using WeihanLi.Common.Helpers;
using Xunit;

namespace WeihanLi.Common.Test.HelpersTest;

public class ProcessExecutorTest
{
    [Fact]
    public async Task HostNameTest()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return;
        }
        using var executor = new ProcessExecutor("hostName");
        var list = new List<string>();
        executor.OnOutputDataReceived += (_, str) =>
        {
            list.Add(str);
        };
        var exitCode = -1;
        executor.OnExited += (_, code) =>
        {
            exitCode = code;
        };
        await executor.ExecuteAsync();
        Assert.NotEmpty(list);

        var hostName = Dns.GetHostName();
        Assert.Contains(list, x => hostName.Equals(x));
        Assert.Equal(0, exitCode);
    }

    // [Fact]
    // public async Task EnvironmentVariablesTest()
    // {
    //     if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
    //     {
    //         return;
    //     }
    //     using var executor = new ProcessExecutor(new ProcessStartInfo("powershell", "-Command \"Write-Host $env:TestUser\"")
    //     {
    //         Environment =
    //             {
    //                 { "TestUser", "Alice" }
    //             }
    //     });
    //     var list = new List<string>();
    //     executor.OnOutputDataReceived += (_, str) =>
    //     {
    //         list.Add(str);
    //     };
    //     var exitCode = -1;
    //     executor.OnExited += (_, code) =>
    //     {
    //         exitCode = code;
    //     };
    //     await executor.ExecuteAsync();
    //     Assert.NotEmpty(list);

    //     Assert.Contains(list, x => "Alice".Equals(x));
    //     Assert.Equal(0, exitCode);
    // }
}
