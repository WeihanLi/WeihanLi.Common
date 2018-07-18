using System.ComponentModel;

namespace WeihanLi.Common.Models
{
    /// <summary>
    /// JsonResultModel
    /// </summary>
    public class JsonResultModel
    {
        /// <summary>
        /// JsonResultStatus
        /// </summary>
        public JsonResultStatus Status { get; set; }

        /// <summary>
        /// Message
        /// </summary>
        public string ErrorMsg { get; set; }

        /// <summary>
        /// ResponseData
        /// </summary>
        public object Result { get; set; }
    }

    /// <summary>
    /// JsonResultModel
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    public class JsonResultModel<T>
    {
        /// <summary>
        /// JsonResultStatus
        /// </summary>
        public JsonResultStatus Status { get; set; }

        /// <summary>
        /// Message
        /// </summary>
        public string ErrorMsg { get; set; }

        /// <summary>
        /// ResponseData
        /// </summary>
        public T Result { get; set; }

        public void SetSuccessResult(T result)
        {
            ErrorMsg = string.Empty;
            Status = JsonResultStatus.Success;
            Result = result;
        }

        public void SetProcessFailResult()
        {
            ErrorMsg = Resource.InvokeFail;
            Status = JsonResultStatus.ProcessFail;
        }
    }

    /// <summary>
    /// 返回的结果状态
    /// </summary>
    public enum JsonResultStatus
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

        [Description("ServiceUnavailable")]
        ServiceUnavailable = 503,

        [Description("VersionNotSupported")]
        VersionNotSupported = 505
    }
}
