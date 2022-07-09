﻿// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using System.Diagnostics;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Helpers;

public static class CommandExecutor
{
    /// <summary>
    /// Execute command with a process
    /// </summary>
    /// <param name="commandPath">executable command path</param>
    /// <param name="arguments">command arguments</param>
    /// <param name="workingDirectory">working directory</param>
    /// <returns>exit code</returns>
    public static int Execute(string commandPath, string? arguments = null, string? workingDirectory = null)
    {
        return new ProcessStartInfo(commandPath, arguments ?? string.Empty)
        {
            UseShellExecute = false,
            CreateNoWindow = true,

            WorkingDirectory = workingDirectory ?? Environment.CurrentDirectory
        }.Execute();
    }

    /// <summary>
    /// Execute command with a process
    /// </summary>
    /// <param name="commandPath">executable command path</param>
    /// <param name="arguments">command arguments</param>
    /// <param name="workingDirectory">working directory</param>
    /// <returns>exit code</returns>
    public static async Task<int> ExecuteAsync(string commandPath, string? arguments = null, string? workingDirectory = null)
    {
        return await new ProcessStartInfo(commandPath, arguments ?? string.Empty)
        {
            UseShellExecute = false,
            CreateNoWindow = true,

            WorkingDirectory = workingDirectory ?? Environment.CurrentDirectory
        }.ExecuteAsync();
    }

    /// <summary>
    /// Execute command with a process and capture the output
    /// </summary>
    /// <param name="commandPath">executable command path</param>
    /// <param name="arguments">command arguments</param>
    /// <param name="workingDirectory">working directory</param>
    /// <returns>command execute result</returns>
    public static CommandResult ExecuteAndCapture(string commandPath, string? arguments = null, string? workingDirectory = null)
    {
        var processStartInfo = new ProcessStartInfo(commandPath, arguments ?? string.Empty)
        {
            UseShellExecute = false,
            CreateNoWindow = true,
            WorkingDirectory = workingDirectory ?? Environment.CurrentDirectory
        };
        return ExecuteAndCapture(processStartInfo);
    }

    /// <summary>
    /// Execute command with a process and capture the output
    /// </summary>
    /// <param name="processStartInfo">processStartInfo</param>
    /// <returns>command execute result</returns>
    public static CommandResult ExecuteAndCapture(this ProcessStartInfo processStartInfo)
    {
        return processStartInfo.GetResult();
    }

    /// <summary>
    /// Execute command with a process and capture the output
    /// </summary>
    /// <param name="commandPath">executable command path</param>
    /// <param name="arguments">command arguments</param>
    /// <param name="workingDirectory">working directory</param>
    /// <returns>command execute result</returns>
    public static Task<CommandResult> ExecuteAndCaptureAsync(string commandPath, string? arguments = null, string? workingDirectory = null)
    {
        var processStartInfo = new ProcessStartInfo(commandPath, arguments ?? string.Empty)
        {
            UseShellExecute = false,
            CreateNoWindow = true,
            WorkingDirectory = workingDirectory ?? Environment.CurrentDirectory
        };
        return ExecuteAndCaptureAsync(processStartInfo);
    }

    /// <summary>
    /// Execute command with a process and capture the output
    /// </summary>
    /// <param name="processStartInfo">processStartInfo</param>
    /// <param name="cancellationToken"></param>
    /// <returns>command execute result</returns>
    public static async Task<CommandResult> ExecuteAndCaptureAsync(this ProcessStartInfo processStartInfo, CancellationToken cancellationToken = default)
    {
        return await processStartInfo.GetResultAsync(cancellationToken);
    }
}

public sealed class CommandResult
{
    public CommandResult(int exitCode, string standardOut, string standardError)
    {
        ExitCode = exitCode;
        StandardOut = standardOut;
        StandardError = standardError;
    }

    public string StandardOut { get; }
    public string StandardError { get; }
    public int ExitCode { get; }

    public CommandResult EnsureSuccessfulExitCode(int successCode = 0)
    {
        if (ExitCode != successCode)
        {
            throw new InvalidOperationException($"Unexpected exit code:{ExitCode} {StandardError}");
        }
        return this;
    }
}
