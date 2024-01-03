using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;
using WeihanLi.Common.Compressor;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Helpers;

public interface IDataSerializer
{
    /// <summary>
    /// 序列化
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    /// <param name="obj">object</param>
    /// <returns>bytes</returns>
    [RequiresUnreferencedCode("Members from serialized types may be trimmed if not referenced directly")]
    byte[] Serialize<T>(T obj);

    /// <summary>
    /// 反序列化
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    /// <param name="bytes">bytes</param>
    /// <returns>obj</returns>
    [RequiresUnreferencedCode("Members from serialized types may be trimmed if not referenced directly")]
    T Deserialize<T>(byte[] bytes);
}

public class XmlDataSerializer : IDataSerializer
{
    internal static readonly Lazy<XmlDataSerializer> Instance = new();

    [RequiresUnreferencedCode("Members from serialized types may be trimmed if not referenced directly")]
    public virtual byte[] Serialize<T>(T obj)
    {
        if (typeof(Task).IsAssignableFrom(typeof(T)))
        {
            throw new ArgumentException(Resource.TaskCanNotBeSerialized);
        }
        if (obj is null)
        {
            throw new ArgumentNullException(nameof(obj));
        }
        using var ms = new MemoryStream();
        var serializer = new XmlSerializer(typeof(T));
        serializer.Serialize(ms, obj);
        return ms.ToArray();
    }

    [RequiresUnreferencedCode("If some of the generic arguments are annotated (either with DynamicallyAccessedMembersAttribute, or generic constraints), trimming can't validate that the requirements of those annotations are met.")]
    public virtual T Deserialize<T>(byte[] bytes)
    {
        if (typeof(Task).IsAssignableFrom(typeof(T)))
        {
            throw new ArgumentException(Resource.TaskCanNotBeSerialized);
        }

        using var ms = new MemoryStream(bytes);
        var serializer = new XmlSerializer(typeof(T));
        return (T)serializer.Deserialize(ms)!;
    }

}

public class JsonDataSerializer : IDataSerializer
{
    [RequiresUnreferencedCode("Members from serialized types may be trimmed if not referenced directly")]
    public virtual byte[] Serialize<T>(T obj)
    {
        if (typeof(Task).IsAssignableFrom(typeof(T)))
        {
            throw new ArgumentException(Resource.TaskCanNotBeSerialized);
        }
        return obj.ToJson().GetBytes();
    }

    [RequiresUnreferencedCode("Members from serialized types may be trimmed if not referenced directly")]
    public virtual T Deserialize<T>(byte[] bytes)
    {
        if (typeof(Task).IsAssignableFrom(typeof(T)))
        {
            throw new ArgumentException(Resource.TaskCanNotBeSerialized);
        }

        return bytes.GetString().JsonToObject<T>() ?? throw new ArgumentNullException(nameof(bytes));
    }
}

public sealed class CompressDataSerializer : IDataSerializer
{
    private readonly IDataSerializer _serializer;
    private readonly IDataCompressor _compressor;

    public CompressDataSerializer(IDataSerializer serializer, IDataCompressor compressor)
    {
        _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        _compressor = compressor ?? throw new ArgumentNullException(nameof(compressor));
    }

    [RequiresUnreferencedCode("Members from serialized types may be trimmed if not referenced directly")]
    public byte[] Serialize<T>(T obj)
    {
        return _compressor.Compress(_serializer.Serialize(obj));
    }

    [RequiresUnreferencedCode("Members from serialized types may be trimmed if not referenced directly")]
    public T Deserialize<T>(byte[] bytes)
    {
        return _serializer.Deserialize<T>(_compressor.Decompress(bytes));
    }
}
