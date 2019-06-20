using WeihanLi.Common.Helpers;
using Xunit;

namespace WeihanLi.Common.Test.HelpersTest
{
    public class StringHelperTest
    {
        [Fact]
        public void HideSensitiveInfo()
        {
            var testString = "12345678901";

            Assert.Equal("132****5489", StringHelper.HideTelDetails("13212345489"));
            Assert.Equal("123***901", StringHelper.HideSensitiveInfo(testString, 3, 3, sensitiveCharCount: 3));
            Assert.Equal("1****", StringHelper.HideSensitiveInfo(testString, 11, 1));
            Assert.Equal("***1", StringHelper.HideSensitiveInfo(testString, 11, 1, 3, false));
        }
    }
}
