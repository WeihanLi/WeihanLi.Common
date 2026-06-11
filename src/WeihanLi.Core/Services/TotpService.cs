// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using System.Text;
using WeihanLi.Common.Otp;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Services;

/// <summary>
/// Time-based one-time password service
/// </summary>
public interface ITotpService
{
    /// <summary>
    /// Generates a time-based one-time password for the specified security token.
    /// </summary>
    /// <param name="securityToken">The security token bytes.</param>
    /// <returns>The generated one-time password.</returns>
    string GetCode(byte[] securityToken);

    /// <summary>
    /// Generates a time-based one-time password and returns its remaining time to live.
    /// </summary>
    /// <param name="securityToken">The security token bytes.</param>
    /// <returns>The generated one-time password and its remaining lifetime in seconds.</returns>
    (string Code, int Ttl) GetCodeWithTtl(byte[] securityToken);

    /// <summary>
    /// Verifies a time-based one-time password for the specified security token.
    /// </summary>
    /// <param name="securityToken">The security token bytes.</param>
    /// <param name="code">The one-time password to verify.</param>
    /// <returns><see langword="true"/> when the code is valid; otherwise, <see langword="false"/>.</returns>
    bool VerifyCode(byte[] securityToken, string code);
}

/// <summary>
/// Extension methods for <see cref="ITotpService"/>.
/// </summary>
public static class TotpServiceExtensions
{
    /// <summary>
    /// Generates a time-based one-time password from a text security token.
    /// </summary>
    /// <param name="totpService">The TOTP service.</param>
    /// <param name="securityToken">The text security token.</param>
    /// <param name="encoding">The encoding used to convert the token to bytes; UTF-8 is used when omitted.</param>
    /// <returns>The generated one-time password.</returns>
    public static string GetCode(this ITotpService totpService, string securityToken, Encoding? encoding = null)
    {
        Guard.NotNull(totpService);
        Guard.NotNullOrEmpty(securityToken);
        return totpService.GetCode(securityToken.GetBytes(encoding));
    }

    /// <summary>
    /// Generates a time-based one-time password from a text security token and returns its remaining time to live.
    /// </summary>
    /// <param name="totpService">The TOTP service.</param>
    /// <param name="securityToken">The text security token.</param>
    /// <param name="encoding">The encoding used to convert the token to bytes; UTF-8 is used when omitted.</param>
    /// <returns>The generated one-time password and its remaining lifetime in seconds.</returns>
    public static (string Code, int Ttl) GetCodeWithTtl(this ITotpService totpService, string securityToken, Encoding? encoding = null)
    {
        Guard.NotNull(totpService);
        Guard.NotNullOrEmpty(securityToken);
        return totpService.GetCodeWithTtl(securityToken.GetBytes(encoding));
    }

    /// <summary>
    /// Verifies a time-based one-time password for a text security token.
    /// </summary>
    /// <param name="totpService">The TOTP service.</param>
    /// <param name="securityToken">The text security token.</param>
    /// <param name="code">The one-time password to verify.</param>
    /// <param name="encoding">The encoding used to convert the token to bytes; UTF-8 is used when omitted.</param>
    /// <returns><see langword="true"/> when the code is valid; otherwise, <see langword="false"/>.</returns>
    public static bool VerifyCode(this ITotpService totpService, string securityToken, string code, Encoding? encoding = null)
    {
        Guard.NotNull(totpService);
        Guard.NotNullOrEmpty(securityToken);
        return totpService.VerifyCode(securityToken.GetBytes(encoding), code);
    }
}

/// <summary>
/// Default implementation of <see cref="ITotpService"/>.
/// </summary>
public sealed class TotpService : ITotpService
{
    private readonly TotpOptions _totpOptions;
    private readonly TimeSpan? _expiry;
    private readonly Totp _totp;

    /// <summary>
    /// Initializes a new instance of the <see cref="TotpService"/> class.
    /// </summary>
    /// <param name="totpOptions">The TOTP options.</param>
    public TotpService(TotpOptions totpOptions)
    {
        _totpOptions = Guard.NotNull(totpOptions);
        _expiry = totpOptions.ExpiresIn is >= Totp.TimeStepSeconds * 2 and <= Totp.MaxTimeStepSeconds
            ? TimeSpan.FromSeconds(totpOptions.ExpiresIn)
            : null;
        _totp = new Totp(_totpOptions.Algorithm, _totpOptions.Size);
    }

    /// <summary>
    /// Generates a time-based one-time password for the specified security token.
    /// </summary>
    /// <param name="securityToken">The security token bytes.</param>
    /// <returns>The generated one-time password.</returns>
    public string GetCode(byte[] securityToken)
    {
        Guard.NotNull(securityToken, nameof(securityToken));

        if (_totpOptions.SaltBytes.IsNullOrEmpty())
            return _totp.Compute(securityToken);

        var bytes = new byte[securityToken.Length + _totpOptions.SaltBytes.Length];
        Array.Copy(securityToken, bytes, securityToken.Length);
        Array.Copy(_totpOptions.SaltBytes, 0, bytes, securityToken.Length, _totpOptions.SaltBytes.Length);
        return _totp.Compute(bytes);
    }

    /// <summary>
    /// Generates a time-based one-time password and returns its remaining time to live.
    /// </summary>
    /// <param name="securityToken">The security token bytes.</param>
    /// <returns>The generated one-time password and its remaining lifetime in seconds.</returns>
    public (string Code, int Ttl) GetCodeWithTtl(byte[] securityToken)
    {
        Guard.NotNull(securityToken, nameof(securityToken));

        if (_totpOptions.SaltBytes.IsNullOrEmpty())
            return _totp.ComputeWithTtl(securityToken);

        var bytes = new byte[securityToken.Length + _totpOptions.SaltBytes.Length];
        Array.Copy(securityToken, bytes, securityToken.Length);
        Array.Copy(_totpOptions.SaltBytes, 0, bytes, securityToken.Length, _totpOptions.SaltBytes.Length);
        return _totp.ComputeWithTtl(bytes);
    }

    /// <summary>
    /// Verifies a time-based one-time password for the specified security token.
    /// </summary>
    /// <param name="securityToken">The security token bytes.</param>
    /// <param name="code">The one-time password to verify.</param>
    /// <returns><see langword="true"/> when the code is valid; otherwise, <see langword="false"/>.</returns>
    public bool VerifyCode(byte[] securityToken, string code)
    {
        if (string.IsNullOrEmpty(code) || code.Length != _totpOptions.Size) return false;
        Guard.NotNull(securityToken);

        if (_totpOptions.SaltBytes.IsNullOrEmpty())
            return _totp.Verify(securityToken, code, _expiry);

        var bytes = new byte[securityToken.Length + _totpOptions.SaltBytes.Length];
        Array.Copy(securityToken, bytes, securityToken.Length);
        Array.Copy(_totpOptions.SaltBytes, 0, bytes, securityToken.Length, _totpOptions.SaltBytes.Length);
        return _totp.Verify(bytes, code, _expiry);
    }
}
