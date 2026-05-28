// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

namespace WeihanLi.Common.Models;

/// <summary>
/// Represents the result of a validation operation.
/// </summary>
public interface IValidationResult
{
    /// <summary>
    /// Gets a value indicating whether validation succeeded.
    /// </summary>
    bool Valid { get; }

    /// <summary>
    /// Gets validation errors keyed by member name.
    /// </summary>
    Dictionary<string, string[]> Errors { get; }
}

/// <summary>
/// Default implementation of <see cref="IValidationResult"/>.
/// </summary>
public sealed class ValidationResult : IValidationResult
{
    private Dictionary<string, string[]> _errors = [];

    /// <inheritdoc cref="IValidationResult"/>
    public bool Valid { get; set; }

    /// <inheritdoc cref="IValidationResult"/>
    public Dictionary<string, string[]> Errors
    {
        get => _errors;
        set => _errors = Guard.NotNull(value);
    }

    /// <summary>
    /// Creates a failed validation result with model-level errors.
    /// </summary>
    /// <param name="errors">The validation error messages.</param>
    /// <returns>A failed validation result.</returns>
    public static ValidationResult Failed(params string[] errors)
    {
        var result = new ValidationResult
        {
            Errors =
            {
                [string.Empty] = errors
            }
        };
        return result;
    }

    /// <summary>
    /// Creates a failed validation result with member-level errors.
    /// </summary>
    /// <param name="errors">The validation errors keyed by member name.</param>
    /// <returns>A failed validation result.</returns>
    public static ValidationResult Failed(Dictionary<string, string[]> errors)
    {
        var result = new ValidationResult
        {
            Errors = errors
        };
        return result;
    }
}
