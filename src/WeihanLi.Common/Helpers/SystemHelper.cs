using System;

namespace WeihanLi.Common.Helpers
{
    public static class SystemHelper
    {
        /// <summary>
        /// ProcessorCount
        /// </summary>
        public static readonly int ProcessorCount = Environment.ProcessorCount;

        /// <summary>
        /// MachineName
        /// </summary>
        public static readonly string MachineName = Environment.MachineName;
    }
}
