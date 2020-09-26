using System.Linq;
using System.Threading.Tasks;
using WeihanLi.Extensions;
using Xunit;

namespace WeihanLi.Common.Test
{
    public class AsyncLockTest
    {
        [Fact]
        public async Task MainTest()
        {
            using var locker = new AsyncLock();
            int num = 0, count = 100;
            //
            await Enumerable.Range(1, count)
                .Select(async x =>
                {
                    // ReSharper disable once AccessToDisposedClosure
                    using (await locker.LockAsync())
                    {
                        num++;
                    }
                })
                .WhenAll();
            Assert.Equal(count, num);
        }
    }
}
