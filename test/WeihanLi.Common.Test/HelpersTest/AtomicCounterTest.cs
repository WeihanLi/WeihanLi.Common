using System.Linq;
using System.Threading.Tasks;
using WeihanLi.Common.Helpers;
using WeihanLi.Extensions;
using Xunit;

namespace WeihanLi.Common.Test.HelpersTest
{
    public class AtomicCounterTest
    {
        [Fact]
        public void ParallelTest()
        {
            var counter = new AtomicCounter();
            Parallel.For(0, 50, i =>
            {
                counter.Increment();
            });
            Assert.Equal(50, counter.Value);
        }

        [Fact]
        public async Task ParallelAsyncTest()
        {
            var counter = new AtomicCounter();
            await Enumerable.Range(1, 50).Select(i => Task.Run(() => counter.Increment())).WhenAll();
            Assert.Equal(50, counter.Value);
        }
    }
}
