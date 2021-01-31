using System;
using System.Text;

namespace WeihanLi.Common.Helpers
{
    /// <summary>
    /// 安全助手
    /// </summary>
    public static class SecurityHelper
    {
        private static readonly char[] _constantCharacters = new[]
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

        private static readonly char[] _constantNumber = new[]
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

        public static readonly Random Random = new();

        /// <summary>
        /// 生成随机验证码
        /// </summary>
        /// <param name="length">验证码长度</param>
        /// <param name="isNumberOnly">验证码是否是纯数字</param>
        /// <returns></returns>
        public static string GenerateRandomCode(int length, bool isNumberOnly = false)
        {
            char[] array;
            if (isNumberOnly)
            {
                array = _constantNumber;
            }
            else
            {
                array = _constantCharacters;
            }
            var stringBuilder = new StringBuilder(length);
            for (var i = 0; i < length; i++)
            {
                stringBuilder.Append(array[Random.Next(array.Length)]);
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// get MD5 hashed string
        /// </summary>
        public static string MD5(string sourceString, bool isLower = false)
        {
            return string.IsNullOrEmpty(sourceString) ? string.Empty : HashHelper.GetHashedString(HashType.MD5, sourceString, isLower);
        }

        /// <summary>
        /// get SHA1 hashed string
        /// </summary>
        public static string SHA1(string sourceString, bool isLower = false)
        {
            return string.IsNullOrEmpty(sourceString) ? string.Empty : HashHelper.GetHashedString(HashType.SHA1, sourceString, isLower);
        }

        /// <summary>
        /// get SHA256 hashed string
        /// </summary>
        public static string SHA256(string sourceString, bool isLower = false)
        {
            return string.IsNullOrEmpty(sourceString) ? string.Empty : HashHelper.GetHashedString(HashType.SHA256, sourceString, isLower);
        }

        /// <summary>
        /// get SHA512 hashed string
        /// </summary>
        public static string SHA512(string sourceString, bool isLower = false)
        {
            return string.IsNullOrEmpty(sourceString) ? string.Empty : HashHelper.GetHashedString(HashType.SHA512, sourceString, isLower);
        }
    }
}
