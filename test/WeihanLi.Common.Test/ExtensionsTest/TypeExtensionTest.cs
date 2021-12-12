using WeihanLi.Common.Helpers;
using WeihanLi.Common.Test.EventsTest;
using WeihanLi.Extensions;
using Xunit;

namespace WeihanLi.Common.Test.ExtensionsTest;

public class TypeExtensionTest
{
    [Fact]
    public void IsBasicTypeTest()
    {
        var types = new[]
        {
                typeof(bool),

                typeof(sbyte),
                typeof(byte),
                typeof(int),
                typeof(uint),
                typeof(short),
                typeof(ushort),
                typeof(long),
                typeof(ulong),
                typeof(float),
                typeof(double),
                typeof(decimal),

                typeof(DateTime),// IsPrimitive:False
                typeof(TimeSpan),// IsPrimitive:False

                typeof(char),
                typeof(string),// IsPrimitive:False

                //typeof(object),// IsPrimitive:False
            };
        Assert.All(types, t => Assert.True(t.IsBasicType()));
    }

    [Fact]
    public void GetDefaultValueTest()
    {
        Assert.Equal(default(int), typeof(int).GetDefaultValue());
        Assert.Equal(default(bool), typeof(bool).GetDefaultValue());
        Assert.Equal(default(TestEvent), typeof(TestEvent).GetDefaultValue());
        Assert.Null(typeof(void).GetDefaultValue());
    }

    [Theory]
    [InlineData(false, typeof(void))]
    [InlineData(false, typeof(int))]
    [InlineData(true, typeof(Task<int>))]
    [InlineData(true, typeof(ValueTask<int>))]
    public void IsTypeAwaitableTest(bool result, Type type)
    {
        Assert.Equal(result, type.IsAwaitable());
    }
}
