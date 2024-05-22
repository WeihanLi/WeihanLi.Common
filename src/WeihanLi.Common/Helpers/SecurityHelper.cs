﻿using System.Security.Cryptography;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Helpers;

public static class SecurityHelper
{
    private static readonly char[] _constantLetterCharacters = 
    [
        'A',
        'B',
        'C',
        'D',
        'E',
        'F',
        'G',
        
        'H',
        'I',
        'J',
        'K',
        'L',
        'M',
        'N',
        'O',
        'P',
        'Q',
        'R',
        'S',
        'T',
        'U',
        'V',
        'W',
        'X',
        'Y',
        'Z'
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
    
    private static readonly char[] _constantHexNumber = 
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
        'A',
        'B',
        'C',
        'D',
        'E',
        'F'
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
        if (isNumberOnly)
        {
            return GenerateRandomCode(length, RandomCodeType.Number);
        }

        var charArray = new char[length];
        var maxLength = _constantNumber.Length + _constantLetterCharacters.Length;
        for (var i = 0; i < length; i++)
        {
            var idx = Random.Next(maxLength);
            if (idx < _constantNumber.Length)
            {
                charArray[i] = _constantNumber[idx];    
            }
            else
            {
                charArray[i] = _constantLetterCharacters[idx - _constantNumber.Length];
            }
        }

        return new string(charArray);
    }
    
    public static string GenerateRandomCode(int length, RandomCodeType type)
    {
        if (type == RandomCodeType.LetterOrNumber) return GenerateRandomCode(length, false);

        var array = type switch
        {
            RandomCodeType.Number => _constantNumber,
            RandomCodeType.Letter => _constantLetterCharacters,
            RandomCodeType.HexNumber => _constantHexNumber,
            _ => throw new NotSupportedException($"Type {type} is not supported")
        };
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
        string? iv = null,
        bool isLower = false, 
        Action<Aes>? aesConfigure = null
        )
    {
        using var aesAlg = Aes.Create();
        aesAlg.Padding = PaddingMode.PKCS7;
        if (string.IsNullOrEmpty(iv))
        {
            aesAlg.Mode = CipherMode.ECB;
        }
        else
        {
            aesAlg.Mode = CipherMode.CBC;
            aesAlg.IV = iv!.GetBytes();
        }
        
        return aesAlg.Encrypt(source, key, isLower, aesConfigure);
    }

    public static string AesDecrypt(
        string encryptedText, 
        string key,
        string? iv = null,
        Action<Aes>? aesConfigure = null
    )
    {
        using var aesAlg = Aes.Create();
        aesAlg.Padding = PaddingMode.PKCS7;
        if (string.IsNullOrEmpty(iv))
        {
            aesAlg.Mode = CipherMode.ECB;
        }
        else
        {
            aesAlg.Mode = CipherMode.CBC;
            aesAlg.IV = iv!.GetBytes();
        }
        
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
        using var encryptor = algorithm.CreateEncryptor();
        return encryptor.TransformFinalBlock(source, 0, source.Length);
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
        using var transform = algorithm.CreateDecryptor();
        return transform.TransformFinalBlock(encrypted, 0, encrypted.Length);
    }
}


public enum RandomCodeType
{
    Number = 0,
    Letter = 1,
    LetterOrNumber = 2,
    HexNumber = 3
}
