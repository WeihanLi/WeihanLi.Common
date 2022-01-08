using WeihanLi.Common.Helpers;
using WeihanLi.Common.Models;
using Xunit;

namespace WeihanLi.Common.Test.HelpersTest;

public class EnumHelperTest
{
    [Fact]
    public void IdNameListTest()
    {
        var list = EnumHelper.ToIdNameList<ReviewState>();
        Assert.Equal(list.Count, Enum.GetNames<ReviewState>().Length);
        foreach (var (id, name) in list)
        {
            Assert.True(Enum.TryParse(name, out ReviewState state));
            Assert.Equal((int)state, id);
        }
    }

    [Fact]
    public void IdNameDescListTest()
    {
        var list = EnumHelper.ToIdNameDescList<ReviewState, sbyte>();
        Assert.Equal(list.Count, Enum.GetNames<ReviewState>().Length);
        foreach (var (id, name, description) in list)
        {
            Assert.True(Enum.TryParse(name, out ReviewState state));
            Assert.Equal((int)state, id);
            Assert.NotNull(description);
        }
    }
}
