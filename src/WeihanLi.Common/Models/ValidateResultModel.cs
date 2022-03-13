// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

namespace WeihanLi.Common.Models;

public class ValidateResultModel
{
    /// <summary>
    /// Valid
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// ErrorMessages
    /// Key: memberName
    /// Value: errorMessages
    /// </summary>
    public Dictionary<string, List<string>>? Errors { get; set; }
}
