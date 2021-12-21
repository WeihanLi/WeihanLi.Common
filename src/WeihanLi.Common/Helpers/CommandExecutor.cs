using System.Diagnostics;

namespace WeihanLi.Common.Helpers;

public static class CommandExecutor
{
    private static bool TryKill(this Process process)
    {
        try
        {
            process.Kill();
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    /// <summary>
    /// Execute command with a process
    /// </summary>
    /// <param name="commandPath">executable command path</param>
    /// <param name="arguments">command arguments</param>
    /// <param name="workingDirectory">working directory</param>
    /// <returns>exit code</returns>
    public static int Execute(string commandPath, string? arguments = null, string? workingDirectory = null)
    {
        return Execute(new ProcessStartInfo(commandPath, arguments ?? string.Empty)
        {
            UseShellExecute = false,
            CreateNoWindow = true,

            WorkingDirectory = workingDirectory ?? Environment.CurrentDirectory
        });
    }

    /// <summary>
    /// Execute command with a process
    /// </summary>
    /// <param name="processStartInfo">processStartInfo</param>
    /// <returns>process exit code</returns>
    public static int Execute(this ProcessStartInfo processStartInfo)
    {
        using var process = new Process()
        {
            StartInfo = processStartInfo
        };
        process.Start();
        process.WaitForExit();
        return process.ExitCode;
    }


    /// <summary>
    /// Execute command with a process
    /// </summary>
    /// <param name="processStartInfo">processStartInfo</param>
    /// <param name="cancellationToken">cancellationToken</param>
    /// <returns>process exit code</returns>
    public static async Task<int> ExecuteAsync(this ProcessStartInfo processStartInfo, CancellationToken cancellationToken = default)
    {
        using var process = new Process()
        {
            StartInfo = processStartInfo
        };
        
        var tcs = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);

        process.EnableRaisingEvents = true;
        process.Exited += (s, e) => tcs.TrySetResult(0);

        process.Start();

        using (cancellationToken.Register(
                   () =>
                   {
                       if (process.TryKill())
                       {
                           _ = tcs.TrySetCanceled(cancellationToken);
                       }
                   }, false))
        {
            _ = await tcs.Task.ConfigureAwait(false);
        }
        
        return process.ExitCode;
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
        // ensure output redirect
        processStartInfo.RedirectStandardOutput = true;
        processStartInfo.RedirectStandardError = true;
        // create a new process to execute command
        using var process = new Process()
        {
            StartInfo = processStartInfo
        };
        process.Start();
        
        var standardOut = process.StandardOutput.ReadToEnd();
        var standardError = process.StandardError.ReadToEnd();
        
        process.WaitForExit();
        return new CommandResult(process.ExitCode, standardOut, standardError);
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
        // ensure output redirect
        processStartInfo.RedirectStandardOutput = true;
        processStartInfo.RedirectStandardError = true;
        // create a new process to execute command
        using var process = new Process()
        {
            StartInfo = processStartInfo
        };
        var tcs = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
        
        process.EnableRaisingEvents = true;
        process.Exited += (s, e) => tcs.TrySetResult(0);
        process.Start();
        
        var standardOutTask = process.StandardOutput.ReadToEndAsync();
        var standardErrorTask = process.StandardError.ReadToEndAsync();
        
        using (cancellationToken.Register(
                   () =>
                   {
                       if (process.TryKill())
                       {
                           _ = tcs.TrySetCanceled(cancellationToken);
                       }
                   }, false))
        {
            await Task.WhenAll(standardOutTask, standardErrorTask, tcs.Task).ConfigureAwait(false);
            return new CommandResult(process.ExitCode, standardOutTask.Result, standardErrorTask.Result);
        }
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
