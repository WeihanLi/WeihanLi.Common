// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the MIT license.

using WeihanLi.Common.Helpers;
using static WeihanLi.Common.Helpers.CommandExecutor;

namespace DotNetCoreSample;

internal static class CommandExecutorTest
{
    public static void MainTest()
    {
        ExecuteAndCapture("hostname")
            .PrintOutputToConsole()
            .EnsureSuccessExitCode();
        
        ExecuteAndOutput("hostname").EnsureSuccessExitCode();

        ExecuteAndOutputAsync("hostname").Wait();
        
        ExecuteCommandAndOutput("hostname").EnsureSuccessExitCode();
    }
}
