// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

namespace WeihanLi.Common.Models;

/// <summary>
/// ReviewState
/// 审核状态
/// </summary>
public enum ReviewState
{
    /// <summary>
    /// UnReviewed
    /// 待审核
    /// </summary>
    UnReviewed = 0,

    /// <summary>
    /// Reviewed
    /// 审核通过
    /// </summary>
    Reviewed = 1,

    /// <summary>
    /// Rejected
    /// 审核被拒绝
    /// </summary>
    Rejected = 2,
}

/// <summary>
/// Represents a review request.
/// </summary>
public class ReviewRequest
{
    /// <summary>
    /// Gets or sets the requested review state.
    /// </summary>
    public ReviewState State { get; set; }

    /// <summary>
    /// Gets or sets the review remark.
    /// </summary>
    public string? Remark { get; set; }

    /// <summary>
    /// Determines whether the review request is valid.
    /// </summary>
    /// <returns><see langword="true"/> when the request is valid; otherwise, <see langword="false"/>.</returns>
    public virtual bool IsValid()
    {
        if (State == ReviewState.Rejected && string.IsNullOrWhiteSpace(Remark))
        {
            return false;
        }
        return true;
    }
}
