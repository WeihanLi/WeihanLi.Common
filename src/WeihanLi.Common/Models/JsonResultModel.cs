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
        None = 0,
        Continue = 100,
        Processing = 102,
        Success = 200,
        RequestError = 400,
        Unauthorized = 401,
        NoPermission = 403,
        ResourceNotFound = 404,
        MethodNotAllowed = 405,
        ProcessFail = 500,
    }
}
