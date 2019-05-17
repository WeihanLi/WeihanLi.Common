using System;

namespace WeihanLi.Common
{
    /// <summary>
    /// IdGenerator
    /// </summary>
    public interface IIdGenerator
    {
        /// <summary>
        /// 生成新的id
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
        private GuidIdGenerator()
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
        private ObjectIdGenerator()
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
