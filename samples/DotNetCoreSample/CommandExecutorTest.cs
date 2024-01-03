// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the MIT license.

using static WeihanLi.Common.Helpers.CommandExecutor;

namespace DotNetCoreSample;

internal class CommandExecutorTest
{
    public static void MainTest()
    {
        var result = ExecuteAndCapture("hostname");
        result.EnsureSuccessExitCode();
        Console.WriteLine(result.StandardOut);
    }
}
