using System.ComponentModel;
using System.Reflection;
using WeihanLi.Common.Models;

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
    
    public static IReadOnlyList<IdNameDescModel> ToIdNameDescList<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] TEnum>() where TEnum : Enum
    {
        var enumType = typeof(TEnum);
        return Array.ConvertAll(Enum.GetNames(enumType), converter: name => new IdNameDescModel()
        {
            Name = name,
            Id = Convert.ToInt32(Enum.Parse(enumType, name)),
            Description = GetDescription(enumType.GetField(name))
        });
    }

    private static string? GetDescription(MemberInfo? memberInfo)
    {
        return memberInfo?.GetCustomAttribute<DescriptionAttribute>()?.Description ?? memberInfo?.Name;
    }
}
