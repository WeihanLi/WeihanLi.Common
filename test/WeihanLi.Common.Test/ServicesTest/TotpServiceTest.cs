// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using WeihanLi.Common.Otp;
using WeihanLi.Common.Services;
using Xunit;

namespace WeihanLi.Common.Test.ServicesTest;

public class TotpServiceTest
{
    [Fact]
    public void BasicTest()
    {
        const string bizToken = "Test1234";
        var totpService = new TotpService(new TotpOptions()
        {
            ExpiresIn = 30
        });
        var code = totpService.GetCode(bizToken);
        Assert.NotEmpty(code);
        Assert.True(totpService.VerifyCode(bizToken, code));
        Thread.Sleep(35 * 1000);
        Assert.False(totpService.VerifyCode(bizToken, code));
    }

    [Fact]
    public void GetCodeWithTtlTest()
    {
        const string bizToken = "Test1234";
        var totpService = new TotpService(new TotpOptions());
        var (Code, Ttl) = totpService.GetCodeWithTtl(bizToken);
        Assert.NotEmpty(Code);
        Assert.True(Ttl >= 1);
    }
}
