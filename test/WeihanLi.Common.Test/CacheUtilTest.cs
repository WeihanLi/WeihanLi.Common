using System.Linq;
using WeihanLi.Common.Models;
using Xunit;

namespace WeihanLi.Common.Test
{
    public class CacheUtilTest
    {
        [Fact]
        public void GetTypeProperty()
        {
            var properties = CacheUtil.GetTypeProperties(typeof(Category));
            Assert.True(properties.SequenceEqual(typeof(Category).GetProperties()));

            var obj = new { Name = "Alice", Age = 10 };
            properties = CacheUtil.GetTypeProperties(obj.GetType());
            Assert.True(properties.SequenceEqual(obj.GetType().GetProperties()));
        }

        [Fact]
        public void GetTypeField()
        {
            var fields = CacheUtil.GetTypeFields(typeof(Category));
            Assert.True(fields.SequenceEqual(typeof(Category).GetFields()));
        }
    }
}
