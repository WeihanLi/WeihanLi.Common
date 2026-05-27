// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using System.ComponentModel;

namespace WeihanLi.Common.Models;

/// <summary>
/// Specifies standard result statuses.
/// </summary>
public enum ResultStatus
{
    /// <summary>
    /// Empty status.
    /// </summary>
    [Description("Empty Status")]
    None = 0,

    /// <summary>
    /// Continue status.
    /// </summary>
    [Description("Continue")]
    Continue = 100,

    /// <summary>
    /// Processing status.
    /// </summary>
    [Description("Processing")]
    Processing = 102,

    /// <summary>
    /// Success status.
    /// </summary>
    [Description("Success")]
    Success = 200,

    /// <summary>
    /// Created status.
    /// </summary>
    [Description("Created")]
    Created = 201,

    /// <summary>
    /// Accepted status.
    /// </summary>
    [Description("Accepted")]
    Accepted = 202,

    /// <summary>
    /// Bad request status.
    /// </summary>
    [Description("BadRequest, Request Parameter Error")]
    [Obsolete("Please use BadRequest instead", true)]
    RequestError = 400,

    /// <summary>
    /// Bad request status.
    /// </summary>
    [Description("BadRequest")]
    BadRequest = 400,

    /// <summary>
    /// Unauthorized status.
    /// </summary>
    [Description("Unauthorized")]
    Unauthorized = 401,

    /// <summary>
    /// Forbidden status.
    /// </summary>
    [Description("NoPermission")]
    [Obsolete("Please use Forbidden instead")]
    NoPermission = 403,

    /// <summary>
    /// Forbidden status.
    /// </summary>
    [Description("Forbidden")]
    Forbidden = 403,

    /// <summary>
    /// Not found status.
    /// </summary>
    [Description("ResourceNotFound")]
    [Obsolete("Please use NotFound instead", true)]
    ResourceNotFound = 404,

    /// <summary>
    /// Not found status.
    /// </summary>
    [Description("NotFound")]
    NotFound = 404,

    /// <summary>
    /// Method not allowed status.
    /// </summary>
    [Description("MethodNotAllowed")]
    MethodNotAllowed = 405,

    /// <summary>
    /// Request timeout status.
    /// </summary>
    [Description("RequestTimeout")]
    RequestTimeout = 408,

    /// <summary>
    /// Too many requests status.
    /// </summary>
    [Description("TooManyRequests")]
    TooManyRequests = 429,

    /// <summary>
    /// Internal error status.
    /// </summary>
    [Description("Process failed, Server Internal Error")]
    [Obsolete("Please use InternalError instead", true)]
    ProcessFail = 500,

    /// <summary>
    /// Internal error status.
    /// </summary>
    [Description("InternalError")]
    InternalError = 500,

    /// <summary>
    /// Not implemented status.
    /// </summary>
    [Description("Not Implemented")]
    NotImplemented = 501,

    /// <summary>
    /// Service unavailable status.
    /// </summary>
    [Description("ServiceUnavailable")]
    ServiceUnavailable = 503,

    /// <summary>
    /// Version not supported status.
    /// </summary>
    [Description("VersionNotSupported")]
    VersionNotSupported = 505
}
