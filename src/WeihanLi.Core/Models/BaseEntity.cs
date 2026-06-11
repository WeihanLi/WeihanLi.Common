// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using System.ComponentModel.DataAnnotations;

namespace WeihanLi.Common.Models;

/// <summary>
/// Represents an entity with an identifier.
/// </summary>
/// <typeparam name="TKey">The identifier type.</typeparam>
public interface IEntity<TKey>
{
    /// <summary>
    /// Gets or sets the entity identifier.
    /// </summary>
    TKey Id { get; set; }
}

/// <summary>
/// Base entity with an identifier.
/// </summary>
/// <typeparam name="TKey">The identifier type.</typeparam>
public class BaseEntity<TKey> : IEntity<TKey>
{
    /// <summary>
    /// Gets or sets the entity identifier.
    /// </summary>
    public TKey Id { get; set; } = default!;
}

/// <summary>
/// Represents an entity with an update timestamp.
/// </summary>
public interface IEntityWithUpdatedAt
{
    /// <summary>
    /// Gets or sets the update timestamp.
    /// </summary>
    DateTimeOffset UpdatedAt { get; set; }
}

/// <summary>
/// Represents an entity with creation and update timestamps.
/// </summary>
public interface IEntityWithCreatedUpdatedAt : IEntityWithUpdatedAt
{
    /// <summary>
    /// Gets or sets the creation timestamp.
    /// </summary>
    DateTimeOffset CreatedAt { get; set; }
}

/// <summary>
/// Represents an entity with updater information.
/// </summary>
public interface IEntityWithUpdatedBy
{
    /// <summary>
    /// Gets or sets the updater identifier.
    /// </summary>
    string UpdatedBy { get; set; }
}

/// <summary>
/// Represents an entity with creator and updater information.
/// </summary>
public interface IEntityWithCreatedUpdatedBy : IEntityWithUpdatedBy
{
    /// <summary>
    /// Gets or sets the creator identifier.
    /// </summary>
    string CreatedBy { get; set; }
}

/// <summary>
/// Represents an entity with update timestamp and updater information.
/// </summary>
public interface IEntityWithUpdatedAtAndBy
    : IEntityWithUpdatedAt, IEntityWithUpdatedBy;

/// <summary>
/// Represents an entity with creation and update timestamps plus creator and updater information.
/// </summary>
public interface IEntityWithCreatedUpdatedAtAndBy
    : IEntityWithCreatedUpdatedAt, IEntityWithCreatedUpdatedBy, IEntityWithUpdatedAtAndBy;

/// <summary>
/// Represents an entity with a review state.
/// </summary>
public interface IEntityWithReviewState
{
    /// <summary>
    /// Gets or sets the review state.
    /// </summary>
    ReviewState State { get; set; }
}

/// <summary>
/// Represents an entity with a remark.
/// </summary>
public interface IEntityWithRemark
{
    /// <summary>
    /// Gets or sets the remark.
    /// </summary>
    [StringLength(2048)]
    string? Remark { get; set; }
}

/// <summary>
/// Represents an entity with review state and remark.
/// </summary>
public interface IEntityWithReviewStateAndRemark : IEntityWithReviewState, IEntityWithRemark;

/// <summary>
/// Base entity with soft-delete state.
/// </summary>
/// <typeparam name="TKey">The identifier type.</typeparam>
public class BaseEntityWithDeleted<TKey> : BaseEntity<TKey>, ISoftDeleteEntityWithDeleted
{
    /// <summary>
    /// Gets or sets a value indicating whether the entity is soft deleted.
    /// </summary>
    public bool IsDeleted { get; set; }
}

/// <summary>
/// Base entity with an update timestamp.
/// </summary>
/// <typeparam name="TKey">The identifier type.</typeparam>
public class BaseEntityWithUpdatedAt<TKey> : BaseEntity<TKey>, IEntityWithUpdatedAt
{
    /// <summary>
    /// Gets or sets the update timestamp.
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }
}

/// <summary>
/// Base entity with creation and update timestamps.
/// </summary>
/// <typeparam name="TKey">The identifier type.</typeparam>
public class BaseEntityWithCreatedUpdatedAt<TKey> : BaseEntityWithUpdatedAt<TKey>, IEntityWithCreatedUpdatedAt
{
    /// <summary>
    /// Gets or sets the creation timestamp.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }
}

/// <summary>
/// Base entity with creation and update timestamps plus soft-delete state.
/// </summary>
/// <typeparam name="TKey">The identifier type.</typeparam>
public class BaseEntityWithCreatedUpdatedAtAndDeleted<TKey> : BaseEntityWithCreatedUpdatedAt<TKey>,
    ISoftDeleteEntityWithDeleted
{
    /// <summary>
    /// Gets or sets a value indicating whether the entity is soft deleted.
    /// </summary>
    public bool IsDeleted { get; set; }
}

