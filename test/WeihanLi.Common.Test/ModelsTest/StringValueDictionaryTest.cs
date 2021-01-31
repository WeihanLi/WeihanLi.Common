using Newtonsoft.Json;
using System.Collections.Generic;
using WeihanLi.Common.Models;
using Xunit;

namespace WeihanLi.Common.Test.ModelsTest
{
    public class StringValueDictionaryTest
    {
        [Fact]
        public void EqualsTest()
        {
            var abc = new { Id = 1, Name = "Tom" };
            var dic1 = StringValueDictionary.FromObject(abc);
            var dic2 = StringValueDictionary.FromObject(new Dictionary<string, object>()
            {
                {"Name", "Tom" },
                {"Id", 1},
            });

            Assert.True(dic1 == dic2);
            Assert.Equal(dic1, dic2);
        }

        [Fact]
        public void DistinctTest()
        {
            var abc = new { Id = 1, Name = "Tom" };
            var dic1 = StringValueDictionary.FromObject(abc);
            var dic2 = StringValueDictionary.FromObject(new Dictionary<string, object>()
            {
                { "Id", 1 },
                { "Name", "Tom" },
            });
            var set = new HashSet<StringValueDictionary>();
            set.Add(dic1);
            set.Add(dic2);

            Assert.Single(set);
        }

        [Fact]
        public void CloneTest()
        {
            var dic1 = StringValueDictionary.FromObject(new Dictionary<string, object>()
            {
                {"Id", 1},
                {"Name", "Tom" }
            });
            var dic2 = dic1.Clone();
            Assert.False(ReferenceEquals(dic1, dic2));
            Assert.True(dic1 == dic2);
        }

        record Person(int Id, string Name);

        [Fact]
        public void RecordTest()
        {
            var str1 = "{\"Id\":1, \"Name\":\"Tom\"}";
            var p1 = JsonConvert.DeserializeObject<Person>(str1);

            var str2 = "{\"Name\":\"Tom\",\"Id\":1}";
            var p2 = JsonConvert.DeserializeObject<Person>(str2);

            Assert.True(p1 == p2);
            Assert.Equal(p1, p2);
        }

        [Fact]
        public void ImplicitConvertTest()
        {
            var abc = new { Id = 1, Name = "Tom" };
            var stringValueDictionary = StringValueDictionary.FromObject(abc);
            Dictionary<string, string> dictionary = stringValueDictionary;
            Assert.Equal(stringValueDictionary.Count, dictionary.Count);

            var dic2 = StringValueDictionary.FromObject(dictionary);

            Assert.Equal(dic2, stringValueDictionary);
            Assert.True(dic2 == stringValueDictionary);
        }
    }
}
