// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

namespace WeihanLi.Common.Models;

public class BaseEntity<TKey>
{
    public TKey Id { get; set; } = default!;
}

public interface IEntityWithCreatedUpdatedAt
{
    DateTime CreatedAt { get; set; }
    DateTime UpdatedAt { get; set; }
}

public interface IEntityWithCreatedUpdatedBy
{
    string CreatedBy { get; set; }
    string UpdatedBy { get; set; }
}

public interface IEntityWithCreatedUpdatedAtAndBy : IEntityWithCreatedUpdatedAt, IEntityWithCreatedUpdatedBy
{
}

public interface IEntityWithReviewState
{
    ReviewState State { get; set; }
}

public class BaseEntityWithDeleted<TKey> : BaseEntity<TKey>, ISoftDeleteEntityWithDeleted
{
    public bool IsDeleted { get; set; }
}

public class BaseEntityWithCreatedUpdatedAt<TKey> : BaseEntity<TKey>, IEntityWithCreatedUpdatedAt
{
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class BaseEntityWithCreatedUpdatedAtAndDeleted<TKey> : BaseEntityWithCreatedUpdatedAt<TKey>,
    ISoftDeleteEntityWithDeleted
{
    public bool IsDeleted { get; set; }
}

public class BaseEntityWithCreatedUpdatedAtAndBy<TKey> : BaseEntityWithCreatedUpdatedAt<TKey>,
    IEntityWithCreatedUpdatedAtAndBy
{
    public string CreatedBy { get; set; } = default!;
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

public class BaseEntityWithReviewStateWithDeleted<TKey> : BaseEntityWithDeleted<TKey>, IEntityWithReviewState
{
    public ReviewState State { get; set; }
}

public class BaseEntityWithCreatedUpdatedAtAndDeletedAndReviewState<TKey> :
    BaseEntityWithCreatedUpdatedAtAndDeleted<TKey>, IEntityWithReviewState
{
    public ReviewState State { get; set; }
}

public class BaseEntityWithCreatedUpdatedAtAndByAndDeletedAndReviewState<TKey> :
    BaseEntityWithCreatedUpdatedAtAndByAndDeleted<TKey>, IEntityWithReviewState
{
    public ReviewState State { get; set; }
}

public class BaseEntity : BaseEntity<int>;

public class BaseEntityWithDeleted : BaseEntityWithDeleted<int>;

public class BaseEntityWithReviewState : BaseEntityWithReviewState<int>;

public class BaseEntityWithReviewStateWithDeleted : BaseEntityWithReviewStateWithDeleted<int>;

public class BaseEntityWithCreatedUpdatedAt : BaseEntityWithCreatedUpdatedAt<int>;

public class BaseEntityWithCreatedUpdatedAtAndDeleted : BaseEntityWithCreatedUpdatedAtAndDeleted<int>;

public class BaseEntityWithCreatedUpdatedAtAndDeletedAndReviewState : BaseEntityWithCreatedUpdatedAtAndDeletedAndReviewState<
    int>;

public class BaseEntityWithCreatedUpdatedAtAndBy : BaseEntityWithCreatedUpdatedAtAndBy<int>;

public class BaseEntityWithCreatedUpdatedAtAndByAndDeleted : BaseEntityWithCreatedUpdatedAtAndByAndDeleted<int>;

public class BaseEntityWithCreatedUpdatedAtAndByAndDeletedAndReviewState :
    BaseEntityWithCreatedUpdatedAtAndByAndDeletedAndReviewState<int>;
