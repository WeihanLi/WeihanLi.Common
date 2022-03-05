namespace WeihanLi.Common.Models;

public record Result
{
    /// <summary>
    /// ResultStatus
    /// </summary>
    public ResultStatus Status { get; set; }

    /// <summary>
    /// Message
    /// </summary>
    public string? Msg { get; set; }

    public static Result Success()
    {
        return new()
        {
            Status = ResultStatus.Success,
        };
    }

    public static Result<T> Success<T>(T result)
    {
        return new()
        {
            Status = ResultStatus.Success,
            Data = result
        };
    }

    public static Result Fail(string? errorMsg, ResultStatus status = ResultStatus.RequestError)
    {
        return new()
        {
            Msg = errorMsg,
            Status = status,
        };
    }

    public static Result<T> Fail<T>(string? errorMsg, ResultStatus status = ResultStatus.RequestError, T? result = default)
    {
        return new()
        {
            Msg = errorMsg,
            Status = status,
            Data = result
        };
    }

    public Result<T> ToResult<T>(T data)
    {
        return new Result<T>()
        {
            Data = data,
            Status = Status,
            Msg = Msg,
        };
    }
}

public record Result<T> : Result
{
    public T? Data { get; set; }

    public Result<T1> ToResult<T1>(Func<T?, T1> converter)
    {
        Guard.NotNull(converter);
        return new()
        {
            Data = converter(Data),
            Status = Status,
            Msg = Msg,
        };
    }
}

public static class ResultExtensions
{
    /// <summary>
    /// Whether the result status is Success
    /// </summary>
    /// <param name="result">result</param>
    /// <returns>true, status is Success, otherwise not</returns>
    public static bool IsSuccess(this Result result)
        => Guard.NotNull(result).Status == ResultStatus.Success;

    public static Result<T> WrapResult<T>(T t, ResultStatus status = ResultStatus.Success, string? msg = null)
        => new()
        {
            Data = t,
            Status = status,
            Msg = msg
        };
}
