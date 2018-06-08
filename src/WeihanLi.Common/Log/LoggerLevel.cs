namespace WeihanLi.Common.Log
{
    /// <summary>
    /// LoggerLevel
    /// </summary>
    public enum LogHelperLevel
    {
        /// <summary>
        /// All logging levels
        /// </summary>
        All = 0,

        /// <summary>
        /// A info logging level
        /// </summary>
        Info = 1,

        /// <summary>
        /// A debug logging level
        /// </summary>
        Debug = 2,

        /// <summary>
        /// A trace logging level
        /// </summary>
        Trace = 4,

        /// <summary>
        /// A warn logging level
        /// </summary>
        Warn = 8,

        /// <summary>
        /// An error logging level
        /// </summary>
        Error = 16,

        /// <summary>
        /// A fatal logging level
        /// </summary>
        Fatal = 32,

        /// <summary>
        /// None
        /// </summary>
        None = 64
    }
}
