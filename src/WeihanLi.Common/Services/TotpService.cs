// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using Microsoft.Extensions.Options;
using System.Text;
using WeihanLi.Common.Otp;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Services;

/// <summary>
/// Time-based one-time password service
/// </summary>
public interface ITotpService
{
    string GetCode(byte[] securityToken);

    (string Code, int Ttl) GetCodeWithTtl(byte[] securityToken);
    bool VerifyCode(byte[] securityToken, string code);
}

public static class TotpServiceExtensions
{
    public static string GetCode(this ITotpService totpService, string securityToken, Encoding? encoding = null)
    {
        Guard.NotNull(totpService);
        Guard.NotNullOrEmpty(securityToken);
        return totpService.GetCode(securityToken.GetBytes(encoding));
    }
    
    public static (string Code, int Ttl) GetCodeWithTtl(this ITotpService totpService, string securityToken, Encoding? encoding = null)
    {
        Guard.NotNull(totpService);
        Guard.NotNullOrEmpty(securityToken);
        return totpService.GetCodeWithTtl(securityToken.GetBytes(encoding));
    }
    
    public static bool VerifyCode(this ITotpService totpService, string securityToken, string code, Encoding? encoding = null)
    {
        Guard.NotNull(totpService);
        Guard.NotNullOrEmpty(securityToken);
        return totpService.VerifyCode(securityToken.GetBytes(encoding), code);
    }
}

public interface ITotpServiceFactory
{
    ITotpService GetService(string? name = null);
}

public sealed class TotpServiceFactory: ITotpServiceFactory
{
    private readonly IOptionsMonitor<TotpOptions> _optionsMonitor;

    public TotpServiceFactory(IOptionsMonitor<TotpOptions> optionsMonitor)
    {
        _optionsMonitor = optionsMonitor;
    }
    
    public ITotpService GetService(string? name = null)
    {
        return new TotpService(_optionsMonitor.Get(name ?? Options.DefaultName));
    }
}

public sealed class TotpService: ITotpService
{
    private readonly TotpOptions _totpOptions;
    private readonly TimeSpan _expiry;
    private readonly Totp _totp;

    public TotpService(TotpOptions totpOptions)
    {
        _totpOptions = totpOptions;
        _expiry = TimeSpan.FromSeconds(totpOptions.ExpiresIn);
        _totp = new Totp(_totpOptions.Algorithm, _totpOptions.Size);
    }
    
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
