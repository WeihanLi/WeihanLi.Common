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
        public void ToGenericTest()
        {
            var num1 = 1.2;
            var toNum1 = num1.To<decimal>();
            Assert.Equal(typeof(decimal), toNum1.GetType());
            Assert.Equal(typeof(double), toNum1.To<double>().GetType());

            // nullable test
            var nullableNum = num1.To<decimal?>();
            Assert.NotNull(nullableNum);
            Assert.Equal(1.2m, nullableNum.Value);
            Assert.Equal(1.2m, nullableNum.To<decimal>());
            // nullable to nullable
            var nullableNum2 = nullableNum.To<double?>();
            Assert.NotNull(nullableNum2);
            Assert.Equal(1.2d, nullableNum2.Value);

            // int to bool test
            Assert.False(0.To<bool>());
            Assert.True(1.To<bool>());
        }

        [Fact]
        public void ToTest()
        {
            var num1 = 1.2;
            var toNum1 = num1.To(typeof(decimal));
            Assert.Equal(typeof(decimal), toNum1.GetType());
            Assert.Equal(typeof(double), toNum1.To(typeof(double)).GetType());

            // nullable test
            var nullableNum = num1.To(typeof(decimal?));
            Assert.NotNull(nullableNum);
            Assert.Equal(1.2m, ((decimal?)nullableNum).GetValueOrDefault());
            Assert.Equal(1.2m, nullableNum.To(typeof(decimal)));
            // nullable to nullable
            var nullableNum2 = nullableNum.To(typeof(double?));
            Assert.NotNull(nullableNum2);
            Assert.Equal(1.2d, (double)nullableNum2.To(typeof(double)));

            // int to bool test
            Assert.False((bool)0.To(typeof(bool)));
            Assert.True((bool)1.To(typeof(bool)));
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
