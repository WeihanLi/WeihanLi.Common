using BenchmarkDotNet.Running;

namespace WeihanLi.Common.Benchmark;

public class Program
{
    public static void Main(string[] args)
    {
        // BenchmarkRunner.Run<MapperTest>();
        // BenchmarkRunner.Run<CreateInstanceTest>();
        // BenchmarkRunner.Run<DITest>();

        BenchmarkRunner.Run<PipelineTest>();

        Console.ReadLine();
    }
}
