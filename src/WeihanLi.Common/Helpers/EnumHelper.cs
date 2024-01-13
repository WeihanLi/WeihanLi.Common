using System.Diagnostics.CodeAnalysis;
using WeihanLi.Common.Models;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Helpers;

public static class EnumHelper
{
    public static IReadOnlyList<IdNameModel> ToIdNameList<TEnum>() where TEnum : Enum
    {
        var enumType = typeof(TEnum);
        return Array.ConvertAll(Enum.GetNames(enumType), name => new IdNameModel()
        {
            Name = name,
            Id = Convert.ToInt32(Enum.Parse(enumType, name))
        });
    }

    [RequiresUnreferencedCode("Generic TypeConverters may require the generic types to be annotated. For example, NullableConverter requires the underlying type to be DynamicallyAccessedMembers All.")]
    public static IReadOnlyList<IdNameModel<TValue>> ToIdNameList<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] TEnum, TValue>() where TEnum : Enum
    {
        var enumType = typeof(TEnum);
        return Array.ConvertAll(Enum.GetNames(enumType), name => new IdNameModel<TValue>()
        {
            Id = Enum.Parse(enumType, name).To<TValue>(),
            Name = name,
        });
    }

    public static IReadOnlyList<IdNameDescModel> ToIdNameDescList<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] TEnum>() where TEnum : Enum
    {
        var enumType = typeof(TEnum);
        return Array.ConvertAll(Enum.GetNames(enumType), converter: name => new IdNameDescModel()
        {
            Name = name,
            Id = Convert.ToInt32(Enum.Parse(enumType, name)),
            Description = enumType.GetField(name)?.GetDescription()
        });
    }

    [RequiresUnreferencedCode("Generic TypeConverters may require the generic types to be annotated. For example, NullableConverter requires the underlying type to be DynamicallyAccessedMembers All.")]
    public static IReadOnlyList<IdNameDescModel<TValue>> ToIdNameDescList<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] TEnum, TValue>() where TEnum : Enum
    {
        var enumType = typeof(TEnum);
        return Array.ConvertAll(Enum.GetNames(enumType), converter: name => new IdNameDescModel<TValue>()
        {
            Id = Enum.Parse(enumType, name).To<TValue>(),
            Name = name,
            Description = enumType.GetField(name)?.GetDescription()
        });
    }
}
