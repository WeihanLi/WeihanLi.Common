// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using WeihanLi.Common.Models;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Helpers;

public static class EnumHelpers
{
    [RequiresUnreferencedCode("Generic TypeConverters may require the generic types to be annotated. For example, NullableConverter requires the underlying type to be DynamicallyAccessedMembers All.")]
    public static IReadOnlyList<IdNameModel<TValue>> ToIdNameList<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] TEnum, TValue>()
        where TEnum : Enum
    {
        var enumType = typeof(TEnum);
        return Array.ConvertAll(Enum.GetNames(enumType), name => new IdNameModel<TValue>()
        {
            Id = Enum.Parse(enumType, name).To<TValue>(),
            Name = name,
        });
    }

    [RequiresUnreferencedCode("Generic TypeConverters may require the generic types to be annotated. For example, NullableConverter requires the underlying type to be DynamicallyAccessedMembers All.")]
    public static IReadOnlyList<IdNameDescModel<TValue>> ToIdNameDescList<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] TEnum, TValue>() where TEnum : Enum
    {
        var enumType = typeof(TEnum);
        return Array.ConvertAll(Enum.GetNames(enumType), converter: name =>
        {
            var fieldInfo = enumType.GetField(name);
            return new IdNameDescModel<TValue>()
            {
                Id = Enum.Parse(enumType, name).To<TValue>(),
                Name = name,
                Description = fieldInfo!.GetDescription()
            };
        });
    }
}
