// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Models;

public static class ModelValidator
{
    [RequiresUnreferencedCode("The Type of instance cannot be statically discovered.")]
    public static bool TryValidate(object instance, out string? result)
    {
        if (TryValidate(instance, out IReadOnlyDictionary<string, string>? results))
        {
            result = null;
            return true;
        }

        result = results?.ToJson();
        return false;
    }

    [RequiresUnreferencedCode("The Type of instance cannot be statically discovered.")]
    public static bool TryValidate(object instance, out IReadOnlyDictionary<string, string>? result)
    {
        if (TryValidate(instance, out IReadOnlyCollection<System.ComponentModel.DataAnnotations.ValidationResult>? results))
        {
            result = null;
            return true;
        }

        result = results?.SelectMany(r => r.MemberNames.Select(m => new KeyValuePair<string, string>(m, r.ErrorMessage!)))
            .GroupBy(kv => kv.Key, kv => kv.Value)
            .ToDictionary(kv => kv.Key, kv => kv.First());
        return false;
    }

    [RequiresUnreferencedCode("The Type of instance cannot be statically discovered.")]
    private static bool TryValidate(object instance, out IReadOnlyCollection<System.ComponentModel.DataAnnotations.ValidationResult>? result)
    {
        var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
        if (Validator.TryValidateObject(instance, new ValidationContext(instance, null, null), results, true))
        {
            result = null;
            return true;
        }

        result = results.AsReadOnly();
        return false;
    }
}
