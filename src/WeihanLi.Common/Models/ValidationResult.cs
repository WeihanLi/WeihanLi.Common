// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

namespace WeihanLi.Common.Models;

public interface IValidationResult
{
    /// <summary>
    /// Valid
    /// </summary>
    bool Valid { get; }

    /// <summary>
    /// ErrorMessages
    /// Key: memberName
    /// Value: errorMessages
    /// </summary>
    Dictionary<string, string[]> Errors { get; }
}

public sealed class ValidationResult: IValidationResult
{
    private Dictionary<string, string[]> _errors = new();
    
    /// <inheritdoc cref="IValidationResult"/>
    public bool Valid { get; set; }

    /// <inheritdoc cref="IValidationResult"/>
    public Dictionary<string, string[]> Errors
    {
        get => _errors;
        set => _errors = Guard.NotNull(value);
    }

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
    
    public static ValidationResult Failed(Dictionary<string, string[]> errors)
    {
        var result = new ValidationResult
        {
            Errors = errors
        };
        return result;
    }
}
