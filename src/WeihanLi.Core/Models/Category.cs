// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using System.ComponentModel.DataAnnotations;

namespace WeihanLi.Common.Models;

/// <summary>
/// Represents a category.
/// </summary>
/// <typeparam name="TKey">The category identifier type.</typeparam>
public record Category<TKey>
{
    /// <summary>
    /// Gets or sets the category identifier.
    /// </summary>
    public TKey Id { get; set; } = default!;

    /// <summary>
    /// Gets or sets the category name.
    /// </summary>
    [StringLength(256)]
    [Required]
    public string Name { get; set; } = null!;

    /// <summary>
    /// Gets or sets the parent category identifier.
    /// </summary>
    public TKey ParentId { get; set; } = default!;
}

/// <summary>
/// Represents a category with an integer identifier.
/// </summary>
public record Category : Category<int>;

/// <summary>
/// Represents a category with a description.
/// </summary>
public record CategoryWithDesc : Category<int>
{
    /// <summary>
    /// Gets or sets the category description.
    /// </summary>
    [StringLength(2048)]
    public string? Description { get; set; }
}
