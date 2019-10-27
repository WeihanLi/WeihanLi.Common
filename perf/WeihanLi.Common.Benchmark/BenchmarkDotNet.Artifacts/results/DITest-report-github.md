``` ini

BenchmarkDotNet=v0.10.14, OS=Windows 10.0.18362
Intel Core i5-6300U CPU 2.40GHz (Skylake), 1 CPU, 4 logical and 2 physical cores
.NET Core SDK=3.0.100
  [Host]     : .NET Core 2.1.13 (CoreCLR 4.6.28008.01, CoreFX 4.6.28008.01), 64bit RyuJIT
  Job-WYOCTV : .NET Core 2.1.13 (CoreCLR 4.6.28008.01, CoreFX 4.6.28008.01), 64bit RyuJIT

RemoveOutliers=False  Runtime=Core  LaunchCount=3  
RunStrategy=Throughput  TargetCount=10  WarmupCount=5  

```
|                        Method |         Mean |      Error |     StdDev |          Op/s | Scaled | ScaledSD |    Gen 0 | Allocated |
|------------------------------ |-------------:|-----------:|-----------:|--------------:|-------:|---------:|---------:|----------:|
|                          NoDI |     7.339 ns |  0.4238 ns |  0.6343 ns | 136,254,586.0 |   1.00 |     0.00 |   7.6256 |   12000 B |
|                     Singleton |   122.726 ns | 10.2681 ns | 15.3688 ns |   8,148,233.7 |  16.83 |     2.42 |        - |       0 B |
|                        Scoped |   115.013 ns |  4.3511 ns |  6.5125 ns |   8,694,685.5 |  15.77 |     1.47 |        - |       0 B |
|                     Transient |    55.458 ns |  2.3618 ns |  3.5350 ns |  18,031,583.2 |   7.60 |     0.74 |   7.5989 |   12000 B |
| ServiceContainerSingletonTest |   567.988 ns | 27.2793 ns | 40.8304 ns |   1,760,601.5 |  77.88 |     8.00 | 144.5313 |  228000 B |
|    ServiceContainerScopedTest |   602.487 ns | 37.9224 ns | 56.7604 ns |   1,659,786.3 |  82.61 |     9.83 | 144.5313 |  228000 B |
| ServiceContainerTransientTest | 1,478.825 ns | 57.9028 ns | 86.6661 ns |     676,212.6 | 202.77 |    19.08 | 406.2500 |  640000 B |
