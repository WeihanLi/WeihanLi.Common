using System.Security.Cryptography;
using System.Text;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Helpers;

/// <summary>
/// 安全助手
/// </summary>
public static class SecurityHelper
{
    private static readonly char[] _constantCharacters = 
    [
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
    ];

    private static readonly char[] _constantNumber = 
    [
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
    ];

#if NET6_0_OR_GREATER
        public static Random Random => Random.Shared;
#else

    [ThreadStatic]
    private static Random? _random;

    /// <summary>
    /// Thread-safe Random instance
    /// </summary>
    public static Random Random
    {
        get
        {
            _random ??= new();
            return _random;
        }
    }

#endif

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

    public static string Aes(
        string plainText, 
        string key, 
        bool isLower = false, 
        Action<Aes>? aesConfigure = null
        )
    {
        using var aesAlg = System.Security.Cryptography.Aes.Create();
        aesAlg.Key = key.GetBytes();
        aesAlg.Mode = CipherMode.ECB;
        aesAlg.Padding = PaddingMode.PKCS7;
        aesConfigure?.Invoke(aesAlg);
        
        var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
        using var ms = new MemoryStream();
        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        {
            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            cs.Write(plainBytes);
        }
        var bytes = ms.ToArray();
#if NET9_0_OR_GREATER
        return isLower ? Convert.ToHexStringLower(bytes) : Convert.ToHexString(bytes);
#elif NET5_0_OR_GREATER
        return isLower ? Convert.ToHexString(bytes).ToLowerInvariant() : Convert.ToHexString(bytes);
#else
        var bitString = BitConverter.ToString(bytes).Replace("-","");
        return isLower ? bitString.ToLowerInvariant() : bitString;
#endif
    }
}
