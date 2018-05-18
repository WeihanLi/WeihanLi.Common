using System;
using WeihanLi.Extensions;
using Xunit;

namespace WeihanLi.Common.Test.ExtensionsTest
{
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
    }
}
