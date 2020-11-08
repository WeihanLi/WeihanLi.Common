using System.IO;
using System.Xml.Serialization;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Helpers
{
    public static class SerializeHelper
    {
        /// <summary>
        ///     A T extension method that serialize an object to Json.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="this">The @this to act on.</param>
        /// <returns>The Json string.</returns>
        public static string SerializeJson<T>(T @this) => @this.ToJson();

        /// <summary>
        ///     An object extension method that serialize a string to XML.
        /// </summary>
        /// <param name="this">The @this to act on.</param>
        /// <returns>The string representation of the Xml Serialization.</returns>
        public static string SerializeXml(object @this)
        {
            var xmlSerializer = new XmlSerializer(@this.GetType());
            using (var stringWriter = new StringWriter())
            {
                xmlSerializer.Serialize(stringWriter, @this);
                using (var streamReader = new StringReader(stringWriter.GetStringBuilder().ToString()))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }

        /// <summary>
        ///     A string extension method that deserialize a Json string to object.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="this">The @this to act on.</param>
        /// <returns>The object deserialized.</returns>
        public static T DeserializeJson<T>(string @this)
        {
            return @this.JsonToObject<T>();
        }

        /// <summary>
        ///     A string extension method that deserialize an Xml as &lt;T&gt;.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="this">The @this to act on.</param>
        /// <returns>The deserialize Xml as &lt;T&gt;</returns>
        public static T DeserializeXml<T>(string @this)
        {
            var x = new XmlSerializer(typeof(T));
            using var r = new StringReader(@this);
            return (T)x.Deserialize(r);
        }
    }
}
