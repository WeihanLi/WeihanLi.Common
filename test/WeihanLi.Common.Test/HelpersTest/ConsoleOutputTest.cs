using System;
using System.Threading.Tasks;
using WeihanLi.Common.Helpers;
using Xunit;

namespace WeihanLi.Common.Test.HelpersTest
{
    public class ConsoleOutputTest
    {
        [Theory]
        [InlineData("Hello World")]
        public void CaptureTest(string str)
        {
            using var output = ConsoleOutput.Capture();
            // ReSharper disable once Xunit.XunitTestWithConsoleOutput
            Console.Write(str);
            Assert.Equal(str, output.StandardOutput);
        }

        [Theory]
        [InlineData("Hello World")]
        public async Task CaptureAsyncTest(string str)
        {
            using var output = await ConsoleOutput.CaptureAsync();
            // ReSharper disable once Xunit.XunitTestWithConsoleOutput
            Console.Write(str);
            Assert.Equal(str, output.StandardOutput);
        }
    }
}
