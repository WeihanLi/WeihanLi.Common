using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net;

namespace WeihanLi.Common.Json
{
    // https://stackoverflow.com/questions/18668617/json-net-error-getting-value-from-scopeid-on-system-net-ipaddress
    /// <summary>
    /// IPAddress JsonConverter
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class IPAddressConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(IPAddress);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return IPAddress.Parse((string)reader.Value);
        }
    }

    /// <summary>
    /// IpEndPoint JsonConverter
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class IPEndPointConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(IPEndPoint));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var endPoint = (IPEndPoint)value;
            var obj = new JObject
            {
                { "Address", JToken.FromObject(endPoint.Address, serializer) },
                { "Port", endPoint.Port }
            };
            obj.WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jObject = JObject.Load(reader);
            var address = jObject["Address"].ToObject<IPAddress>(serializer);
            var port = jObject["Port"].Value<int>();
            return new IPEndPoint(address, port);
        }
    }
}
