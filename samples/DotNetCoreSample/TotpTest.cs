// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using WeihanLi.Common.Helpers;
using WeihanLi.Common.Otp;

namespace DotNetCoreSample;

public class TotpTest
{
    public static void MainTest()
    {
        var secret = "xx";
        var totp = new Totp();
        while (true)
        {
            var (Code, Ttl) = totp.ComputeWithTtl(Base32EncodeHelper.GetBytes(secret));
            Console.WriteLine(@$"{Code} {Ttl}");
            ConsoleHelper.ReadLineWithPrompt();
        }
    }
}
