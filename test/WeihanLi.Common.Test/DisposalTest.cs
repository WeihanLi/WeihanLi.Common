using System.Threading.Tasks;
using Xunit;

namespace WeihanLi.Common.Test;

public class DisposalTest
{
    [Fact]
    public void DisposableActionTest()
    {
        var a = 0;
        var disposal = new DisposableAction(() => a++);
        disposal.Dispose();
        Assert.Equal(1, a);
    }

    [Fact]
    public void DisposableActionParallelTest()
    {
        var a = 0;
        var disposal = new DisposableAction(() => a++);
        Parallel.For(1, 10, _ =>
        {
            disposal.Dispose();
        });
        Assert.Equal(1, a);
    }
}
