using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Xml.Serialization;
using JetBrains.Annotations;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Helpers
{
    public interface IDataSerializer
    {
        /// <summary>
        /// 序列化
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="obj">object</param>
        /// <returns>bytes</returns>
        byte[] Serialize<T>([NotNull]T obj);

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="bytes">bytes</param>
        /// <returns>obj</returns>
        T Deserializer<T>([NotNull]byte[] bytes);
    }

    public interface ICompressSerializer { }

    public class BinaryDataSerializer : IDataSerializer
    {
        private readonly BinaryFormatter _binaryFormatter = new BinaryFormatter();

        public T Deserializer<T>(byte[] bytes)
        {
            if (typeof(Task).IsAssignableFrom(typeof(T)))
            {
                throw new ArgumentException(Resource.TaskCanNotBeSerialized);
            }
            using (var memoryStream = new MemoryStream(bytes))
            {
                return (T)_binaryFormatter.Deserialize(memoryStream);
            }
        }

        public byte[] Serialize<T>(T obj)
        {
            if (typeof(Task).IsAssignableFrom(typeof(T)))
            {
                throw new ArgumentException(Resource.TaskCanNotBeSerialized);
            }
            using (var memoryStream = new MemoryStream())
            {
                _binaryFormatter.Serialize(memoryStream, obj);
                return memoryStream.ToArray();
            }
        }
    }

    public class XmlDataSerializer : IDataSerializer
    {
        public T Deserializer<T>(byte[] bytes)
        {
            if (typeof(Task).IsAssignableFrom(typeof(T)))
            {
                throw new ArgumentException(Resource.TaskCanNotBeSerialized);
            }
            using (var ms = new MemoryStream(bytes))
            {
                var serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(ms);
            }
        }

        public byte[] Serialize<T>(T obj)
        {
            if (typeof(Task).IsAssignableFrom(typeof(T)))
            {
                throw new ArgumentException(Resource.TaskCanNotBeSerialized);
            }
            using (var ms = new MemoryStream())
            {
                var serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(ms, obj);
                return ms.ToArray();
            }
        }
    }

    public class JsonDataSerializer : IDataSerializer
    {
        public T Deserializer<T>(byte[] bytes)
        {
            if (typeof(Task).IsAssignableFrom(typeof(T)))
            {
                throw new ArgumentException(Resource.TaskCanNotBeSerialized);
            }
            return bytes.GetString().JsonToType<T>();
        }

        public byte[] Serialize<T>(T obj)
        {
            if (typeof(Task).IsAssignableFrom(typeof(T)))
            {
                throw new ArgumentException(Resource.TaskCanNotBeSerialized);
            }
            return obj.ToJson().GetBytes();
        }
    }

    public class CompressGZipSerilizer : IDataSerializer, ICompressSerializer
    {
        private readonly IDataSerializer _serializer;

        public CompressGZipSerilizer(IDataSerializer serializer)
        {
            _serializer = serializer;
        }

        public byte[] Serialize<T>(T obj)
        {
            return _serializer.Serialize(obj).CompressGZip();
        }

        public T Deserializer<T>(byte[] bytes)
        {
            return _serializer.Deserializer<T>(bytes.DecompressGZip());
        }
    }
}
