using System;
using System.Linq;
using WeihanLi.Common.Models;
using WeihanLi.Extensions;
using Xunit;

namespace WeihanLi.Common.Test.ExtensionsTest
{
    public class StringExtensionTest
    {
        [Theory]
        [InlineData(typeof(uint), "uint")]
        [InlineData(typeof(int), "int")]
        [InlineData(typeof(bool), "bool")]
        [InlineData(typeof(byte), "byte")]
        [InlineData(typeof(short), "short")]
        [InlineData(typeof(float), "float")]
        [InlineData(typeof(double), "double")]
        [InlineData(typeof(long), "long")]
        [InlineData(typeof(decimal), "decimal")]
        [InlineData(typeof(Guid), "guid")]
        [InlineData(typeof(DateTime), "datetime")]
        [InlineData(typeof(string), "string")]
        [InlineData(typeof(string), "String")]
        [InlineData(typeof(string), "System.String")]
        [InlineData(typeof(Category), "WeihanLi.Common.Models.Category")]
        public void GetTypeByTypeName(Type type, string name)
        {
            Assert.Equal(type, name.GetTypeByTypeName());
        }

        [Theory]
        [InlineData("axx", "123")]
        [InlineData("", "123")]
        [InlineData(" ", "123")]
        [InlineData(null, "123")]
        public void GetNotEmptyValue(string value, string defaultValue)
        {
            var expected = string.IsNullOrEmpty(value) ? defaultValue : value;
            Assert.Equal(expected, value.GetNotEmptyValueOrDefault(defaultValue));
        }

        [Theory]
        [InlineData("axx", "123")]
        [InlineData("", "123")]
        [InlineData(" ", "123")]
        [InlineData(null, "123")]
        public void StringGetValue(string value, string defaultValue)
        {
            var expected = string.IsNullOrWhiteSpace(value) ? defaultValue : value;
            Assert.Equal(expected, value.GetValueOrDefault(defaultValue));
        }

        [Theory]
        [InlineData("axx", "123")]
        [InlineData("axx", "")]
        [InlineData("12345", "123")]
        [InlineData("", "123")]
        [InlineData(null, "123")]
        [InlineData(null, null)]
        public void TrimStart(string value, string start)
        {
            var expected = start.IsNotNullOrEmpty() && value?.StartsWith(start) == true ? value.Substring(start.Length) : value;
            Assert.Equal(expected, value.TrimStart(start));
        }

        [Theory]
        [InlineData("axx")]
        [InlineData("12345")]
        [InlineData(" ")]
        [InlineData("")]
        [InlineData(null)]
        public void IsNullOrEmpty(string value)
        {
            Assert.Equal(string.IsNullOrEmpty(value), value.IsNullOrEmpty());
        }

        [Theory]
        [InlineData("axx")]
        [InlineData("12345")]
        [InlineData(" ")]
        [InlineData("")]
        [InlineData(null)]
        public void IsNotNullOrEmpty(string value)
        {
            Assert.Equal(!string.IsNullOrEmpty(value), value.IsNotNullOrEmpty());
        }

        [Theory]
        [InlineData("axx")]
        [InlineData("12345")]
        [InlineData(" ")]
        [InlineData("")]
        [InlineData(null)]
        public void IsNullOrWhiteSpace(string value)
        {
            Assert.Equal(string.IsNullOrWhiteSpace(value), value.IsNullOrWhiteSpace());
        }

        [Theory]
        [InlineData("axx")]
        [InlineData("12345")]
        [InlineData(" ")]
        [InlineData("")]
        [InlineData(null)]
        public void IsNotNullOrWhiteSpace(string value)
        {
            Assert.Equal(!string.IsNullOrWhiteSpace(value), value.IsNotNullOrWhiteSpace());
        }

        [Fact]
        public void SplitArray()
        {
            var count = 5;

            var str = Enumerable.Range(1, count).StringJoin(",");
            var array = str.SplitArray<int>();
            Assert.Equal(count, array.Length);
            Assert.True(array.SequenceEqual(Enumerable.Range(1, count)));

            str = Enumerable.Range(1, count).StringJoin(";");
            array = str.SplitArray<int>(new[] { ';' });
            Assert.Equal(count, array.Length);
            Assert.True(array.SequenceEqual(Enumerable.Range(1, count)));

            var array1 = str.SplitArray<int?>(new[] { ';' });
            Assert.Equal(count, array1.Length);
            Assert.True(array1.Select(x => x.GetValueOrDefault()).SequenceEqual(Enumerable.Range(1, count)));
        }
    }
}