/// <summary>
/// Base entity with creation and update timestamps, soft-delete state, and remark.
/// </summary>
/// <typeparam name="TKey">The identifier type.</typeparam>
public class BaseEntityWithCreatedUpdatedAtAndDeletedAndRemark<TKey>
    : BaseEntityWithCreatedUpdatedAtAndDeleted<TKey>,
    IEntityWithRemark
{
    /// <summary>
    /// Gets or sets the remark.
    /// </summary>
    [StringLength(2048)]
    public string? Remark { get; set; }
}

/// <summary>
/// Base entity with update timestamp and updater information.
/// </summary>
/// <typeparam name="TKey">The identifier type.</typeparam>
public class BaseEntityWithUpdatedAtAndBy<TKey>
    : BaseEntityWithUpdatedAt<TKey>, IEntityWithUpdatedAtAndBy
{
    /// <summary>
    /// Gets or sets the updater identifier.
    /// </summary>
    [StringLength(256)]
    public string UpdatedBy { get; set; } = default!;
}

/// <summary>
/// Base entity with creation and update timestamps plus creator and updater information.
/// </summary>
/// <typeparam name="TKey">The identifier type.</typeparam>
public class BaseEntityWithCreatedUpdatedAtAndBy<TKey>
    : BaseEntityWithCreatedUpdatedAt<TKey>, IEntityWithCreatedUpdatedAtAndBy
{
    /// <summary>
    /// Gets or sets the creator identifier.
    /// </summary>
    [StringLength(256)]
    public string CreatedBy { get; set; } = default!;

    /// <summary>
    /// Gets or sets the updater identifier.
    /// </summary>
    [StringLength(256)]
    public string UpdatedBy { get; set; } = default!;
}

/// <summary>
/// Base entity with update timestamp, updater information, and soft-delete state.
/// </summary>
/// <typeparam name="TKey">The identifier type.</typeparam>
public class BaseEntityWithUpdatedAtAndByAndDeleted<TKey> : BaseEntityWithUpdatedAtAndBy<TKey>,
    ISoftDeleteEntityWithDeleted
{
    /// <summary>
    /// Gets or sets a value indicating whether the entity is soft deleted.
    /// </summary>
    public bool IsDeleted { get; set; }
}

/// <summary>
/// Base entity with creation and update timestamps, creator and updater information, and soft-delete state.
/// </summary>
/// <typeparam name="TKey">The identifier type.</typeparam>
public class BaseEntityWithCreatedUpdatedAtAndByAndDeleted<TKey> : BaseEntityWithCreatedUpdatedAtAndBy<TKey>,
    ISoftDeleteEntityWithDeleted
{
    /// <summary>
    /// Gets or sets a value indicating whether the entity is soft deleted.
    /// </summary>
    public bool IsDeleted { get; set; }
}

/// <summary>
/// Base entity with a review state.
/// </summary>
/// <typeparam name="TKey">The identifier type.</typeparam>
public class BaseEntityWithReviewState<TKey> : BaseEntity<TKey>, IEntityWithReviewState
{
    /// <summary>
    /// Gets or sets the review state.
    /// </summary>
    public ReviewState State { get; set; }
}

/// <summary>
/// Base entity with review state and remark.
/// </summary>
/// <typeparam name="TKey">The identifier type.</typeparam>
public class BaseEntityWithReviewStateAndRemark<TKey>
    : BaseEntityWithReviewState<TKey>, IEntityWithReviewStateAndRemark
{
    /// <summary>
    /// Gets or sets the remark.
    /// </summary>
    [StringLength(2048)]
    public string? Remark { get; set; }
}

/// <summary>
/// Base entity with review state and soft-delete state.
/// </summary>
/// <typeparam name="TKey">The identifier type.</typeparam>
public class BaseEntityWithReviewStateWithDeleted<TKey>
    : BaseEntityWithDeleted<TKey>, IEntityWithReviewState
{
    /// <summary>
    /// Gets or sets the review state.
    /// </summary>
    public ReviewState State { get; set; }
}

/// <summary>
/// Base entity with creation and update timestamps, soft-delete state, and review state.
/// </summary>
/// <typeparam name="TKey">The identifier type.</typeparam>
public class BaseEntityWithCreatedUpdatedAtAndDeletedAndReviewState<TKey> :
    BaseEntityWithCreatedUpdatedAtAndDeleted<TKey>, IEntityWithReviewState
{
    /// <summary>
    /// Gets or sets the review state.
    /// </summary>
    public ReviewState State { get; set; }
}

/// <summary>
/// Base entity with creation and update timestamps, soft-delete state, review state, and remark.
/// </summary>
/// <typeparam name="TKey">The identifier type.</typeparam>
public class BaseEntityWithCreatedUpdatedAtAndDeletedAndReviewStateAndRemark<TKey> :
    BaseEntityWithCreatedUpdatedAtAndDeletedAndReviewState<TKey>, IEntityWithReviewStateAndRemark
{
    /// <summary>
    /// Gets or sets the remark.
    /// </summary>
    [StringLength(2048)]
    public string? Remark { get; set; }
}

