using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace WeihanLi.Common.Helpers
{
    /// <summary>
    /// 安全助手
    /// </summary>
    public static class SecurityHelper
    {
        //防SQL注入正则表达式1
        private static readonly Regex Sqlkeywordregex1 = new Regex(@"(select|insert|delete|from|count\(|drop|table|update|truncate|asc\(|mid\(|char\(|xp_cmdshell|exec|master|net|local|group|administrators|user|or|and)", RegexOptions.IgnoreCase);

        //防SQL注入正则表达式2
        private static readonly Regex Sqlkeywordregex2 = new Regex(@"(select|insert|delete|from|count\(|drop|table|update|truncate|asc\(|mid\(|char\(|xp_cmdshell|exec|master|net|local|group|administrators|user|or|and|-|;|,|\(|\)|\[|\]|\{|\}|%|@|\*|!|\')", RegexOptions.IgnoreCase);

        private static readonly char[] Constant = new[]
        {
            '0',
            '1',
            '2',
            '3',
            '4',
            '5',
            '6',
            '7',
            '8',
            '9',
            'a',
            'b',
            'c',
            'd',
            'e',
            'f',
            'g',
            'h',
            'i',
            'j',
            'k',
            'l',
            'm',
            'n',
            'o',
            'p',
            'q',
            'r',
            's',
            't',
            'u',
            'v',
            'w',
            'x',
            'y',
            'z'
        };

        private static readonly char[] ConstantNumber = new[]
        {
            '0',
            '1',
            '2',
            '3',
            '4',
            '5',
            '6',
            '7',
            '8',
            '9'
        };

        /// <summary>
        /// 生成随机验证码
        /// </summary>
        /// <param name="length">验证码长度</param>
        /// <param name="isNumberOnly">验证码是否是纯数字</param>
        /// <returns></returns>
        public static string GenerateRandomCode(int length, bool isNumberOnly = false)
        {
            int num;
            char[] array;
            if (isNumberOnly)
            {
                num = ConstantNumber.Length;
                array = ConstantNumber;
            }
            else
            {
                num = Constant.Length;
                array = Constant;
            }
            var stringBuilder = new StringBuilder(num);
            var random = new Random();
            for (var i = 0; i < length; i++)
            {
                stringBuilder.Append(array[random.Next(num)]);
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// 判断当前字符串是否存在SQL注入
        /// </summary>
        /// <param name="sql">sqlstring</param>
        /// <param name="isStrict">是否严格控制</param>
        /// <returns></returns>
        public static bool IsSafeSqlString(string sql, bool isStrict = true)
        {
            if (sql != null)
            {
                if (isStrict)
                {
                    return !Sqlkeywordregex2.IsMatch(sql);
                }
                else
                {
                    return !Sqlkeywordregex1.IsMatch(sql);
                }
            }
            return true;
        }

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="sourceString">原字符串</param>
        /// <param name="isLower">加密后的字符串是否为小写</param>
        /// <returns>加密后字符串</returns>
        public static string MD5_Encrypt(string sourceString, bool isLower = false)
        {
            if (string.IsNullOrEmpty(sourceString))
            {
                return "";
            }
            return HashHelper.GetHashedString(HashType.MD5, sourceString, isLower);
        }

        /// <summary>
        /// use sha1 to encrypt string
        /// </summary>
        public static string SHA1_Encrypt(string sourceString, bool isLower = false)
        {
            if (string.IsNullOrEmpty(sourceString))
            {
                return "";
            }
            return HashHelper.GetHashedString(HashType.SHA1, sourceString, isLower);
        }

        /// <summary>
        /// SHA256 加密
        /// </summary>
        public static string SHA256_Encrypt(string sourceString, bool isLower = false)
        {
            if (string.IsNullOrEmpty(sourceString))
            {
                return "";
            }
            return HashHelper.GetHashedString(HashType.SHA256, sourceString, isLower);
        }

        /// <summary>
        /// SHA512_加密
        /// </summary>
        public static string SHA512_Encrypt(string sourceString, bool isLower = false)
        {
            if (string.IsNullOrEmpty(sourceString))
            {
                return "";
            }
            return HashHelper.GetHashedString(HashType.SHA512, sourceString, isLower);
        }
    }

    /// <summary>
    /// HashHelper
    /// </summary>
    public static class HashHelper
    {
        /// <summary>
        /// 计算字符串Hash值
        /// </summary>
        /// <param name="type">hash类型</param>
        /// <param name="str">要hash的字符串</param>
        /// <returns>hash过的字节数组</returns>
        public static byte[] GetHashedBytes(HashType type, string str) => GetHashedBytes(type, str, Encoding.UTF8);

        /// <summary>
        /// 计算字符串Hash值
        /// </summary>
        /// <param name="type">hash类型</param>
        /// <param name="str">要hash的字符串</param>
        /// <param name="encoding">编码类型</param>
        /// <returns>hash过的字节数组</returns>
        public static byte[] GetHashedBytes(HashType type, string str, Encoding encoding)
        {
            var bytes = encoding.GetBytes(str);
            return GetHashedBytes(type, bytes);
        }

        /// <summary>
        /// 获取哈希之后的字符串
        /// </summary>
        /// <param name="type">哈希类型</param>
        /// <param name="str">源字符串</param>
        /// <returns>哈希算法处理之后的字符串</returns>
        public static string GetHashedString(HashType type, string str) => GetHashedString(type, str, Encoding.UTF8);

        /// <summary>
        /// 获取哈希之后的字符串
        /// </summary>
        /// <param name="type">哈希类型</param>
        /// <param name="str">源字符串</param>
        /// <param name="isLower">是否是小写</param>
        /// <returns>哈希算法处理之后的字符串</returns>
        public static string GetHashedString(HashType type, string str, bool isLower) => GetHashedString(type, str, Encoding.UTF8, isLower);

        /// <summary>
        /// 获取哈希之后的字符串
        /// </summary>
        /// <param name="type">哈希类型</param>
        /// <param name="str">源字符串</param>
        /// <param name="encoding">编码类型</param>
        /// <param name="isLower">是否是小写</param>
        /// <returns>哈希算法处理之后的字符串</returns>
        public static string GetHashedString(HashType type, string str, Encoding encoding, bool isLower = false)
        {
            if (string.IsNullOrEmpty(str))
            {
                return "";
            }
            var hashedBytes = GetHashedBytes(type, str, encoding);
            var sbText = new StringBuilder();
            if (isLower)
            {
                foreach (var b in hashedBytes)
                {
                    sbText.Append(b.ToString("x2"));
                }
            }
            else
            {
                foreach (var b in hashedBytes)
                {
                    sbText.Append(b.ToString("X2"));
                }
            }
            return sbText.ToString();
        }

        /// <summary>
        /// 获取Hash后的字节数组
        /// </summary>
        /// <param name="type">哈希类型</param>
        /// <param name="bytes">原字节数组</param>
        /// <returns></returns>
        public static byte[] GetHashedBytes(HashType type, byte[] bytes)
        {
            HashAlgorithm algorithm;
            switch (type)
            {
                case HashType.MD5:
                    algorithm = MD5.Create();
                    break;

                case HashType.SHA1:
                    algorithm = SHA1.Create();
                    break;

                case HashType.SHA256:
                    algorithm = SHA256.Create();
                    break;

                case HashType.SHA384:
                    algorithm = SHA384.Create();
                    break;

                case HashType.SHA512:
                    algorithm = SHA512.Create();
                    break;

                default:
                    algorithm = MD5.Create();
                    break;
            }
            var hashedBytes = algorithm.ComputeHash(bytes);
            algorithm.Dispose();
            return hashedBytes;
        }
    }

    /// <summary>
    /// Hash 类型
    /// </summary>
    public enum HashType
    {
        /// <summary>
        /// MD5
        /// </summary>
        MD5 = 0,

        /// <summary>
        /// SHA1
        /// </summary>
        SHA1 = 1,

        /// <summary>
        /// SHA256
        /// </summary>
        SHA256 = 2,

        /// <summary>
        /// SHA384
        /// </summary>
        SHA384 = 3,

        /// <summary>
        /// SHA512
        /// </summary>
        SHA512 = 4
    }
}
