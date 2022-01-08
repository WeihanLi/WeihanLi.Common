namespace WeihanLi.Common.Logging;

/// <summary>
/// LogLevel
/// </summary>
public enum LogHelperLogLevel
{
    /// <summary>
    /// All logging levels
    /// </summary>
    All = 0,

    /// <summary>
    /// A trace logging level
    /// </summary>
    Trace = 1,

    /// <summary>
    /// A debug logging level
    /// </summary>
    Debug = 2,

    /// <summary>
    /// A info logging level
    /// </summary>
    Info = 4,

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
