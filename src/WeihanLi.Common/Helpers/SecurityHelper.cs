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
        var array = isNumberOnly ? _constantNumber : _constantCharacters;
        var charArray = new char[length];
        for (var i = 0; i < length; i++)
        {
            charArray[i] = array[Random.Next(array.Length)];
        }
        return new string(charArray);
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

    public static string AesEncrypt(
        string source, 
        string key, 
        bool isLower = false, 
        Action<Aes>? aesConfigure = null
        )
    {
        using var aesAlg = Aes.Create();
        aesAlg.Mode = CipherMode.ECB;
        aesAlg.Padding = PaddingMode.PKCS7;
        return aesAlg.Encrypt(source, key, isLower, aesConfigure);
    }

    public static string AesDecrypt(
        string encryptedText, 
        string key,
        Action<Aes>? aesConfigure = null
    )
    {
        using var aesAlg = Aes.Create();
        aesAlg.Mode = CipherMode.ECB;
        aesAlg.Padding = PaddingMode.PKCS7;
        return aesAlg.Decrypt(encryptedText, key, aesConfigure);
    }

    public static string Encrypt<TAlgorithm>(this TAlgorithm algorithm, 
        string source, string key, 
        bool isLowerCase = false,
        Action<TAlgorithm>? algorithmConfigure = null
        ) where TAlgorithm : SymmetricAlgorithm
    {
        Guard.NotNull(algorithm);
        var encryptedBytes = Encrypt(algorithm, source.GetBytes(), key.GetBytes(), algorithmConfigure);
        return encryptedBytes.ToHexString(isLowerCase);
    }

    public static byte[] Encrypt<TAlgorithm>(this TAlgorithm algorithm, 
        byte[] source, byte[] key, Action<TAlgorithm>? algorithmConfigure = null
    ) where TAlgorithm : SymmetricAlgorithm
    {
        Guard.NotNull(algorithm);
        algorithm.Key = key;
        algorithmConfigure?.Invoke(algorithm);
        var encryptor = algorithm.CreateEncryptor(algorithm.Key, algorithm.IV);
        using var ms = new MemoryStream();
        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        {
            cs.Write(source);
        }
        return ms.ToArray();
    }

    public static string Decrypt<TAlgorithm>(this TAlgorithm algorithm, 
        string encryptedHexString, string key, 
        Action<TAlgorithm>? algorithmConfigure = null
    ) where TAlgorithm : SymmetricAlgorithm
    {
        var encryptedBytes = Decrypt(algorithm, encryptedHexString.HexStringToBytes(), key.GetBytes(), algorithmConfigure);
        return encryptedBytes.GetString();
    }

    public static byte[] Decrypt<TAlgorithm>(this TAlgorithm algorithm, 
        byte[] encrypted, byte[] key, Action<TAlgorithm>? algorithmConfigure = null
    ) where TAlgorithm : SymmetricAlgorithm
    {
        Guard.NotNull(algorithm);
        algorithm.Key = key;
        algorithmConfigure?.Invoke(algorithm);
        var transform = algorithm.CreateDecryptor(algorithm.Key, algorithm.IV);
        using var ms = new MemoryStream();
        using(var cs = new CryptoStream(ms, transform, CryptoStreamMode.Write))
        {
            cs.Write(encrypted);   
        }
        return ms.ToArray();
    }
}
