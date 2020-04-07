using System.Threading;
using WeihanLi.Common.Helpers;
using Xunit;

namespace WeihanLi.Common.Test.HelpersTest
{
    public class TotpHelperTest
    {
        [Fact]
        public void Test()
        {
            lock (TotpHelper.ConfigureTotpOptions(null))
            {
                var bizToken = "test_xxx";
                var code = TotpHelper.GenerateCode(bizToken);
                Thread.Sleep(2000);
                Assert.True(TotpHelper.VerifyCode(bizToken, code));
            }
        }

        [Fact]
        public void SaltTest()
        {
            var bizToken = "test_xxx";
            var code = TotpHelper.GenerateCode(bizToken);
            lock (TotpHelper.ConfigureTotpOptions(options =>
            {
                options.Salt = "amazing-dotnet";
            }))
            {
                var code1 = TotpHelper.GenerateCode(bizToken);
                Thread.Sleep(2000);
                Assert.False(TotpHelper.VerifyCode(bizToken, code));
                Assert.True(TotpHelper.VerifyCode(bizToken, code1));
            }
        }
    }
}
