using System.Text;
using WeihanLi.Common.Otp;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Helpers;

public static class TotpHelper
{
    private static readonly Lazy<Totp> _totp = new(() => new Totp(_defaultOptions.Algorithm, _defaultOptions.Size));

    private static readonly TotpOptions _defaultOptions = new();

    /// <summary>
    /// Configure the default <see cref="TotpOptions"/> for <see cref="TotpHelper"/>
    /// </summary>
    /// <param name="configAction">configure</param>
    /// <returns>configured options</returns>
    public static TotpOptions ConfigureTotpOptions(Action<TotpOptions> configAction)
    {
        Guard.NotNull(configAction, nameof(configAction));
        configAction.Invoke(_defaultOptions);
        return _defaultOptions;
    }

    /// <summary>
    /// Generates code for the specified <paramref name="securityToken"/>.
    /// </summary>
    /// <param name="securityToken">The security token to generate code.</param>
    /// <returns>The generated code.</returns>
    public static string GenerateCode(byte[] securityToken)
    {
        Guard.NotNull(securityToken, nameof(securityToken));

        if (_defaultOptions.SaltBytes == null)
        {
            return _totp.Value.Compute(securityToken);
        }
        var bytes = new byte[securityToken.Length + _defaultOptions.SaltBytes.Length];
        Array.Copy(securityToken, bytes, securityToken.Length);
        Array.Copy(_defaultOptions.SaltBytes, 0, bytes, securityToken.Length, _defaultOptions.SaltBytes.Length);

        return _totp.Value.Compute(bytes);
    }

    /// <summary>
    /// Generates code for the specified <paramref name="securityToken"/>.
    /// </summary>
    /// <param name="securityToken">The security token to generate code.</param>
    /// <returns>The generated code.</returns>
    public static string GenerateCode(string securityToken) => GenerateCode(Encoding.UTF8.GetBytes(securityToken));

    /// <summary>
    /// ttl of the code for the specified <paramref name="securityToken"/>.
    /// </summary>
    /// <param name="securityToken">The security token to generate code.</param>
    /// <returns>the code remaining seconds expires in</returns>
    public static int TTL(byte[] securityToken)
    {
        Guard.NotNull(securityToken, nameof(securityToken));

        return _totp.Value.RemainingSeconds();
    }

    /// <summary>
    /// ttl of the code for the specified <paramref name="securityToken"/>.
    /// </summary>
    /// <param name="securityToken">The security token to generate code.</param>
    public static int TTL(string securityToken) => TTL(Encoding.UTF8.GetBytes(securityToken));

    /// <summary>
    /// Validates the code for the specified <paramref name="securityToken"/>.
    /// </summary>
    /// <param name="securityToken">The security token for verifying.</param>
    /// <param name="code">The code to validate.</param>
    /// <param name="expiresIn">expiresIn, in seconds</param>
    /// <returns><c>True</c> if validate succeed, otherwise, <c>false</c>.</returns>
    public static bool VerifyCode(byte[] securityToken, string code, int expiresIn = -1)
    {
        Guard.NotNull(securityToken, nameof(securityToken));
        if (string.IsNullOrWhiteSpace(code))
        {
            return false;
        }

        if (_defaultOptions.SaltBytes.IsNullOrEmpty())
        {
            return _totp.Value.Verify(securityToken, code, TimeSpan.FromSeconds(expiresIn >= 0 ? expiresIn : _defaultOptions.ExpiresIn));
        }

        var saltBytes = _defaultOptions.SaltBytes;
        var bytes = new byte[securityToken.Length + saltBytes.Length];
        Array.Copy(securityToken, bytes, securityToken.Length);
        Array.Copy(saltBytes, 0, bytes, securityToken.Length, saltBytes.Length);

        return _totp.Value.Verify(bytes, code, TimeSpan.FromSeconds(expiresIn >= 0 ? expiresIn : _defaultOptions.ExpiresIn));
    }

    /// <summary>
    /// Validates the code for the specified <paramref name="securityToken"/>.
    /// </summary>
    /// <param name="securityToken">The security token for verifying.</param>
    /// <param name="code">The code to validate.</param>
    /// <param name="expiresIn">expiresIn, in seconds</param>
    /// <returns><c>True</c> if validate succeed, otherwise, <c>false</c>.</returns>
    public static bool VerifyCode(string securityToken, string code, int expiresIn = -1) => VerifyCode(Encoding.UTF8.GetBytes(securityToken), code, expiresIn);
}
