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
#if NET6_0_OR_GREATER
#else
    public static Task WaitForExitAsync(this Process process, CancellationToken cancellationToken = default)
    {
        Guard.NotNull(process);

        var tcs = new TaskCompletionSource<object?>(TaskCreationOptions.RunContinuationsAsynchronously);
        EventHandler handler = (_, _) => tcs.TrySetResult(null);
        try
        {
            process.Exited += handler;
            tcs.Task.Wait(cancellationToken);
        }
        catch (OperationCanceledException)when (cancellationToken.IsCancellationRequested)
        {
            tcs.TrySetCanceled();
        }
        catch (Exception ex)
        {
            tcs.SetException(ex);
        }
        finally
        {
            process.Exited -= handler;
        }

        return tcs.Task;
    }
#endif

    /// <summary>
    /// Get process execute result
    /// </summary>
    /// <param name="psi">ProcessStartInfo</param>
    /// <returns>process output and exitCode</returns>
    public static async Task<CommandResult> GetResultAsync(this ProcessStartInfo psi)
    {
        var stdOutStringBuilder = new StringBuilder();
        using var stdOut = new StringWriter(stdOutStringBuilder);
        var stdErrStringBuilder = new StringBuilder();
        using var stdErr = new StringWriter(stdErrStringBuilder);
        var exitCode = await GetExitCodeAsync(psi, stdOut, stdErr);
        return new(exitCode, stdOutStringBuilder.ToString(), stdErrStringBuilder.ToString());
    }

    /// <summary>
    /// Wait for process exit and get exit code
    /// </summary>
    /// <param name="psi">Process is started from this information</param>
    /// <param name="stdOut">Defaults to Console.Out</param>
    /// <param name="stdErr">Defaults to Console.Error</param>
    /// <returns>Process exit code</returns>
    public static async Task<int> GetExitCodeAsync(this ProcessStartInfo psi, TextWriter? stdOut = null,
        TextWriter? stdErr = null)
    {
        stdOut ??= Console.Out;
        stdErr ??= Console.Error;
        psi.UseShellExecute = false;
        psi.RedirectStandardError = true;
        psi.RedirectStandardOutput = true;
        using var process = new Process { StartInfo = psi };
        var stdOutComplete = new TaskCompletionSource<object?>();
        var stdErrComplete = new TaskCompletionSource<object?>();
        process.OutputDataReceived += (_, e) =>
        {
            if (e.Data != null)
                stdOut.WriteLine(e.Data);
            else
                stdOutComplete.SetResult(null);
        };
        process.ErrorDataReceived += (_, e) =>
        {
            if (e.Data != null)
                stdErr.WriteLine(e.Data);
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
        await Task.WhenAll(process.WaitForExitAsync(), stdOutComplete.Task, stdErrComplete.Task);

        return process.ExitCode;
    }
}
