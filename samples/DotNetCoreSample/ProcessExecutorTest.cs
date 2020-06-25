using System;
using System.Diagnostics;
using WeihanLi.Common.Helpers;

namespace DotNetCoreSample
{
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

            executor.OutputDataReceived += (sender, str) =>
            {
                Console.WriteLine(str);
            };
            executor.Execute();
        }
    }
}
