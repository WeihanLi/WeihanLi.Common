using System.Collections.Specialized;
using System.Linq;
using WeihanLi.Extensions;
using Xunit;

namespace WeihanLi.Common.Test.ExtensionsTest
{
    public class CollectionExtensionTest
    {
        [Fact]
        public void NameValueCollectionToDictionaryTest()
        {
            var collection = new NameValueCollection()
            {
                {"Test1", "Test1"},
                {"Test2", "Test1"},
                {"Test3", "Test1"},
                {"Test4", "Test1"},
            };
            var dic = collection.ToDictionary();
            Assert.Equal(collection.Count, dic.Count);
            foreach (var key in collection.AllKeys)
            {
                Assert.True(dic.ContainsKey(key!));
                Assert.Equal(collection.Get(key), dic[key!]);
            }
        }

        [Fact]
        public void NameValueCollectionToQueryStringTest()
        {
            var collection = new NameValueCollection();
            Assert.Equal(string.Empty, collection.ToQueryString());
            Assert.Equal(string.Empty, ((NameValueCollection)null!).ToQueryString());

            collection = new NameValueCollection()
            {
                {"Test1", "Test1"},
                {"Test2", "Test1"},
                {"Test3", "Test1"}
            };
            Assert.Equal("Test1=Test1&Test2=Test1&Test3=Test1", collection.ToQueryString());
        }

        [Fact]
        public void ListRemoveWhereTest()
        {
            var list = Enumerable.Range(1, 10).ToList();
            list.RemoveWhere(x => x > 6);
            Assert.False(list.Exists(x => x > 6));
        }
    }
}
