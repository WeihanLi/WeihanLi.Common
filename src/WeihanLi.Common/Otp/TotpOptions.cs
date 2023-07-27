using WeihanLi.Common.Helpers;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Otp;

public class TotpOptions
{
    /// <summary>
    /// 计算 code 的算法
    /// The algorithm for calculating the totp code
    /// </summary>
    public OtpHashAlgorithm Algorithm { get; set; } = OtpHashAlgorithm.SHA1;

    /// <summary>
    /// 生成的 code 长度
    /// The expected code length
    /// </summary>
    public int Size { get; set; } = 6;

    /// <summary>
    /// 过期时间，单位是秒
    /// The code expire time, 300s by default
    /// </summary>
    public int ExpiresIn { get; set; } = 300;

    private string? _salt;

    /// <summary>
    /// Salt for security consideration
    /// </summary>
    public string? Salt
    {
        get => _salt;
        set
        {
            _salt = value;
            SaltBytes = value.IsNullOrEmpty() ? null : Base32EncodeHelper.GetBytes(value);
        }
    }

    internal byte[]? SaltBytes { get; private set; }
}
