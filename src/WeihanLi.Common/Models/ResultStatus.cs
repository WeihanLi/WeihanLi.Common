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
    RequestError = 400,
    BadRequest = 400,

    [Description("Unauthorized")]
    Unauthorized = 401,

    [Description("NoPermission")]
    NoPermission = 403,
    Forbidden = 403,

    [Description("ResourceNotFound")]
    ResourceNotFound = 404,
    NotFound = 404,

    [Description("MethodNotAllowed")]
    MethodNotAllowed = 405,

    [Description("RequestTimeout")]
    RequestTimeout = 408,

    [Description("TooManyRequests")]
    TooManyRequests = 429,

    [Description("Process failed, Server Internal Error")]
    ProcessFail = 500,
    InternalError = 500,

    [Description("Not Implemented")]
    NotImplemented = 501,

    [Description("ServiceUnavailable")]
    ServiceUnavailable = 503,

    [Description("VersionNotSupported")]
    VersionNotSupported = 505
}
