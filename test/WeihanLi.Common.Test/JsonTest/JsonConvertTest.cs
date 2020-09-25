using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
            var str = JsonConvert.SerializeObject(new
            {
                Ip = ip,
                Endpoint = new IPEndPoint(ip, 6379)
            }, new IPAddressConverter(), new IPEndPointConverter());
            JsonConvert.DeserializeObject<JObject>(str, new IPAddressConverter(), new IPEndPointConverter());
        }
    }
}
