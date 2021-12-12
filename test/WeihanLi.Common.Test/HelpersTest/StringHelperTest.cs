using WeihanLi.Common.Helpers;
using Xunit;

namespace WeihanLi.Common.Test.HelpersTest;

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

    [Theory]
    [InlineData(' ')]
    // the characters below are not separator
    //[InlineData(';')]
    //[InlineData('_')]
    //[InlineData('-')]
    public void IsSeparator(char c)
    {
        Assert.True(char.IsSeparator(c));
    }

    [Theory]
    [InlineData("test", "Test")]
    [InlineData("testProject", "TestProject")]
    [InlineData("userName", "UserName")]
    [InlineData("UserName", "UserName")]
    public void PascalCaseTest(string str1, string str2)
    {
        Assert.Equal(str2, StringHelper.ToPascalCase(str1));
    }

    [Theory]
    [InlineData("test", "Test")]
    [InlineData("testProject", "TestProject")]
    [InlineData("userName", "userName")]
    [InlineData("userName", "UserName")]
    //[InlineData("user name", "user name")]
    //[InlineData("user Name", "user Name")]
    public void ToCamelCaseTest(string str1, string str2)
    {
        Assert.Equal(str1, StringHelper.ToCamelCase(str2));
    }
}
