using System.Threading;
using WeihanLi.Common.Helpers;
using Xunit;

namespace WeihanLi.Common.Test.HelpersTest
{
    public class TotpHelperTest
    {
        private readonly object _lock = new object();

        [Fact]
        public void Test()
        {
            lock (_lock)
            {
                TotpHelper.ConfigureTotpOptions(options =>
                {
                    options.Salt = null;
                    options.ExpiresIn = 300;
                });
                var bizToken = "test_xxx";
                var code = TotpHelper.GenerateCode(bizToken);
                Thread.Sleep(2000);
                Assert.NotEmpty(code);
                Assert.True(TotpHelper.VerifyCode(bizToken, code));
            }
        }

        [Fact]
        public void SaltTest()
        {
            lock (_lock)
            {
                var bizToken = "test_xxx";
                TotpHelper.ConfigureTotpOptions(options => options.Salt = null);
                var code = TotpHelper.GenerateCode(bizToken);

                TotpHelper.ConfigureTotpOptions(options =>
                {
                    options.Salt = "amazing-dotnet";
                    options.ExpiresIn = 300;
                });

                var code1 = TotpHelper.GenerateCode(bizToken);
                Thread.Sleep(2000);
                Assert.NotEmpty(code);
                Assert.False(TotpHelper.VerifyCode(bizToken, code));
                Assert.NotEmpty(code1);
                Assert.True(TotpHelper.VerifyCode(bizToken, code1));
            }
        }
    }
}
