using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml.Serialization;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Helpers
{
    public static class SerializeHelper
    {
        /// <summary>
        ///     An object extension method that serialize an object to binary.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="this">The @this to act on.</param>
        /// <returns>A string.</returns>
        public static string SerializeBinary<T>(T @this)
        {
            var binaryWrite = new BinaryFormatter();

            using (var memoryStream = new MemoryStream())
            {
                binaryWrite.Serialize(memoryStream, @this);
                return Encoding.UTF8.GetString(memoryStream.ToArray());
            }
        }

        /// <summary>
        ///     An object extension method that serialize an object to binary.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="this">The @this to act on.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns>A string.</returns>
        public static string SerializeBinary<T>(T @this, Encoding encoding)
        {
            var binaryWrite = new BinaryFormatter();

            using (var memoryStream = new MemoryStream())
            {
                binaryWrite.Serialize(memoryStream, @this);
                return encoding.GetString(memoryStream.ToArray());
            }
        }

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
        ///     A string extension method that deserialize a string binary as &lt;T&gt;.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="this">The @this to act on.</param>
        /// <returns>The deserialize binary as &lt;T&gt;</returns>
        public static T DeserializeBinary<T>(string @this)
        {
            using (var stream = new MemoryStream(Encoding.Default.GetBytes(@this)))
            {
                var binaryRead = new BinaryFormatter();
                return (T)binaryRead.Deserialize(stream);
            }
        }

        /// <summary>
        ///     A string extension method that deserialize a string binary as &lt;T&gt;.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="this">The @this to act on.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns>The deserialize binary as &lt;T&gt;</returns>
        public static T DeserializeBinary<T>(string @this, Encoding encoding)
        {
            using (var stream = new MemoryStream(encoding.GetBytes(@this)))
            {
                var binaryRead = new BinaryFormatter();
                return (T)binaryRead.Deserialize(stream);
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
