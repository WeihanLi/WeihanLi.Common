using System;
using System.Security.Cryptography;
using System.Text;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Helpers
{
    /// <summary>
    /// 安全助手
    /// </summary>
    public static class SecurityHelper
    {
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

        public static readonly Random Random = new Random();

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
            for (var i = 0; i < length; i++)
            {
                stringBuilder.Append(array[Random.Next(num)]);
            }
            return stringBuilder.ToString();
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
        /// <returns>hash过的字节数组</returns>
        public static byte[] GetHashedBytes(HashType type, string str, string key) => GetHashedBytes(type, key.IsNotNullOrEmpty() ? key.GetBytes() : null, str.GetBytes());

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
        /// <param name="key">key</param>
        /// <param name="isLower">是否是小写</param>
        /// <returns>哈希算法处理之后的字符串</returns>
        public static string GetHashedString(HashType type, string str, string key, bool isLower) => GetHashedString(type, str, key, Encoding.UTF8, isLower);

        /// <summary>
        /// 获取哈希之后的字符串
        /// </summary>
        /// <param name="type">哈希类型</param>
        /// <param name="str">源字符串</param>
        /// <param name="encoding">编码类型</param>
        /// <param name="isLower">是否是小写</param>
        /// <returns>哈希算法处理之后的字符串</returns>
        public static string GetHashedString(HashType type, string str, Encoding encoding, bool isLower = false) => GetHashedString(type, str, null, encoding, isLower);

        /// <summary>
        /// 获取哈希之后的字符串
        /// </summary>
        /// <param name="type">哈希类型</param>
        /// <param name="str">源字符串</param>
        /// <param name="key">key</param>
        /// <param name="encoding">编码类型</param>
        /// <param name="isLower">是否是小写</param>
        /// <returns>哈希算法处理之后的字符串</returns>
        public static string GetHashedString(HashType type, string str, string key, Encoding encoding, bool isLower = false)
        {
            if (string.IsNullOrEmpty(str))
            {
                return "";
            }
            var hashedBytes = GetHashedBytes(type, key.IsNotNullOrEmpty() ? encoding.GetBytes(key) : null, encoding.GetBytes(str));
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
        public static byte[] GetHashedBytes(HashType type, byte[] bytes) => GetHashedBytes(type, null, bytes);

        /// <summary>
        /// 获取Hash后的字节数组
        /// </summary>
        /// <param name="type">哈希类型</param>
        /// <param name="key">key</param>
        /// <param name="bytes">原字节数组</param>
        /// <returns></returns>
        public static byte[] GetHashedBytes(HashType type, byte[] key, byte[] bytes)
        {
            HashAlgorithm algorithm = null;
            try
            {
                if (key == null)
                {
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
                }
                else
                {
                    switch (type)
                    {
                        case HashType.MD5:
                            algorithm = new HMACMD5(key);
                            break;

                        case HashType.SHA1:
                            algorithm = new HMACSHA1(key);
                            break;

                        case HashType.SHA256:
                            algorithm = new HMACSHA256(key);
                            break;

                        case HashType.SHA384:
                            algorithm = new HMACSHA384(key);
                            break;

                        case HashType.SHA512:
                            algorithm = new HMACSHA512(key);
                            break;

                        default:
                            algorithm = new HMACMD5(key);
                            break;
                    }
                }
                return algorithm.ComputeHash(bytes);
            }
            finally
            {
                algorithm?.Dispose();
            }
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
