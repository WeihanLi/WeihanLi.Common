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
        var totp = new Totp(OtpHashAlgorithm.SHA1, 6);
        while (true)
        {
            var code = totp.Compute(Base32EncodeHelper.GetBytes(secret));
            Console.WriteLine(code);
            ConsoleHelper.ReadLineWithPrompt();
        }
    }
}
