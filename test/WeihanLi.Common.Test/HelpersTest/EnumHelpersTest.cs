using WeihanLi.Common.Helpers;
using WeihanLi.Common.Models;
using Xunit;

namespace WeihanLi.Common.Test.HelpersTest;

public class EnumHelpersTest
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
        var list = EnumHelpers.ToIdNameDescList<ReviewState, sbyte>();
        Assert.Equal(list.Count, Enum.GetNames<ReviewState>().Length);
        foreach (var (id, name, description) in list)
        {
            Assert.True(Enum.TryParse(name, out ReviewState state));
            Assert.Equal((int)state, id);
            Assert.NotNull(description);
        }
    }
}
