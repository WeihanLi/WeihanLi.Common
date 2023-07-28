using WeihanLi.Common.Helpers;
using Xunit;

namespace WeihanLi.Common.Test.HelpersTest;

public class TotpHelperTest
{
    [Fact]
    public void SaltTest()
    {
        var bizToken = "test_xxx";
        TotpHelper.ConfigureTotpOptions(options => options.Salt = null);
        var code = TotpHelper.GetCode(bizToken);
        Assert.NotEmpty(code);

        TotpHelper.ConfigureTotpOptions(options =>
        {
            options.Salt = "amazing-dotnet";
            options.ExpiresIn = 300;
        });
        Assert.False(TotpHelper.VerifyCode(bizToken, code));

        var code1 = TotpHelper.GetCode(bizToken);
        Assert.NotEmpty(code1);
        Thread.Sleep(2000);
        Assert.True(TotpHelper.VerifyCode(bizToken, code1));
    }
}
