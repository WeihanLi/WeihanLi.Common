// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using Microsoft.Extensions.Options;
using WeihanLi.Common.Otp;

namespace WeihanLi.Common.Services;

public interface ITotpServiceFactory
{
    ITotpService GetService(string? name = null);
}

public sealed class TotpServiceFactory(IOptionsMonitor<TotpOptions> optionsMonitor) : ITotpServiceFactory
{
    private readonly IOptionsMonitor<TotpOptions> _optionsMonitor = optionsMonitor;

    public ITotpService GetService(string? name = null)
    {
        return new TotpService(_optionsMonitor.Get(name ?? Options.DefaultName));
    }
}
