using System.Diagnostics.CodeAnalysis;
using WeihanLi.Common.Models;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Helpers;

public static class EnumHelper
{
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)]
    public static IReadOnlyList<IdNameModel> ToIdNameList<TEnum>() where TEnum : Enum
    {
        var enumType = typeof(TEnum);
        return Array.ConvertAll(Enum.GetNames(enumType), name => new IdNameModel()
        {
            Name = name,
            Id = Convert.ToInt32(Enum.Parse(enumType, name))
        });
    }

    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)]
    public static IReadOnlyList<IdNameModel<TValue>> ToIdNameList<TEnum, TValue>() where TEnum : Enum
    {
        var enumType = typeof(TEnum);
        return Array.ConvertAll(Enum.GetNames(enumType), name => new IdNameModel<TValue>()
        {
            Id = Enum.Parse(enumType, name).To<TValue>(),
            Name = name,
        });
    }

    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)]
    public static IReadOnlyList<IdNameDescModel> ToIdNameDescList<TEnum>() where TEnum : Enum
    {
        var enumType = typeof(TEnum);
        return Array.ConvertAll(Enum.GetNames(enumType), converter: name => new IdNameDescModel()
        {
            Name = name,
            Id = Convert.ToInt32(Enum.Parse(enumType, name)),
            Description = enumType.GetField(name)?.GetDescription()
        });
    }

    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)]
    public static IReadOnlyList<IdNameDescModel<TValue>> ToIdNameDescList<TEnum, TValue>() where TEnum : Enum
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
