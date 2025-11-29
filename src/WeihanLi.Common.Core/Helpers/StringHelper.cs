// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace WeihanLi.Common.Helpers;

public static class StringHelper
{
    #region 隐藏敏感信息

    /// <summary>
    /// 敏感信息字符
    /// </summary>
    public const char SensitiveChar = '*';

    /// <summary>
    /// 隐藏敏感信息
    /// </summary>
    /// <param name="info">信息实体</param>
    /// <param name="left">左边保留的字符数</param>
    /// <param name="right">右边保留的字符数</param>
    /// <param name="sensitiveCharCount">敏感字符数量</param>
    /// <param name="basedOnLeft">当长度异常时，是否显示左边 ，true显示左边，false显示右边 </param>
    /// <returns></returns>
    public static string HideSensitiveInfo(string? info, int left, int right, int sensitiveCharCount = 4, bool basedOnLeft = true)
    {
        if (string.IsNullOrEmpty(info))
        {
            return string.Empty;
        }

        if (right < 0) right = 0;
        if (left < 0) left = 0;

        if (info!.Length - left - right > 0)
        {
            return info[..left]
                .PadRight(left + sensitiveCharCount, SensitiveChar)
                .Insert(left + sensitiveCharCount, info[^right..]);
        }

        if (basedOnLeft)
        {
            return info.Length > left && left > 0
                ? info.Remove(left).Insert(left, new string(SensitiveChar, sensitiveCharCount))
                : info[..1].PadRight(1 + sensitiveCharCount, SensitiveChar);
        }

        return info.Length > right && right > 0
            ? info[^right..].PadLeft(right + sensitiveCharCount, SensitiveChar)
            : info[..1].PadLeft(1 + sensitiveCharCount, SensitiveChar);
    }

    /// <summary>
    /// 隐藏手机号详情
    /// </summary>
    /// <param name="phone">手机号</param>
    /// <param name="left">左边保留字符数</param>
    /// <param name="right">右边保留字符数</param>
    /// <returns></returns>
    public static string HideTelDetails(string? phone, int left = 3, int right = 4) => HideSensitiveInfo(phone, left, right);

    /// <summary>
    /// 隐藏邮箱地址详情
    /// </summary>
    /// <param name="email">邮箱地址</param>
    /// <param name="left">邮箱地址头保留字符个数，默认值设置为3</param>
    /// <returns></returns>
    public static string HideEmailDetails(string? email, int left = 3)
    {
        if (string.IsNullOrEmpty(email))
        {
            return string.Empty;
        }

        if (Regex.IsMatch(email!, @"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"))//如果是邮件地址
        {
            var suffixLen = email!.Length - email.LastIndexOf('@');
            return HideSensitiveInfo(email, left, suffixLen, basedOnLeft: false);
        }

        return HideSensitiveInfo(email, left, 0);
    }

    #endregion 隐藏敏感信息

    /// <summary>
    /// 去除html标记，转换成一般文本
    /// </summary>
    /// <param name="htmlStr">html文本</param>
    /// <param name="removeWhiteSpace">是否移除空格</param>
    /// <returns></returns>
    public static string Html2Text(string htmlStr, bool removeWhiteSpace = false)
    {
        if (string.IsNullOrEmpty(htmlStr))
        {
            return "";
        }
        var regEx_style = "<style[^>]*?>[\\s\\S]*?<\\/style>"; //定义style的正则表达式
        var regEx_script = "<script[^>]*?>[\\s\\S]*?<\\/script>"; //定义script的正则表达式
        var regEx_html = "<[^>]+>"; //定义HTML标签的正则表达式
        var str = Regex.Replace(htmlStr, regEx_style, "");//删除css
        str = Regex.Replace(str, regEx_script, "");//删除js
        str = Regex.Replace(str, regEx_html, "");//删除html标记
        str = Regex.Replace(str, "\t|\r|\n", " ");//去除tab、空行
        str = str.Replace("&nbsp;", " ");
        if (removeWhiteSpace)
        {
            str = htmlStr.Replace(" ", "");//去除空格
        }
        return str.Trim();
    }

    public const string Empty = "";
    public const string CarriageReturnLineFeed = "\r\n";
    public const char CarriageReturn = '\r';
    public const char LineFeed = '\n';
    public const char Tab = '\t';

