// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using WeihanLi.Common;
using WeihanLi.Common.Helpers;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Extensions;

public static class ProcessExtension
{
    public static ProcessStartInfo WithEnv(this ProcessStartInfo processStartInfo, string name, string value)
    {
        Guard.NotNull(processStartInfo);
        processStartInfo.Environment[name] = value;
        return processStartInfo;
    }

#if NET6_0_OR_GREATER
#else
    public static Task WaitForExitAsync(this Process process, CancellationToken cancellationToken = default)
    {
        Guard.NotNull(process);
        process.EnableRaisingEvents = true;
        var tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        try
        {
            process.Exited += EventHandler;
            tcs.Task.Wait(cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            process.TryKill();
            tcs.TrySetCanceled();
        }
        catch (Exception ex)
        {
            tcs.SetException(ex);
        }
        finally
        {
            process.Exited -= EventHandler;
        }

        return tcs.Task;

        void EventHandler(object o, EventArgs eventArgs) => tcs.TrySetResult();
    }
#endif

    /// <summary>
    /// Execute command with a process
    /// </summary>
    /// <param name="processStartInfo">processStartInfo</param>
    /// <returns>process exit code</returns>
    public static int Execute(this ProcessStartInfo processStartInfo)
    {
        using var process = new Process();
        process.StartInfo = processStartInfo;
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
        using var process = new Process();
        process.StartInfo = processStartInfo;
        process.Start();
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, ApplicationHelper.ExitToken);
        cts.Token.Register((p) => ((Process)p!).TryKill(), process);
        await process.WaitForExitAsync(cts.Token);
        return process.ExitCode;
    }

    /// <summary>
    /// Get process execute result
    /// </summary>
    /// <param name="psi">ProcessStartInfo</param>
    /// <returns>process output and exitCode</returns>
    public static CommandResult GetResult(this ProcessStartInfo psi)
    {
        var stdOutStringBuilder = new StringBuilder();
        using var stdOut = new StringWriter(stdOutStringBuilder);
        var stdErrStringBuilder = new StringBuilder();
        using var stdErr = new StringWriter(stdErrStringBuilder);
        var exitCode = -1;
        int? processId = null;
        Action<Process> processStartAction = p => processId = p.Id;

        try
        {
            using var process = psi.ExecuteProcess(stdOut, stdErr, processStartAction);
            exitCode = process.ExitCode;
        }
        catch (Win32Exception win32Exception)
        {
            exitCode = win32Exception.ErrorCode;
        }
        return new(exitCode, stdOutStringBuilder.ToString(), stdErrStringBuilder.ToString())
        {
            ProcessId = processId
        };
    }

    /// <summary>
    /// Get process execute result
    /// </summary>
    /// <param name="psi">ProcessStartInfo</param>
    /// <param name="cancellationToken">cancellationToken</param>
    /// <returns>process output and exitCode</returns>
    public static async Task<CommandResult> GetResultAsync(this ProcessStartInfo psi, CancellationToken cancellationToken = default)
    {
        var stdOutStringBuilder = new StringBuilder();
#if NET
        await
#endif
        using var stdOut = new StringWriter(stdOutStringBuilder);
        var stdErrStringBuilder = new StringBuilder();
#if NET
        await
#endif
        using var stdErr = new StringWriter(stdErrStringBuilder);
        var exitCode = -1;
        int? processId = null;
        Action<Process> processStartAction = p => processId = p.Id;
        try
        {
            using var process = await psi.ExecuteProcessAsync(stdOut, stdErr, processStartAction.WrapTask(), cancellationToken);
            exitCode = process.ExitCode;
        }
        catch (Win32Exception win32Exception)
        {
            exitCode = win32Exception.ErrorCode;
        }
        return new(exitCode, stdOutStringBuilder.ToString(), stdErrStringBuilder.ToString())
        {
            ProcessId = processId
        };
    }

    /// <summary>
    /// Wait for process exit and get exit code
    /// </summary>
    /// <param name="psi">Process is started from this information</param>
    /// <param name="stdOut">Defaults to Console.Out</param>
    /// <param name="stdErr">Defaults to Console.Error</param>
    /// <returns>Process exit code</returns>
    public static int GetExitCode(this ProcessStartInfo psi, TextWriter? stdOut = null,
        TextWriter? stdErr = null)
    {
        try
        {
            using var process = psi.ExecuteProcess(stdOut, stdErr);
            return process.ExitCode;
        }
        catch (Win32Exception win32Exception)
        {
            return win32Exception.ErrorCode;
        }
    }

