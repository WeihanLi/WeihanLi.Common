// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using System.ComponentModel;

namespace WeihanLi.Common.Models;

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
    [Obsolete("Please use BadRequest instead", true)]
    RequestError = 400,

    [Description("BadRequest")]
    BadRequest = 400,

    [Description("Unauthorized")]
    Unauthorized = 401,

    [Description("NoPermission")]
    [Obsolete("Please use Forbidden instead")]
    NoPermission = 403,

    [Description("Forbidden")]
    Forbidden = 403,

    [Description("ResourceNotFound")]
    [Obsolete("Please use NotFound instead", true)]
    ResourceNotFound = 404,

    [Description("NotFound")]
    NotFound = 404,

    [Description("MethodNotAllowed")]
    MethodNotAllowed = 405,

    [Description("RequestTimeout")]
    RequestTimeout = 408,

    [Description("TooManyRequests")]
    TooManyRequests = 429,

    [Description("Process failed, Server Internal Error")]
    [Obsolete("Please use InternalError instead", true)]
    ProcessFail = 500,

    [Description("InternalError")]
    InternalError = 500,

    [Description("Not Implemented")]
    NotImplemented = 501,

    [Description("ServiceUnavailable")]
    ServiceUnavailable = 503,

    [Description("VersionNotSupported")]
    VersionNotSupported = 505
}