/// <summary>
/// Base entity with creation and update timestamps, creator and updater information, soft-delete state, and review state.
/// </summary>
/// <typeparam name="TKey">The identifier type.</typeparam>
public class BaseEntityWithCreatedUpdatedAtAndByAndDeletedAndReviewState<TKey> :
    BaseEntityWithCreatedUpdatedAtAndByAndDeleted<TKey>, IEntityWithReviewState
{
    /// <summary>
    /// Gets or sets the review state.
    /// </summary>
    public ReviewState State { get; set; }
}

/// <summary>
/// Base entity with creation and update timestamps, creator and updater information, soft-delete state, review state, and remark.
/// </summary>
/// <typeparam name="TKey">The identifier type.</typeparam>
public class BaseEntityWithCreatedUpdatedAtAndByAndDeletedAndReviewStateAndRemark<TKey> :
    BaseEntityWithCreatedUpdatedAtAndByAndDeletedAndReviewState<TKey>, IEntityWithReviewStateAndRemark
{
    /// <summary>
    /// Gets or sets the remark.
    /// </summary>
    [StringLength(2048)]
    public string? Remark { get; set; }
}

/// <summary>
/// Base entity with an integer identifier.
/// </summary>
public class BaseEntity : BaseEntity<int>;

/// <summary>
/// Base entity with an integer identifier and soft-delete state.
/// </summary>
public class BaseEntityWithDeleted : BaseEntityWithDeleted<int>;

/// <summary>
/// Base entity with an integer identifier and review state.
/// </summary>
public class BaseEntityWithReviewState : BaseEntityWithReviewState<int>;

/// <summary>
/// Base entity with an integer identifier, review state, and remark.
/// </summary>
public class BaseEntityWithReviewStateAndRemark : BaseEntityWithReviewStateAndRemark<int>;

/// <summary>
/// Base entity with an integer identifier, review state, and soft-delete state.
/// </summary>
public class BaseEntityWithReviewStateWithDeleted
    : BaseEntityWithReviewStateWithDeleted<int>;

/// <summary>
/// Base entity with an integer identifier plus creation and update timestamps.
/// </summary>
public class BaseEntityWithCreatedUpdatedAt
    : BaseEntityWithCreatedUpdatedAt<int>;

/// <summary>
/// Base entity with an integer identifier, creation and update timestamps, and soft-delete state.
/// </summary>
public class BaseEntityWithCreatedUpdatedAtAndDeleted
    : BaseEntityWithCreatedUpdatedAtAndDeleted<int>;

/// <summary>
/// Base entity with an integer identifier, creation and update timestamps, soft-delete state, and remark.
/// </summary>
public class BaseEntityWithCreatedUpdatedAtAndDeletedAndRemark
    : BaseEntityWithCreatedUpdatedAtAndDeletedAndRemark<int>;

/// <summary>
/// Base entity with an integer identifier, creation and update timestamps, soft-delete state, and review state.
/// </summary>
public class BaseEntityWithCreatedUpdatedAtAndDeletedAndReviewState
    : BaseEntityWithCreatedUpdatedAtAndDeletedAndReviewState<int>;

/// <summary>
/// Base entity with an integer identifier, creation and update timestamps, soft-delete state, review state, and remark.
/// </summary>
public class BaseEntityWithCreatedUpdatedAtAndDeletedAndReviewStateAndRemark
    : BaseEntityWithCreatedUpdatedAtAndDeletedAndReviewStateAndRemark<int>;

/// <summary>
/// Base entity with an integer identifier, creation and update timestamps, and creator and updater information.
/// </summary>
public class BaseEntityWithCreatedUpdatedAtAndBy
    : BaseEntityWithCreatedUpdatedAtAndBy<int>;

/// <summary>
/// Base entity with an integer identifier, creation and update timestamps, creator and updater information, and soft-delete state.
/// </summary>
public class BaseEntityWithCreatedUpdatedAtAndByAndDeleted
    : BaseEntityWithCreatedUpdatedAtAndByAndDeleted<int>;

/// <summary>
/// Base entity with an integer identifier, creation and update timestamps, creator and updater information, soft-delete state, and review state.
/// </summary>
public class BaseEntityWithCreatedUpdatedAtAndByAndDeletedAndReviewState
    : BaseEntityWithCreatedUpdatedAtAndByAndDeletedAndReviewState<int>;

/// <summary>
/// Base entity with an integer identifier, creation and update timestamps, creator and updater information, soft-delete state, review state, and remark.
/// </summary>
public class BaseEntityWithCreatedUpdatedAtAndByAndDeletedAndReviewStateAndRemark
    : BaseEntityWithCreatedUpdatedAtAndByAndDeletedAndReviewStateAndRemark<int>;
