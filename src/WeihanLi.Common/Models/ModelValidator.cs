// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using System.ComponentModel.DataAnnotations;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Models;

public static class ModelValidator
{
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

    public static bool TryValidate(object instance, out IReadOnlyDictionary<string, string>? result)
    {
        if (TryValidate(instance, out IReadOnlyCollection<ValidationResult>? results))
        {
            result = null;
            return true;
        }

        result = results?.SelectMany(r => r.MemberNames.Select(m => new KeyValuePair<string, string>(m, r.ErrorMessage!)))
            .GroupBy(kv => kv.Key, kv => kv.Value)
            .ToDictionary(kv => kv.Key, kv => kv.First());
        return false;
    }

    private static bool TryValidate(object instance, out IReadOnlyCollection<ValidationResult>? result)
    {
        var results = new List<ValidationResult>();
        if (Validator.TryValidateObject(instance, new ValidationContext(instance, null, null), results, true))
        {
            result = null;
            return true;
        }

        result = results.AsReadOnly();
        return false;
    }
}
