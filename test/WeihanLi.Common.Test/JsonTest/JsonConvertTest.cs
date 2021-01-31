using Newtonsoft.Json;
using System.Net;
using WeihanLi.Common.Json;
using WeihanLi.Extensions;
using Xunit;

namespace WeihanLi.Common.Test.JsonTest
{
    public class JsonConvertTest
    {
        [Fact]
        public void JsonTest()
        {
            var obj = new
            {
                Name = "Mike",
                Age = 10,
                Class = new { Grade = 7, }
            };
            Assert.Equal(JsonConvert.SerializeObject(obj), obj.ToJson());
        }

        [Fact]
        public void IpAddressJsonSerializeTest()
        {
            var ip = IPAddress.Parse("192.168.0.102");
            var endPoint = new IPEndPoint(ip, 6379);
            var str = JsonConvert.SerializeObject(new TestModel
            {
                Ip = ip,
                EndPoint = endPoint
            }, new IPAddressConverter(), new IPEndPointConverter());
            var result = JsonConvert.DeserializeObject<TestModel>(str, new IPAddressConverter(), new IPEndPointConverter());
            Assert.Equal(ip, result.Ip);
            Assert.Equal(endPoint, result.EndPoint);
        }

        private class TestModel
        {
            public IPAddress? Ip { get; set; }

            public IPEndPoint? EndPoint { get; set; }
        }
    }
}
