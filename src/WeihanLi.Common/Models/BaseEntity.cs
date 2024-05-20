// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using System.ComponentModel.DataAnnotations;

namespace WeihanLi.Common.Models;

public interface IEntity<TKey>
{
    TKey Id { get; set; }
}

public class BaseEntity<TKey> : IEntity<TKey>
{
    public TKey Id { get; set; } = default!;
}

public interface IEntityWithCreatedUpdatedAt
{
    DateTimeOffset CreatedAt { get; set; }
    DateTimeOffset UpdatedAt { get; set; }
}

public interface IEntityWithCreatedUpdatedBy
{
    string CreatedBy { get; set; }
    string UpdatedBy { get; set; }
}

public interface IEntityWithCreatedUpdatedAtAndBy
    : IEntityWithCreatedUpdatedAt, IEntityWithCreatedUpdatedBy;

public interface IEntityWithReviewState
{
    ReviewState State { get; set; }
}

public interface IEntityWithRemark
{
    [StringLength(2048)]
    string? Remark { get; set; }
}

public interface IEntityWithReviewStateAndRemark : IEntityWithReviewState, IEntityWithRemark;

public class BaseEntityWithDeleted<TKey> : BaseEntity<TKey>, ISoftDeleteEntityWithDeleted
{
    public bool IsDeleted { get; set; }
}

public class BaseEntityWithCreatedUpdatedAt<TKey> : BaseEntity<TKey>, IEntityWithCreatedUpdatedAt
{
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class BaseEntityWithCreatedUpdatedAtAndDeleted<TKey> : BaseEntityWithCreatedUpdatedAt<TKey>,
    ISoftDeleteEntityWithDeleted
{
    public bool IsDeleted { get; set; }
}

public class BaseEntityWithCreatedUpdatedAtAndDeletedAndRemark<TKey> 
    : BaseEntityWithCreatedUpdatedAtAndDeleted<TKey>,
    IEntityWithRemark
{
    [StringLength(2048)]
    public string? Remark { get; set; }
}

public class BaseEntityWithCreatedUpdatedAtAndBy<TKey> 
    : BaseEntityWithCreatedUpdatedAt<TKey>, IEntityWithCreatedUpdatedAtAndBy
{
    [StringLength(256)]
    public string CreatedBy { get; set; } = default!;
    [StringLength(256)]
    public string UpdatedBy { get; set; } = default!;
}

public class BaseEntityWithCreatedUpdatedAtAndByAndDeleted<TKey> : BaseEntityWithCreatedUpdatedAtAndBy<TKey>,
    ISoftDeleteEntityWithDeleted
{
    public bool IsDeleted { get; set; }
}

public class BaseEntityWithReviewState<TKey> : BaseEntity<TKey>, IEntityWithReviewState
{
    public ReviewState State { get; set; }
}

public class BaseEntityWithReviewStateAndRemark<TKey> 
    : BaseEntityWithReviewState<TKey>, IEntityWithReviewStateAndRemark
{
    [StringLength(2048)]
    public string? Remark { get; set; }
}

public class BaseEntityWithReviewStateWithDeleted<TKey> 
    : BaseEntityWithDeleted<TKey>, IEntityWithReviewState
{
    public ReviewState State { get; set; }
}

public class BaseEntityWithCreatedUpdatedAtAndDeletedAndReviewState<TKey> :
    BaseEntityWithCreatedUpdatedAtAndDeleted<TKey>, IEntityWithReviewState
{
    public ReviewState State { get; set; }
}

public class BaseEntityWithCreatedUpdatedAtAndDeletedAndReviewStateAndRemark<TKey> :
    BaseEntityWithCreatedUpdatedAtAndDeletedAndReviewState<TKey>, IEntityWithReviewStateAndRemark
{
    [StringLength(2048)]
    public string? Remark { get; set; }
}

public class BaseEntityWithCreatedUpdatedAtAndByAndDeletedAndReviewState<TKey> :
    BaseEntityWithCreatedUpdatedAtAndByAndDeleted<TKey>, IEntityWithReviewState
{
    public ReviewState State { get; set; }
}

public class BaseEntityWithCreatedUpdatedAtAndByAndDeletedAndReviewStateAndRemark<TKey> :
    BaseEntityWithCreatedUpdatedAtAndByAndDeletedAndReviewState<TKey>, IEntityWithReviewStateAndRemark
{
    [StringLength(2048)]
    public string? Remark { get; set; }
}

public class BaseEntity : BaseEntity<int>;

public class BaseEntityWithDeleted : BaseEntityWithDeleted<int>;

public class BaseEntityWithReviewState : BaseEntityWithReviewState<int>;

public class BaseEntityWithReviewStateAndRemark : BaseEntityWithReviewStateAndRemark<int>;

public class BaseEntityWithReviewStateWithDeleted
    : BaseEntityWithReviewStateWithDeleted<int>;

public class BaseEntityWithCreatedUpdatedAt
    : BaseEntityWithCreatedUpdatedAt<int>;

public class BaseEntityWithCreatedUpdatedAtAndDeleted
    : BaseEntityWithCreatedUpdatedAtAndDeleted<int>;

public class BaseEntityWithCreatedUpdatedAtAndDeletedAndRemark
    : BaseEntityWithCreatedUpdatedAtAndDeletedAndRemark<int>;

public class BaseEntityWithCreatedUpdatedAtAndDeletedAndReviewState
    : BaseEntityWithCreatedUpdatedAtAndDeletedAndReviewState<int>;

public class BaseEntityWithCreatedUpdatedAtAndDeletedAndReviewStateAndRemark
    : BaseEntityWithCreatedUpdatedAtAndDeletedAndReviewStateAndRemark<int>;

public class BaseEntityWithCreatedUpdatedAtAndBy 
    : BaseEntityWithCreatedUpdatedAtAndBy<int>;

public class BaseEntityWithCreatedUpdatedAtAndByAndDeleted 
    : BaseEntityWithCreatedUpdatedAtAndByAndDeleted<int>;

public class BaseEntityWithCreatedUpdatedAtAndByAndDeletedAndReviewState
    : BaseEntityWithCreatedUpdatedAtAndByAndDeletedAndReviewState<int>;

public class BaseEntityWithCreatedUpdatedAtAndByAndDeletedAndReviewStateAndRemark
    : BaseEntityWithCreatedUpdatedAtAndByAndDeletedAndReviewStateAndRemark<int>;
