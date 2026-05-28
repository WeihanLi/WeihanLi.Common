// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

namespace WeihanLi.Common.Models;

/// <summary>
/// Represents a model with an identifier and name.
/// </summary>
/// <typeparam name="TKey">The identifier type.</typeparam>
public record IdNameModel<TKey>
{
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    public TKey Id { get; set; } = default!;

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Deconstructs the model into identifier and name values.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="name">The name.</param>
    public void Deconstruct(out TKey id, out string name)
    {
        id = Id;
        name = Name;
    }
}

/// <summary>
/// Represents a model with an identifier, name, and description.
/// </summary>
/// <typeparam name="TKey">The identifier type.</typeparam>
public record IdNameDescModel<TKey> : IdNameModel<TKey>
{
    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Deconstructs the model into identifier, name, and description values.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="name">The name.</param>
    /// <param name="description">The description.</param>
    public void Deconstruct(out TKey id, out string name, out string? description)
    {
        id = Id;
        name = Name;
        description = Description;
    }
}

/// <summary>
/// Represents a model with an integer identifier and name.
/// </summary>
public record IdNameModel : IdNameModel<int>;

/// <summary>
/// Represents a model with an integer identifier, name, and description.
/// </summary>
public record IdNameDescModel : IdNameDescModel<int>;
