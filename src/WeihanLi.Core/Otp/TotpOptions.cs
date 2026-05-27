using System.Text;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Otp;

/// <summary>
/// Options for time-based one-time password generation and validation.
/// </summary>
public sealed class TotpOptions
{
    /// <summary>
    /// Gets or sets the hash algorithm used to calculate the TOTP code.
    /// </summary>
    public OtpHashAlgorithm Algorithm { get; set; } = OtpHashAlgorithm.SHA1;

    /// <summary>
    /// Gets or sets the generated code length. Allowed values are 4 through 9.
    /// </summary>
    public int Size
    {
        get => _size;
        set
        {
            if (value is > 9 or < 4)
                throw new ArgumentOutOfRangeException(nameof(value), value, @"Size out of range, allowed range 4~9");
            _size = value;
        }
    }

    /// <summary>
    /// Gets or sets the code expiration time in seconds. The default value is 300.
    /// </summary>
    public int ExpiresIn { get; set; } = 300;

    private string? _salt;
    private int _size = 6;

    /// <summary>
    /// Gets or sets the salt appended to the security token before code generation.
    /// </summary>
    public string? Salt
    {
        get => _salt;
        set
        {
            _salt = value;
            SaltBytes = value.IsNullOrEmpty() ? null : Encoding.UTF8.GetBytes(value);
        }
    }

    internal byte[]? SaltBytes { get; private set; }
}
