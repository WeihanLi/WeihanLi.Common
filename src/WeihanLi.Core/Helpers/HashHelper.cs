using System.Security.Cryptography;
using System.Text;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Helpers;

public static class HashHelper
{
    public static string GetHashedString(HashType type, string str) => GetHashedString(type, str, Encoding.UTF8);

    public static string GetHashedString(HashType type, string str, bool isLower) => GetHashedString(type, str, Encoding.UTF8, isLower);

    public static string GetHashedString(HashType type, string str, string? key, bool isLower = false) => GetHashedString(type, str, key, Encoding.UTF8, isLower);

    public static string GetHashedString(HashType type, string str, Encoding encoding, bool isLower = false) => GetHashedString(type, str, null, encoding, isLower);

    public static string GetHashedString(HashType type, string str, string? key, Encoding encoding, bool isLower = false)
    {
        return string.IsNullOrEmpty(str) ? string.Empty : GetHashedString(type, str.GetBytes(encoding), string.IsNullOrEmpty(key) ? null : encoding.GetBytes(key!), isLower);
    }

    public static string GetHashedString(HashType type, byte[] source) => GetHashedString(type, source, null);

    public static string GetHashedString(HashType type, byte[] source, bool isLower) => GetHashedString(type, source, null, isLower);

    public static string GetHashedString(HashType type, byte[] source, byte[]? key, bool isLower = false)
    {
        Guard.NotNull(source, nameof(source));
        if (source.Length == 0)
        {
            return string.Empty;
        }
        var hashedBytes = GetHashedBytes(type, source, key);
        return hashedBytes.ToHexString(isLower);
    }

    public static string GetHashedString(HashAlgorithmName hashAlgorithm, byte[] source, byte[]? key, bool isLower = false)
    {
        Guard.NotNull(source, nameof(source));
        if (source.Length == 0) return string.Empty;

#if NET9_0_OR_GREATER
        return key is { Length: > 0 } 
            ? GetHashedString(hashAlgorithm, new ReadOnlySpan<byte>(key), new ReadOnlySpan<byte>(source), isLower) 
            : GetHashedString(hashAlgorithm, new ReadOnlySpan<byte>(source), isLower);
#else
        var hashedBytes = GetHashedBytes(hashAlgorithm, source, key);
        return hashedBytes.ToHexString(isLower);
#endif
    }

    public static byte[] GetHashedBytes(HashType type, string str) => GetHashedBytes(type, str, Encoding.UTF8);

    public static byte[] GetHashedBytes(HashType type, string str, Encoding encoding)
    {
        Guard.NotNull(str, nameof(str));
        if (str == string.Empty)
        {
            return [];
        }
        var bytes = encoding.GetBytes(str);
        return GetHashedBytes(type, bytes);
    }

    public static byte[] GetHashedBytes(HashType type, byte[] bytes) => GetHashedBytes(type, bytes, null);

    public static byte[] GetHashedBytes(HashType type, byte[] bytes, byte[]? key)
    {
        Guard.NotNull(bytes, nameof(bytes));
        if (bytes.Length == 0)
        {
            return bytes;
        }

        HashAlgorithm? algorithm = null;
        try
        {
            if (key == null)
            {
                algorithm = type switch
                {
                    HashType.SHA1 => SHA1.Create(),
                    HashType.SHA256 => SHA256.Create(),
                    HashType.SHA384 => SHA384.Create(),
                    HashType.SHA512 => SHA512.Create(),
                    _ => MD5.Create()
                };
            }
            else
            {
                algorithm = type switch
                {
                    HashType.SHA1 => new HMACSHA1(key),
                    HashType.SHA256 => new HMACSHA256(key),
                    HashType.SHA384 => new HMACSHA384(key),
                    HashType.SHA512 => new HMACSHA512(key),
                    _ => new HMACMD5(key)
                };
            }
            return algorithm.ComputeHash(bytes);
        }
        finally
        {
            algorithm?.Dispose();
        }
    }

    public static byte[] GetHashedBytes(HashAlgorithmName hashAlgorithm, byte[] bytes, byte[]? key)
    {
        Guard.NotNull(bytes, nameof(bytes));
        if (Enum.TryParse(hashAlgorithm.Name, true, out HashType hashType))
            return GetHashedBytes(hashType, bytes, key);

        throw new ArgumentOutOfRangeException(nameof(hashAlgorithm), @$"Unsupported hash algorithm {hashAlgorithm.Name}");
    }

#if NET9_0_OR_GREATER
    public static string GetHashedString(HashAlgorithmName hashAlgorithm, ReadOnlySpan<byte> bytes, bool isLower = false)
    {
        var hashedBytes = CryptographicOperations.HashData(hashAlgorithm, bytes);
        return hashedBytes.ToHexString(isLower);
    }
    
    public static string GetHashedString(HashAlgorithmName hashAlgorithm, ReadOnlySpan<byte> keys, ReadOnlySpan<byte> bytes, bool isLower = false)
    {
        var hashedBytes = CryptographicOperations.HmacData(hashAlgorithm, keys, bytes);
        return hashedBytes.ToHexString(isLower);
    }
#endif
}

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
