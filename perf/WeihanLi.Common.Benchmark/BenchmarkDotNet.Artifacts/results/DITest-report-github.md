``` ini

BenchmarkDotNet=v0.10.14, OS=Windows 10.0.18362
Intel Core i5-6300U CPU 2.40GHz (Skylake), 1 CPU, 4 logical and 2 physical cores
.NET Core SDK=3.0.100
  [Host]     : .NET Core 2.1.13 (CoreCLR 4.6.28008.01, CoreFX 4.6.28008.01), 64bit RyuJIT
  Job-UURGLC : .NET Core 2.1.13 (CoreCLR 4.6.28008.01, CoreFX 4.6.28008.01), 64bit RyuJIT

RemoveOutliers=False  Runtime=Core  LaunchCount=3  
RunStrategy=Throughput  TargetCount=10  WarmupCount=5  

```
|                        Method |         Mean |     Error |     StdDev |       Median |          Op/s | Scaled | ScaledSD |      Gen 0 |  Allocated |
|------------------------------ |-------------:|----------:|-----------:|-------------:|--------------:|-------:|---------:|-----------:|-----------:|
|                          NoDI |     8.082 ns |  1.168 ns |   1.748 ns |     7.370 ns | 123,737,124.2 |   1.00 |     0.00 |   762.6953 |  1200000 B |
|                     Singleton |   128.733 ns | 10.401 ns |  15.568 ns |   122.103 ns |   7,768,021.0 |  16.60 |     3.78 |          - |        0 B |
|                        Scoped |   115.224 ns |  3.434 ns |   5.139 ns |   113.493 ns |   8,678,739.2 |  14.86 |     2.94 |          - |        0 B |
|                     Transient |    61.175 ns |  7.712 ns |  11.542 ns |    55.817 ns |  16,346,632.9 |   7.89 |     2.13 |   761.7188 |  1200000 B |
| ServiceContainerSingletonTest |   642.179 ns | 43.931 ns |  65.754 ns |   615.747 ns |   1,557,198.7 |  82.82 |    18.10 | 14437.5000 | 22800000 B |
|    ServiceContainerScopedTest |   658.143 ns | 68.601 ns | 102.679 ns |   601.012 ns |   1,519,427.5 |  84.88 |    21.07 | 14468.7500 | 22800000 B |
| ServiceContainerTransientTest | 1,790.600 ns | 55.919 ns |  83.698 ns | 1,775.545 ns |     558,472.0 | 230.92 |    45.85 | 46000.0000 | 72400000 B |
