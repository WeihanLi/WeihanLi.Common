using System.Text;
using WeihanLi.Common.Otp;
using WeihanLi.Common.Services;

namespace WeihanLi.Common.Helpers;

public static class TotpHelper
{
    private static readonly Lazy<ITotpService> _totp = new(() => new TotpService(_defaultOptions));

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
    public static string GetCode(byte[] securityToken)
    {
        return _totp.Value.GetCode(securityToken);
    }

    /// <summary>
    /// Generates code for the specified <paramref name="securityToken"/>.
    /// </summary>
    /// <param name="securityToken">The security token to generate code.</param>
    /// <returns>The generated code.</returns>
    public static string GetCode(string securityToken) => GetCode(Encoding.UTF8.GetBytes(securityToken));

    /// <summary>
    /// Validates the code for the specified <paramref name="securityToken"/>.
    /// </summary>
    /// <param name="securityToken">The security token for verifying.</param>
    /// <param name="code">The code to validate.</param>
    /// <returns><c>True</c> if validate succeed, otherwise, <c>false</c>.</returns>
    public static bool VerifyCode(byte[] securityToken, string code)
    {
        Guard.NotNull(securityToken, nameof(securityToken));
        return _totp.Value.VerifyCode(securityToken, code);
    }

    /// <summary>
    /// Validates the code for the specified <paramref name="securityToken"/>.
    /// </summary>
    /// <param name="securityToken">The security token for verifying</param>
    /// <param name="code">The code to validate.</param>
    /// <returns><c>True</c> if validate succeed, otherwise, <c>false</c>.</returns>
    public static bool VerifyCode(string securityToken, string code) => VerifyCode(Encoding.UTF8.GetBytes(securityToken), code);
}
