// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

namespace WeihanLi.Common.Models;

public class BaseEntity<TKey>
{
    public TKey Id { get; set; } = default!;
}

public class BaseEntityWithDeleted<TKey> : BaseEntity<TKey>
{
    public bool IsDeleted { get; set; }
}

public class BaseEntityWithReviewState<TKey> : BaseEntity<TKey>
{
    public ReviewState State { get; set; }
}

public class BaseEntityWithReviewStateWithDeleted<TKey> : BaseEntityWithDeleted<TKey>
{
    public ReviewState State { get; set; }
}

public class BaseEntity : BaseEntity<int>
{
}

public class BaseEntityWithDeleted : BaseEntityWithDeleted<int>
{
}

public class BaseEntityWithReviewState : BaseEntityWithReviewState<int>
{
}

public class BaseEntityWithReviewStateWithDeleted : BaseEntityWithReviewStateWithDeleted<int>
{
}
