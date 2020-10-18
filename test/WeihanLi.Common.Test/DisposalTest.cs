using System.Threading.Tasks;
using Xunit;

namespace WeihanLi.Common.Test
{
    public class DisposalTest
    {
        [Fact]
        public void DisposableActionTest()
        {
            int a = 0;
            var disposal = new DisposableAction(() => a++);
            disposal.Dispose();
            Assert.Equal(1, a);
        }

        [Fact]
        public void DisposableActionParallelTest()
        {
            int a = 0;
            var disposal = new DisposableAction(() => a++);
            Parallel.For(1, 10, i =>
            {
                disposal.Dispose();
            });
            Assert.Equal(1, a);
        }
    }
}
