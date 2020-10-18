using System.Threading;
using Xunit;

namespace WeihanLi.Common.Test
{
    public class ProfileTest
    {
        [Theory]
        [InlineData(100)]
        [InlineData(500)]
        [InlineData(1000)]
        public void StopWatchProfileTest(int delay)
        {
            var profiler = new StopwatchProfiler();
            profiler.Start();
            Thread.Sleep(delay);
            profiler.Stop();
            Assert.True(profiler.ElapsedMilliseconds > delay);
            profiler.Restart();
            Thread.Sleep(delay / 2);
            profiler.Stop();
            Assert.True(profiler.ElapsedMilliseconds < delay);
            profiler.Reset();
            Assert.Equal(0, profiler.ElapsedMilliseconds);
        }
    }
}
