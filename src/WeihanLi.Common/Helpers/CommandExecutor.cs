// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using System.Diagnostics;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Helpers;

public static class CommandExecutor
{
    private static readonly char[] SpaceSeparator = [' '];
    
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
        var cmd = command.Split(SpaceSeparator, 2);
        return Execute(cmd[0], cmd.Length > 1 ? cmd[1] : null, workingDirectory, configure);
    }
    
    /// <summary>
    /// Execute command and capture output
    /// </summary>
    /// <param name="command">command with arguments</param>
    /// <param name="workingDirectory">working directory for the command</param>
    /// <param name="configure">configure the ProcessStartInfo</param>
    /// <returns>command execute result</returns>
    public static CommandResult ExecuteCommandAndCapture(string command, string? workingDirectory = null, Action<ProcessStartInfo>? configure = null)
    {
        Guard.NotNullOrEmpty(command);
        var cmd = command.Split(SpaceSeparator, 2);
        return ExecuteAndCapture(cmd[0], cmd.Length > 1 ? cmd[1] : null, workingDirectory, configure);
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
        var cmd = command.Split(SpaceSeparator, 2);
        return ExecuteAsync(cmd[0], cmd.Length > 1 ? cmd[1] : null, workingDirectory, configure, cancellationToken);
    }
    
    /// <summary>
    /// Execute command and capture output async
    /// </summary>
    /// <param name="command">command with arguments</param>
    /// <param name="workingDirectory">working directory for the command</param>
    /// <param name="configure">configure the ProcessStartInfo</param>
    /// <param name="cancellationToken">cancellationToken</param>
    /// <returns>command execute result</returns>
    public static Task<CommandResult> ExecuteCommandAndCaptureAsync(string command, string? workingDirectory = null, Action<ProcessStartInfo>? configure = null, CancellationToken cancellationToken = default)
    {
        Guard.NotNullOrEmpty(command);
        var cmd = command.Split(SpaceSeparator, 2);
        return ExecuteAndCaptureAsync(cmd[0], cmd.Length > 1 ? cmd[1] : null, workingDirectory, configure, cancellationToken);
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
    /// Execute command with a process async
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
    /// Execute command with a process
    /// </summary>
    /// <param name="commandPath">executable command path</param>
    /// <param name="arguments">command arguments</param>
    /// <param name="workingDirectory">working directory</param>
    /// <param name="stdout">stdout writer, write to console by default</param>
    /// <param name="stderr">stderr writer, write to console by default</param>
    /// <param name="configure">configure the ProcessStartInfo</param>
    /// <returns>exit code</returns>
    public static int ExecuteAndOutput(string commandPath, string? arguments = null, string? workingDirectory = null, 
        TextWriter? stdout = null, TextWriter? stderr = null, Action<ProcessStartInfo>? configure = null)
    {
        var processStartInfo = new ProcessStartInfo(commandPath, arguments ?? string.Empty)
        {
            UseShellExecute = false,
            CreateNoWindow = true,
            WorkingDirectory = workingDirectory ?? Environment.CurrentDirectory
        };
        configure?.Invoke(processStartInfo);
        return processStartInfo.GetExitCode(stdout ?? Console.Out, stderr ?? Console.Error);
    }
    
    /// <summary>
    /// Execute command with a process
    /// </summary>
    /// <param name="commandPath">executable command path</param>
    /// <param name="arguments">command arguments</param>
    /// <param name="workingDirectory">working directory</param>
    /// <param name="stdout">stdout writer, write to console by default</param>
    /// <param name="stderr">stderr writer, write to console by default</param>
    /// <param name="configure">configure the ProcessStartInfo</param>
    /// <param name="cancellationToken">cancellationToken</param>
    /// <returns>exit code</returns>
    public static async Task<int> ExecuteAndOutputAsync(string commandPath, string? arguments = null, string? workingDirectory = null,
        TextWriter? stdout = null, TextWriter? stderr = null, Action<ProcessStartInfo>? configure = null, CancellationToken cancellationToken = default)
    {
        var processStartInfo = new ProcessStartInfo(commandPath, arguments ?? string.Empty)
        {
            UseShellExecute = false,
            CreateNoWindow = true,
            WorkingDirectory = workingDirectory ?? Environment.CurrentDirectory
        };
        configure?.Invoke(processStartInfo);
        return await processStartInfo.GetExitCodeAsync(stdout ?? Console.Out, stderr ?? Console.Error,cancellationToken);
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

public sealed class CommandResult(int exitCode, string standardOut, string standardError)
{
    public string StandardOut { get; } = standardOut;
    public string StandardError { get; } = standardError;
    public int ExitCode { get; } = exitCode;

    [Obsolete("Please use EnsureSuccessExitCode() instead", true)]
    public CommandResult EnsureSuccessfulExitCode(int successCode = 0) => EnsureSuccessExitCode(successCode);
    
    public CommandResult EnsureSuccessExitCode(int successCode = 0)
    {
        if (ExitCode != successCode)
        {
            throw new InvalidOperationException($"Unexpected exit code:{ExitCode}");
        }
        return this;
    }
}
