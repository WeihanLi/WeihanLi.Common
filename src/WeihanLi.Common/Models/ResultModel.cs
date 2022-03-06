// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

namespace WeihanLi.Common.Models;

[Obsolete("Please use Result")]
public record ResultModel
{
    public ResultStatus Status { get; set; }

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

[Obsolete("Please use Result<T>")]
public record ResultModel<T> : ResultModel
{
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