    /// <summary>
    /// Wait for process exit and get exit code
    /// </summary>
    /// <param name="psi">Process is started from this information</param>
    /// <param name="stdOut">Defaults to Console.Out</param>
    /// <param name="stdErr">Defaults to Console.Error</param>
    /// <param name="cancellationToken">cancellationToken</param>
    /// <returns>Process exit code</returns>
    public static async Task<int> GetExitCodeAsync(this ProcessStartInfo psi, TextWriter? stdOut = null,
        TextWriter? stdErr = null, CancellationToken cancellationToken = default)
    {
        try
        {
            using var process = await psi.ExecuteProcessAsync(stdOut, stdErr, null, cancellationToken);
            return process.ExitCode;
        }
        catch (Win32Exception win32Exception)
        {
            return win32Exception.ErrorCode;
        }
    }

    /// <summary>
    /// Execute process
    /// </summary>
    /// <param name="psi">Process is started from this information</param>
    /// <param name="stdOut">Defaults to Console.Out</param>
    /// <param name="stdErr">Defaults to Console.Error</param>
    /// <param name="processStartAction">Action to execute when process start</param>
    /// <returns>Process executed</returns>
    public static Process ExecuteProcess(this ProcessStartInfo psi, TextWriter? stdOut = null,
        TextWriter? stdErr = null, Action<Process>? processStartAction = null)
    {
        psi.RedirectStandardOutput = stdOut != null;
        psi.RedirectStandardError = stdErr != null;

        var process = new Process();
        process.StartInfo = psi;
        process.OutputDataReceived += (_, e) =>
        {
            if (e.Data != null)
                stdOut?.WriteLine(e.Data);
        };
        process.ErrorDataReceived += (_, e) =>
        {
            if (e.Data != null)
                stdErr?.WriteLine(e.Data);
        };

        process.Start();
        processStartAction?.Invoke(process);

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        process.WaitForExit();

        return process;
    }

    /// <summary>
    /// Execute process
    /// </summary>
    /// <param name="psi">Process is started from this information</param>
    /// <param name="stdOut">Defaults to Console.Out</param>
    /// <param name="stdErr">Defaults to Console.Error</param>
    /// <param name="processStartAction">Action to execute when process start</param>
    /// <param name="cancellationToken">cancellationToken</param>
    /// <returns>Process exit code</returns>
    public static async Task<Process> ExecuteProcessAsync(this ProcessStartInfo psi, TextWriter? stdOut = null,
        TextWriter? stdErr = null, Func<Process, Task>? processStartAction = null, CancellationToken cancellationToken = default)
    {
        psi.RedirectStandardOutput = stdOut != null;
        psi.RedirectStandardError = stdErr != null;

        var process = new Process();
        process.StartInfo = psi;
        var stdOutComplete = new TaskCompletionSource();
        var stdErrComplete = new TaskCompletionSource();
        process.OutputDataReceived += (_, e) =>
        {
            if (e.Data != null)
                stdOut?.WriteLine(e.Data);
            else
                stdOutComplete.SetResult();
        };
        process.ErrorDataReceived += (_, e) =>
        {
            if (e.Data != null)
                stdErr?.WriteLine(e.Data);
            else
                stdErrComplete.SetResult();
        };

        process.Start();
        if (processStartAction is not null)
        {
            await processStartAction.Invoke(process);
        }

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, ApplicationHelper.ExitToken);
        cts.Token.Register((p) => ((Process)p!).TryKill(), process);
        await Task.WhenAll(process.WaitForExitAsync(cts.Token), stdOutComplete.Task, stdErrComplete.Task);
        return process;
    }

    /// <summary>
    /// Try kill process
    /// </summary>
    /// <param name="process"></param>
    /// <param name="entireProcessTree"></param>
    /// <returns></returns>
    public static bool TryKill(this Process process, bool entireProcessTree = true)
    {
        return
#if NET6_0_OR_GREATER
        process.Try(x => x.Kill(entireProcessTree))
#else
        process.Try(x => x.Kill())
#endif
            ;
    }
}
