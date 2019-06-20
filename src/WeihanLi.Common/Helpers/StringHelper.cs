using System.Text.RegularExpressions;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Helpers
{
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
        public static string HideSensitiveInfo(string info, int left, int right, int sensitiveCharCount = 4, bool basedOnLeft = true)
        {
            if (string.IsNullOrEmpty(info))
            {
                return "";
            }

            if (right < 0) right = 0;
            if (left < 0) left = 0;

            if (info.Length - left - right > 0)
            {
                return info.Substring(0, left)
                    .PadRight(left + sensitiveCharCount, SensitiveChar)
                    .Insert(left + sensitiveCharCount, info.Substring(info.Length - right));
            }

            if (basedOnLeft)
            {
                return info.Length > left && left > 0
                    ? info.Remove(left).Insert(left, new string(SensitiveChar, sensitiveCharCount))
                    : info.Substring(0, 1).PadRight(1 + sensitiveCharCount, SensitiveChar);
            }

            return info.Length > right && right > 0
                ? info.Substring(info.Length - right).PadLeft(right + sensitiveCharCount, SensitiveChar)
                : info.Substring(0, 1).PadLeft(1 + sensitiveCharCount, SensitiveChar);
        }

        /// <summary>
        /// 隐藏手机号详情
        /// </summary>
        /// <param name="phone">手机号</param>
        /// <param name="left">左边保留字符数</param>
        /// <param name="right">右边保留字符数</param>
        /// <returns></returns>
        public static string HideTelDetails(string phone, int left = 3, int right = 4) => HideSensitiveInfo(phone, left, right);

        /// <summary>
        /// 隐藏邮箱地址详情
        /// </summary>
        /// <param name="email">邮箱地址</param>
        /// <param name="left">邮箱地址头保留字符个数，默认值设置为3</param>
        /// <returns></returns>
        public static string HideEmailDetails(string email, int left = 3)
        {
            if (string.IsNullOrEmpty(email))
            {
                return "";
            }

            if (email.IsMatch(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"))//如果是邮件地址
            {
                var suffixLen = email.Length - email.LastIndexOf('@');
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
    }
}
