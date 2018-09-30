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
        /// <returns></returns>
        string NewId();
    }

    /// <summary>
    /// IdGenerator based on Guid
    /// </summary>
    public class GuidIdGenerator : IIdGenerator
    {
        public static readonly GuidIdGenerator Instance = new GuidIdGenerator();

        public string NewId() => Guid.NewGuid().ToString("N");
    }

    /// <summary>
    /// Snowflake IdGenerator
    /// </summary>
    public class SnowflakeIdGenerator : IIdGenerator
    {
        public string NewId()
        {
            throw new NotImplementedException();
        }
    }
}
