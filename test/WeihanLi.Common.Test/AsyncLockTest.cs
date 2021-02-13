using System.Linq;
using System.Threading.Tasks;
using WeihanLi.Extensions;
using Xunit;

namespace WeihanLi.Common.Test
{
    public class AsyncLockTest
    {
        [Fact]
        public void LockTest()
        {
            using var locker = new AsyncLock();
            int num = 0, count = 100;
            //
            Parallel.For(0, count, _ =>
            {
                // ReSharper disable once AccessToDisposedClosure
                using (locker.Lock())
                {
                    num++;
                }
            });
            Assert.Equal(count, num);
        }

        [Fact]
        public async Task LockAsyncTest()
        {
            using var locker = new AsyncLock();
            int num = 0, count = 100;
            //
            await Enumerable.Range(1, count)
                .Select(async _ =>
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
