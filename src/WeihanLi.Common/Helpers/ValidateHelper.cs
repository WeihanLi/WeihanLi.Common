// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using System.Text.RegularExpressions;

namespace WeihanLi.Common.Helpers;

/// <summary>
/// 常用验证帮助类
/// </summary>
public static class ValidateHelper
{
    //邮件正则表达式
    private static readonly Regex EmailRegex = new(@"^[a-z0-9]([a-z0-9]*[-_]?[a-z0-9]+)*@([a-z0-9]*[-_]?[a-z0-9]+)+[\.][a-z]{2,3}([\.][a-z]{2})?$", RegexOptions.IgnoreCase);

    //手机号正则表达式
    private static readonly Regex MobileRegex = new("^1[2-9][0-9]{9}$", RegexOptions.Compiled);

    /// <summary>
    /// 是否为邮箱名
    /// </summary>
    public static bool IsEmail(string? s)
    {
        if (string.IsNullOrEmpty(s))
        {
            return false;
        }
        return EmailRegex.IsMatch(s!);
    }

    /// <summary>
    /// 是否为手机号
    /// </summary>
    public static bool IsMobile(string? s)
    {
        if (string.IsNullOrEmpty(s))
        {
            return false;
        }
        return MobileRegex.IsMatch(s!);
    }

    /// <summary>
    /// 是否为 IP v4 地址
    /// </summary>
    public static bool IsIP(string s)
    {
        if (string.IsNullOrEmpty(s))
            return false;

        var splits = s.Split('.');
        if (splits.Length is not 4)
            return false;

        return splits.All(s => byte.TryParse(s, out _));
    }

    /// <summary>
    /// 是否是身份证号
    /// </summary>
    public static bool IsIdCard(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return false;
        }
        if (id.Length == 18)
        {
            return CheckIDCard18(id);
        }
        else if (id.Length == 15)
        {
            return CheckIDCard15(id);
        }
        return false;
    }

    /// <summary>
    /// 是否为18位身份证号
    /// </summary>
    private static bool CheckIDCard18(string? id)
    {
        if (long.TryParse(id!.Remove(17), out var n) == false || n < Math.Pow(10, 16) || long.TryParse(id.Replace('x', '0').Replace('X', '0'), out n) == false)
        {
            return false;//数字验证
        }

        var address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
        if (address.IndexOf(id.Remove(2), StringComparison.OrdinalIgnoreCase) == -1)
        {
            return false;//省份验证
        }

        var birth = id.Substring(6, 8).Insert(6, "-").Insert(4, "-");
        if (DateTime.TryParse(birth, out _) == false)
            return false;//生日验证

        var arrVarifyCode = ("1,0,x,9,8,7,6,5,4,3,2").Split(',');
        var Wi = ("7,9,10,5,8,4,2,1,6,3,7,9,10,5,8,4,2").Split(',');
        var Ai = id.Remove(17).ToCharArray();
        var sum = 0;
        for (var i = 0; i < 17; i++)
        {
            sum += int.Parse(Wi[i]) * int.Parse(Ai[i].ToString());
        }

        var y = sum % 11;
        if (!arrVarifyCode[y].Equals(id.Substring(17, 1), StringComparison.CurrentCultureIgnoreCase))
        {
            return false;//校验码验证
        }

        return true;//符合GB11643-1999标准
    }

    /// <summary>
    /// 是否为15位身份证号
    /// </summary>
    private static bool CheckIDCard15(string? id)
    {
        if (long.TryParse(id, out var n) == false || n < Math.Pow(10, 14))
        {
            return false;//数字验证
        }

        var address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
        if (address.IndexOf(id!.Remove(2), StringComparison.OrdinalIgnoreCase) == -1)
        {
            return false;//省份验证
        }

        var birth = id.Substring(6, 6).Insert(4, "-").Insert(2, "-");
        if (DateTime.TryParse(birth, out _) == false)
        {
            return false;//生日验证
        }

        return true;//符合15位身份证标准
    }

    /// <summary>
    /// 是否为邮政编码
    /// </summary>
    public static bool IsZipCode(string? s)
    {
        if (string.IsNullOrEmpty(s))
        {
            return false;
        }
        return s!.Length is 6 && int.TryParse(s, out _);
    }
}
