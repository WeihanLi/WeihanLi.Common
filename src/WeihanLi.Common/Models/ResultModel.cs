namespace WeihanLi.Common.Models;

/// <summary>
/// ResultModel
/// </summary>
[Obsolete("Please use ")]
public record ResultModel
{
    /// <summary>
    /// ResultStatus
    /// </summary>
    public ResultStatus Status { get; set; }

    /// <summary>
    /// Message
    /// </summary>
    public string? ErrorMsg { get; set; }

    public static ResultModel Success()
    {
        return new()
        {
            Status = ResultStatus.Success,
        };
    }

    public static ResultModel<T> Success<T>(T result)
    {
        return new()
        {
            Status = ResultStatus.Success,
            Result = result
        };
    }

    public static ResultModel Fail(string? errorMsg, ResultStatus status = ResultStatus.RequestError)
    {
        return new()
        {
            ErrorMsg = errorMsg,
            Status = status,
        };
    }

    public static ResultModel<T> Fail<T>(string? errorMsg, ResultStatus status = ResultStatus.RequestError, T? result = default)
    {
        return new()
        {
            ErrorMsg = errorMsg,
            Status = status,
            Result = result
        };
    }

    public ResultModel<T> ToResult<T>(T data)
    {
        return new ResultModel<T>()
        {
            Result = data,
            Status = Status,
            ErrorMsg = ErrorMsg,
        };
    }
}

/// <summary>
/// ResultModel
/// </summary>
/// <typeparam name="T">Type</typeparam>
[Obsolete]
public record ResultModel<T> : ResultModel
{
    /// <summary>
    /// ResponseData
    /// </summary>
    public T? Result { get; set; }

    public ResultModel<T1> ToResult<T1>(Func<T?, T1> converter)
    {
        return new ResultModel<T1>()
        {
            Result = converter(Result),
            Status = Status,
            ErrorMsg = ErrorMsg,
        };
    }
}
