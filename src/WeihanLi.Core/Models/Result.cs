// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

namespace WeihanLi.Common.Models;

/// <summary>
/// Represents a standard operation result.
/// </summary>
public record Result
{
    /// <summary>
    /// Gets or sets the result status.
    /// </summary>
    public ResultStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the result message.
    /// </summary>
    public string? Msg { get; set; }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    /// <param name="msg">The optional result message.</param>
    /// <returns>A successful result.</returns>
    public static Result Success(string? msg = null)
    {
        return new()
        {
            Status = ResultStatus.Success,
            Msg = msg
        };
    }

    /// <summary>
    /// Creates a successful result with data.
    /// </summary>
    /// <typeparam name="T">The data type.</typeparam>
    /// <param name="result">The result data.</param>
    /// <param name="msg">The optional result message.</param>
    /// <returns>A successful result containing <paramref name="result"/>.</returns>
    public static Result<T> Success<T>(T result, string? msg = null)
    {
        return new()
        {
            Status = ResultStatus.Success,
            Msg = msg,
            Data = result
        };
    }

    /// <summary>
    /// Creates a failed result.
    /// </summary>
    /// <param name="msg">The failure message.</param>
    /// <param name="status">The failure status.</param>
    /// <returns>A failed result.</returns>
    public static Result Fail(string? msg, ResultStatus status = ResultStatus.BadRequest)
    {
        return new()
        {
            Msg = msg,
            Status = status,
        };
    }

    /// <summary>
    /// Creates a failed result with optional data.
    /// </summary>
    /// <typeparam name="T">The data type.</typeparam>
    /// <param name="msg">The failure message.</param>
    /// <param name="status">The failure status.</param>
    /// <param name="result">The optional result data.</param>
    /// <returns>A failed result.</returns>
    public static Result<T> Fail<T>(string? msg, ResultStatus status = ResultStatus.BadRequest, T? result = default)
    {
        return new()
        {
            Msg = msg,
            Status = status,
            Data = result
        };
    }

    /// <summary>
    /// Converts this result to a typed result with the specified data.
    /// </summary>
    /// <typeparam name="T">The data type.</typeparam>
    /// <param name="data">The result data.</param>
    /// <returns>A typed result that preserves this result's status and message.</returns>
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

/// <summary>
/// Represents a standard operation result with data.
/// </summary>
/// <typeparam name="T">The data type.</typeparam>
public record Result<T> : Result
{
    /// <summary>
    /// Gets or sets the result data.
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// Converts this result to another typed result using the specified converter.
    /// </summary>
    /// <typeparam name="T1">The target data type.</typeparam>
    /// <param name="converter">The data converter.</param>
    /// <returns>A typed result with converted data that preserves this result's status and message.</returns>
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

/// <summary>
/// Extension methods for <see cref="Result"/>.
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Determines whether the result status is <see cref="ResultStatus.Success"/>.
    /// </summary>
    /// <param name="result">The result to check.</param>
    /// <returns><see langword="true"/> when the result status is <see cref="ResultStatus.Success"/>; otherwise, <see langword="false"/>.</returns>
    public static bool IsSuccess(this Result result)
        => Guard.NotNull(result).Status == ResultStatus.Success;

    /// <summary>
    /// Wraps a value in a typed result.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="t">The value to wrap.</param>
    /// <param name="status">The result status.</param>
    /// <param name="msg">The optional result message.</param>
    /// <returns>A typed result containing <paramref name="t"/>.</returns>
    public static Result<T> WrapResult<T>(this T t, ResultStatus status = ResultStatus.Success, string? msg = null)
        => new()
        {
            Data = t,
            Status = status,
            Msg = msg
        };
}
