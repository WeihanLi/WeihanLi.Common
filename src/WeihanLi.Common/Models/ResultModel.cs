using System;
using System.ComponentModel;

namespace WeihanLi.Common.Models
{
    /// <summary>
    /// ResultModel
    /// </summary>
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

    /// <summary>
    /// ResultStatus
    /// 返回的结果状态
    /// </summary>
    public enum ResultStatus
    {
        [Description("Empty Status")]
        None = 0,

        [Description("Continue")]
        Continue = 100,

        [Description("Processing")]
        Processing = 102,

        [Description("Success")]
        Success = 200,

        [Description("Created")]
        Created = 201,

        [Description("Accepted")]
        Accepted = 202,

        [Description("BadRequest, Request Parameter Error")]
        RequestError = 400,

        [Description("Unauthorized")]
        Unauthorized = 401,

        [Description("No permission")]
        NoPermission = 403,

        [Description("ResourceNotFound")]
        ResourceNotFound = 404,

        [Description("MethodNotAllowed")]
        MethodNotAllowed = 405,

        [Description("RequestTimeout")]
        RequestTimeout = 408,

        [Description("ProcessFail,Server Internal Error")]
        ProcessFail = 500,

        [Description("Not Implemented")]
        NotImplemented = 501,

        [Description("ServiceUnavailable")]
        ServiceUnavailable = 503,

        [Description("VersionNotSupported")]
        VersionNotSupported = 505
    }
}
