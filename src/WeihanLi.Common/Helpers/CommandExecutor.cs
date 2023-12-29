// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using System.Diagnostics;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Helpers;

public static class CommandExecutor
{
    /// <summary>
    /// Execute command
    /// </summary>
    /// <param name="command">command with arguments</param>
    /// <param name="workingDirectory">working directory for the command</param>
    /// <param name="configure">configure the ProcessStartInfo</param>
    /// <returns>exit code</returns>
    public static int ExecuteCommand(string command, string? workingDirectory = null, Action<ProcessStartInfo>? configure = null)
    {
        Guard.NotNullOrEmpty(command);
        var cmd = command.Split(new[] { ' ' }, 2);
        return Execute(cmd[0], cmd.Length > 1 ? cmd[1] : null, workingDirectory, configure);
    }

    /// <summary>
    /// Execute command async
    /// </summary>
    /// <param name="command">command with arguments</param>
    /// <param name="workingDirectory">working directory for the command</param>
    /// <param name="configure">configure the ProcessStartInfo</param>
    /// <param name="cancellationToken">cancellationToken</param>
    /// <returns>exit code</returns>
    public static Task<int> ExecuteCommandAsync(string command, string? workingDirectory = null, Action<ProcessStartInfo>? configure = null, CancellationToken cancellationToken = default)
    {
        Guard.NotNullOrEmpty(command);
        var cmd = command.Split(new[] { ' ' }, 2);
        return ExecuteAsync(cmd[0], cmd.Length > 1 ? cmd[1] : null, workingDirectory, configure, cancellationToken);
    }

    /// <summary>
    /// Execute command with a process
    /// </summary>
    /// <param name="commandPath">executable command path</param>
    /// <param name="arguments">command arguments</param>
    /// <param name="workingDirectory">working directory</param>
    /// <param name="configure">configure ProcessStartInfo</param>
    /// <returns>exit code</returns>
    public static int Execute(string commandPath, string? arguments = null, string? workingDirectory = null, Action<ProcessStartInfo>? configure = null)
    {
        var processStartInfo = new ProcessStartInfo(commandPath, arguments ?? string.Empty)
        {
            UseShellExecute = false,
            CreateNoWindow = true,
            WorkingDirectory = workingDirectory ?? Environment.CurrentDirectory
        };
        configure?.Invoke(processStartInfo);
        return processStartInfo.Execute();
    }

    /// <summary>
    /// Execute command with a process
    /// </summary>
    /// <param name="commandPath">executable command path</param>
    /// <param name="arguments">command arguments</param>
    /// <param name="workingDirectory">working directory</param>
    /// <param name="configure">configure the ProcessStartInfo</param>
    /// <param name="cancellationToken">cancellationToken</param>
    /// <returns>exit code</returns>
    public static async Task<int> ExecuteAsync(string commandPath, string? arguments = null, string? workingDirectory = null, Action<ProcessStartInfo>? configure = null, CancellationToken cancellationToken = default)
    {
        var processStartInfo = new ProcessStartInfo(commandPath, arguments ?? string.Empty)
        {
            UseShellExecute = false,
            CreateNoWindow = true,
            WorkingDirectory = workingDirectory ?? Environment.CurrentDirectory
        };
        configure?.Invoke(processStartInfo);
        return await processStartInfo.ExecuteAsync(cancellationToken);
    }

    /// <summary>
    /// Execute command with a process and capture the output
    /// </summary>
    /// <param name="commandPath">executable command path</param>
    /// <param name="arguments">command arguments</param>
    /// <param name="workingDirectory">working directory</param>
    /// <param name="configure">configure the ProcessStartInfo</param>
    /// <returns>command execute result</returns>
    public static CommandResult ExecuteAndCapture(string commandPath, string? arguments = null, string? workingDirectory = null, Action<ProcessStartInfo>? configure = null)
    {
        var processStartInfo = new ProcessStartInfo(commandPath, arguments ?? string.Empty)
        {
            UseShellExecute = false,
            CreateNoWindow = true,
            WorkingDirectory = workingDirectory ?? Environment.CurrentDirectory
        };
        configure?.Invoke(processStartInfo);
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
    /// <param name="configure">configure the ProcessStartInfo</param>
    /// <param name="cancellationToken">cancellationToken</param>
    /// <returns>command execute result</returns>
    public static Task<CommandResult> ExecuteAndCaptureAsync(string commandPath, string? arguments = null, string? workingDirectory = null, Action<ProcessStartInfo>? configure = null, CancellationToken cancellationToken = default)
    {
        var processStartInfo = new ProcessStartInfo(commandPath, arguments ?? string.Empty)
        {
            UseShellExecute = false,
            CreateNoWindow = true,
            WorkingDirectory = workingDirectory ?? Environment.CurrentDirectory
        };
        configure?.Invoke(processStartInfo);
        return ExecuteAndCaptureAsync(processStartInfo, cancellationToken);
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

    [Obsolete("Please use EnsureSuccessExitCode() instead")]
    public CommandResult EnsureSuccessfulExitCode(int successCode = 0) => EnsureSuccessExitCode(successCode);
    
    public CommandResult EnsureSuccessExitCode(int successCode = 0)
    {
        if (ExitCode != successCode)
        {
            throw new InvalidOperationException($"Unexpected exit code:{ExitCode} {StandardError}");
        }
        return this;
    }
}