    public static string ToPascalCase(string s)
    {
        if (string.IsNullOrEmpty(s) || !char.IsLower(s[0]))
        {
            return s;
        }

        var chars = s.ToCharArray();

        for (var i = 0; i < chars.Length; i++)
        {
            if (i == 1 && !char.IsUpper(chars[i]))
            {
                break;
            }

            var hasNext = (i + 1 < chars.Length);
            if (i > 0 && hasNext && !char.IsUpper(chars[i + 1]))
            {
                break;
            }

            chars[i] = ToUpper(chars[i]);
        }

        return new string(chars);
    }

    // https://github.com/JamesNK/Newtonsoft.Json/blob/master/Src/Newtonsoft.Json/Utilities/StringUtils.cs

    public static string ToCamelCase(string s)
    {
        if (string.IsNullOrEmpty(s) || !char.IsUpper(s[0]))
        {
            return s;
        }

        var chars = s.ToCharArray();

        for (var i = 0; i < chars.Length; i++)
        {
            if (i == 1 && !char.IsUpper(chars[i]))
            {
                break;
            }

            var hasNext = (i + 1 < chars.Length);
            if (i > 0 && hasNext && !char.IsUpper(chars[i + 1]))
            {
                // if the next character is a space, which is not considered uppercase
                // (otherwise we wouldn't be here...)
                // we want to ensure that the following:
                // 'FOO bar' is rewritten as 'foo bar', and not as 'foO bar'
                // The code was written in such a way that the first word in uppercase
                // ends when if finds an uppercase letter followed by a lowercase letter.
                // now a ' ' (space, (char)32) is considered not upper
                // but in that case we still want our current character to become lowercase
                if (char.IsSeparator(chars[i + 1]))
                {
                    chars[i] = ToLower(chars[i]);
                }

                break;
            }

            chars[i] = ToLower(chars[i]);
        }

        return new string(chars);
    }

    private static char ToLower(char c)
    {
        return char.ToLower(c, CultureInfo.InvariantCulture);
    }

    private static char ToUpper(char c)
    {
        return char.ToUpper(c, CultureInfo.InvariantCulture);
    }

    public static string ToSnakeCase(string s) => ToSeparatedCase(s, '_');

    public static string ToKebabCase(string s) => ToSeparatedCase(s, '-');

    private enum SeparatedCaseState
    {
        Start,
        Lower,
        Upper,
        NewWord
    }

    private static string ToSeparatedCase(string s, char separator)
    {
        if (string.IsNullOrEmpty(s))
        {
            return s;
        }

        var sb = new StringBuilder();
        var state = SeparatedCaseState.Start;

        for (var i = 0; i < s.Length; i++)
        {
            if (s[i] == ' ')
            {
                if (state != SeparatedCaseState.Start)
                {
                    state = SeparatedCaseState.NewWord;
                }
            }
            else if (char.IsUpper(s[i]))
            {
                switch (state)
                {
                    case SeparatedCaseState.Upper:
                        var hasNext = (i + 1 < s.Length);
                        if (i > 0 && hasNext)
                        {
                            var nextChar = s[i + 1];
                            if (!char.IsUpper(nextChar) && nextChar != separator)
                            {
                                sb.Append(separator);
                            }
                        }
                        break;

                    case SeparatedCaseState.Lower:
                    case SeparatedCaseState.NewWord:
                        sb.Append(separator);
                        break;
                }

                var c = char.ToLower(s[i], CultureInfo.InvariantCulture);

                sb.Append(c);

                state = SeparatedCaseState.Upper;
            }
            else if (s[i] == separator)
            {
                sb.Append(separator);
                state = SeparatedCaseState.Start;
            }
            else
            {
                if (state == SeparatedCaseState.NewWord)
                {
                    sb.Append(separator);
                }

                sb.Append(s[i]);
                state = SeparatedCaseState.Lower;
            }
        }

        return sb.ToString();
    }

    public static bool StartsWith(this string? source, char value)
    {
        return !string.IsNullOrEmpty(source) && source![0] == value;
    }

    public static bool EndsWith(this string source, char value)
    {
        return !string.IsNullOrEmpty(source) && source[^1] == value;
    }
}
