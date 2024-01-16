// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using WeihanLi.Common.Helpers;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Extensions;

public static class ProcessExtension
{
#if NET6_0_OR_GREATER
#else
    public static Task WaitForExitAsync(this Process process, CancellationToken cancellationToken = default)
    {
        Common.Guard.NotNull(process);
        process.EnableRaisingEvents = true;
        var tcs = new TaskCompletionSource<object?>(TaskCreationOptions.RunContinuationsAsynchronously);
        try
        {
            process.Exited += EventHandler;

            if (process.StartTime == DateTime.MinValue)
            {
                process.Start();
            }

            tcs.Task.Wait(cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            tcs.TrySetCanceled();
            process.Try(p => p.Kill());
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

        void EventHandler(object o, EventArgs eventArgs) => tcs.TrySetResult(null);
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
        cts.Token.Register((p) => ((Process)p).TryKill(), process);
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
        var exitCode = GetExitCode(psi, stdOut, stdErr);
        return new CommandResult(exitCode, stdOutStringBuilder.ToString(), stdErrStringBuilder.ToString());
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
#if NETSTANDARD2_1 || NET6_0_OR_GREATER
        await 
#endif
        using var stdOut = new StringWriter(stdOutStringBuilder);
        var stdErrStringBuilder = new StringBuilder();
#if NETSTANDARD2_1 || NET6_0_OR_GREATER
        await 
#endif
        using var stdErr = new StringWriter(stdErrStringBuilder);
        var exitCode = await GetExitCodeAsync(psi, stdOut, stdErr, cancellationToken);
        return new(exitCode, stdOutStringBuilder.ToString(), stdErrStringBuilder.ToString());
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
        psi.RedirectStandardOutput = stdOut != null;
        psi.RedirectStandardError = stdErr != null;
        psi.UseShellExecute = false;
        using var process = new Process();
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

        try
        {
            process.Start();
        }
        catch (Win32Exception win32Exception)
        {
            return win32Exception.ErrorCode;
        }

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        process.WaitForExit();

        return process.ExitCode;
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
        psi.RedirectStandardOutput = stdOut != null;
        psi.RedirectStandardError = stdErr != null;
        psi.UseShellExecute = false;
        using var process = new Process();
        process.StartInfo = psi;
        var stdOutComplete = new TaskCompletionSource<object?>();
        var stdErrComplete = new TaskCompletionSource<object?>();
        process.OutputDataReceived += (_, e) =>
        {
            if (e.Data != null)
                stdOut?.WriteLine(e.Data);
            else
                stdOutComplete.SetResult(null);
        };
        process.ErrorDataReceived += (_, e) =>
        {
            if (e.Data != null)
                stdErr?.WriteLine(e.Data);
            else
                stdErrComplete.SetResult(null);
        };
        try
        {
            process.Start();
        }
        catch (Win32Exception win32Exception)
        {
            return win32Exception.ErrorCode;
        }

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, ApplicationHelper.ExitToken);
        cts.Token.Register((p) => ((Process)p).TryKill(), process);
        await Task.WhenAll(process.WaitForExitAsync(cts.Token), stdOutComplete.Task, stdErrComplete.Task);
        return process.ExitCode;
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
