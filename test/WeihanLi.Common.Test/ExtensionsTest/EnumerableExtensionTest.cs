using WeihanLi.Extensions;
using Xunit;

namespace WeihanLi.Common.Test.ExtensionsTest;

public class EnumerableExtensionTest
{
    [Fact]
    public void SplitTest()
    {
        var data = Enumerable.Range(1, 20);
        var array = data.Split(6).ToArray();
        Assert.Equal(4, array.Length);
        Assert.All(array, Assert.NotNull);
    }
}
