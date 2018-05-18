using WeihanLi.Extensions;
using Xunit;

namespace WeihanLi.Common.Test.ExtensionsTest
{
    /// <summary>
    /// CoreExtensionTest
    /// </summary>
    public class CoreExtensionTest
    {
        #region ObjectExtension

        [Fact]
        public void ToTest()
        {
            var num1 = 1.2;
            var toNum1 = num1.To<decimal>();
            Assert.Equal(typeof(decimal), toNum1.GetType());
            Assert.Equal(typeof(double), toNum1.To<double>().GetType());
        }

        #endregion ObjectExtension

        #region StringExtensionTest

        [Fact]
        public void SafeSubstring()
        {
            var str = "abcdefg";
            Assert.Equal(str.Substring(1, 2), str.SafeSubstring(1, 2));
            Assert.Equal("bcdefg", str.SafeSubstring(1, 20));
            Assert.Equal("", str.SafeSubstring(10, 20));

            Assert.Equal(str.Substring(str.Length), str.SafeSubstring(str.Length));
            Assert.Equal("", str.SafeSubstring(10));
        }

        [Fact]
        public void Sub()
        {
            string str = "abcdef";
            Assert.Equal("ef", str.Sub(-2));
            Assert.Equal("def", str.Sub(3));
        }

        #endregion StringExtensionTest
    }
}
