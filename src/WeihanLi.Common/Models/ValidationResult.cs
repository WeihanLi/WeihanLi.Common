// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

namespace WeihanLi.Common.Models;

public class ValidationResult
{
    /// <summary>
    /// Valid
    /// </summary>
    public bool Valid { get; set; }

    /// <summary>
    /// ErrorMessages
    /// Key: memberName
    /// Value: errorMessages
    /// </summary>
    public Dictionary<string, string[]>? Errors { get; set; }
}
