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
        public async Task Test(string str)
        {
            using var output = await ConsoleOutput.Capture();
            // ReSharper disable once Xunit.XunitTestWithConsoleOutput
            Console.Write(str);
            Assert.Equal(str, output.StandardOutput);
        }
    }
}
