using WeihanLi.Common.Services;
using Xunit;

namespace WeihanLi.Common.Test.ServicesTest
{
    public class CancellationTokenProviderTest
    {
        [Fact]
        public void NullCancellationTokenProviderTest()
        {
            var provider = new NullCancellationTokenProvider();
            var cancellationToken = provider.GetCancellationToken();
            Assert.Equal(default, cancellationToken);
        }
    }
}
