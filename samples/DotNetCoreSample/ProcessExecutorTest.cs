using System;
using System.Diagnostics;
using WeihanLi.Common.Helpers;

namespace DotNetCoreSample;

public class ProcessExecutorTest
{
    public static void RawProcessTest()
    {
        using (var process = new Process()
        {
            StartInfo = new ProcessStartInfo("dotnet", "--info"),
            EnableRaisingEvents = true,
        })
        {
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardError = true;

            process.OutputDataReceived += (sender, args) =>
            {
                Console.WriteLine(args.Data);
            };
            process.Exited += (sender, args) =>
            {
                if (sender is Process _process)
                {
                    Console.WriteLine($"The Process({_process.Id}) exited with code({_process.ExitCode})");
                }
            };
            process.Start();
            process.BeginOutputReadLine();

            process.WaitForExit();
        }
    }

    public static void DotNetInfoTest()
    {
        using var executor = new ProcessExecutor("dotnet", "--info");

        executor.OnOutputDataReceived += (sender, str) =>
        {
            Console.WriteLine(str);
        };
        executor.Execute();
    }

    public static void DotNetNugetGlobalPackagesInfoTest()
    {
        using var executor = new ProcessExecutor("dotnet", "nuget locals global-packages -l");
        var folder = string.Empty;
        executor.OnOutputDataReceived += (sender, str) =>
        {
            if (str is null)
                return;

            Console.WriteLine(str);

            if (str.StartsWith("global-packages:"))
            {
                folder = str.Substring("global-packages:".Length).Trim();
            }
        };
        executor.Execute();

        System.Console.WriteLine(folder);
    }
}
