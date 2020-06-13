using System;

namespace WeihanLi.Common
{
    /// <summary>
    /// IdGenerator
    /// </summary>
    public interface IIdGenerator
    {
        /// <summary>
        /// Generate a new id
        /// </summary>
        /// <returns>new id</returns>
        string NewId();
    }

    /// <summary>
    /// IdGenerator based on Guid
    /// </summary>
    public sealed class GuidIdGenerator : IIdGenerator
    {
        public GuidIdGenerator()
        {
        }

        public static readonly GuidIdGenerator Instance = new GuidIdGenerator();

        public string NewId() => Guid.NewGuid().ToString("N");
    }

    /// <summary>
    /// IdGenerator based on ObjectId
    /// </summary>
    public sealed class ObjectIdGenerator : IIdGenerator
    {
        public ObjectIdGenerator()
        {
        }

        public static readonly ObjectIdGenerator Instance = new ObjectIdGenerator();

        public string NewId() => ObjectId.GenerateNewStringId();
    }

    /// <summary>
    /// Snowflake IdGenerator
    /// WARNING: NotImplemented, do not use
    /// </summary>
    public sealed class SnowflakeIdGenerator : IIdGenerator
    {
        public string NewId()
        {
            throw new NotImplementedException();
        }
    }
}
