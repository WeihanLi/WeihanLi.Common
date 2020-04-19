using System.Threading;
using WeihanLi.Common.Helpers;
using Xunit;

namespace WeihanLi.Common.Test.HelpersTest
{
    public class TotpHelperTest
    {
        private readonly object _lock = new object();
        private readonly string bizToken = "test_xxx";

        [Fact]
        public void Test()
        {
            lock (_lock)
            {
                TotpHelper.ConfigureTotpOptions(options =>
                {
                    options.Salt = null;
                    options.ExpiresIn = 30;
                });
                var code = TotpHelper.GenerateCode(bizToken);
                Assert.NotEmpty(code);
                Thread.Sleep(2000);
                Assert.True(TotpHelper.VerifyCode(bizToken, code));
                Thread.Sleep(60 * 1000);
                Assert.False(TotpHelper.VerifyCode(bizToken, code));
            }
        }

        [Fact]
        public void SaltTest()
        {
            lock (_lock)
            {
                TotpHelper.ConfigureTotpOptions(options => options.Salt = null);
                var code = TotpHelper.GenerateCode(bizToken);
                Assert.NotEmpty(code);

                TotpHelper.ConfigureTotpOptions(options =>
                {
                    options.Salt = "amazing-dotnet";
                    options.ExpiresIn = 300;
                });
                Assert.False(TotpHelper.VerifyCode(bizToken, code));

                var code1 = TotpHelper.GenerateCode(bizToken);
                Assert.NotEmpty(code1);
                Thread.Sleep(2000);
                Assert.True(TotpHelper.VerifyCode(bizToken, code1));
            }
        }
    }
}
