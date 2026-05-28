// ReSharper disable InconsistentNaming
namespace WeihanLi.Common.Otp;

/// <summary>
/// Specifies the HMAC hash algorithm used for one-time password generation.
/// </summary>
public enum OtpHashAlgorithm
{
    /// <summary>
    /// SHA-1 is used as the HMAC hashing algorithm.
    /// </summary>
    SHA1 = 0,

    /// <summary>
    /// SHA-256 is used as the HMAC hashing algorithm.
    /// </summary>
    SHA256 = 1,

    /// <summary>
    /// SHA-512 is used as the HMAC hashing algorithm.
    /// </summary>
    SHA512 = 2,
}
