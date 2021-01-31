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
            var dic1 = StringValueDictionary.Create(abc);
            var dic2 = StringValueDictionary.Create(new Dictionary<string, object>()
            {
                {"Id", 1},
                {"Name", "Tom" }
            });

            Assert.True(dic1 == dic2);
            Assert.Equal(dic1, dic2);
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
            var stringValueDictionary = StringValueDictionary.Create(abc);
            Dictionary<string, string> dictionary = stringValueDictionary;
            Assert.Equal(stringValueDictionary.Count, dictionary.Count);

            var dic2 = StringValueDictionary.Create(dictionary);

            Assert.Equal(dic2, stringValueDictionary);
            Assert.True(dic2 == stringValueDictionary);
        }
    }
}
