using System.Collections.Generic;
using System.Threading.Tasks;
using WeihanLi.Common.Helpers;
using Xunit;

namespace WeihanLi.Common.Test.HelpersTest
{
    public class ProcessExecutorTest
    {
        [Fact]
        public void DotnetInfoTest()
        {
            using var executor = new ProcessExecutor("dotnet", "--info");
            var list = new List<string>();
            executor.OutputDataReceived += (sender, str) =>
            {
                list.Add(str);
            };
            var exitCode = -1;
            executor.OnExited += (sender, code) =>
            {
                exitCode = code;
            };
            executor.Execute();

            Assert.NotEmpty(list);
            Assert.Equal(0, exitCode);
        }

        [Fact]
        public async Task DotnetInfoAsyncTest()
        {
            using var executor = new ProcessExecutor("dotnet", "--info");
            var list = new List<string>();
            executor.OutputDataReceived += (sender, str) =>
            {
                list.Add(str);
            };
            var exitCode = -1;
            executor.OnExited += (sender, code) =>
            {
                exitCode = code;
            };
            await executor.ExecuteAsync();

            Assert.NotEmpty(list);
            Assert.Equal(0, exitCode);
        }
    }
}
