``` ini

BenchmarkDotNet=v0.10.14, OS=Windows 10.0.18362
Intel Core i5-3470 CPU 3.20GHz (Ivy Bridge), 1 CPU, 4 logical and 4 physical cores
.NET Core SDK=3.0.100
  [Host]     : .NET Core 2.1.13 (CoreCLR 4.6.28008.01, CoreFX 4.6.28008.01), 64bit RyuJIT
  Job-DCPQUK : .NET Core 2.1.13 (CoreCLR 4.6.28008.01, CoreFX 4.6.28008.01), 64bit RyuJIT

RemoveOutliers=False  Runtime=Core  LaunchCount=3  
RunStrategy=Throughput  TargetCount=10  WarmupCount=5  

```
|                        Method |         Mean |      Error |     StdDev |       Median |          Op/s | Scaled | ScaledSD |      Gen 0 |  Allocated |
|------------------------------ |-------------:|-----------:|-----------:|-------------:|--------------:|-------:|---------:|-----------:|-----------:|
|                          NoDI |     6.346 ns |  0.0894 ns |  0.1338 ns |     6.335 ns | 157,571,216.1 |   1.00 |     0.00 |   381.3477 |  1200000 B |
|                     Singleton |   100.551 ns |  0.3524 ns |  0.5275 ns |   100.452 ns |   9,945,239.1 |  15.85 |     0.34 |          - |        0 B |
|                        Scoped |    93.423 ns |  0.4487 ns |  0.6716 ns |    93.274 ns |  10,703,980.5 |  14.73 |     0.32 |          - |        0 B |
|                     Transient |    47.579 ns |  0.3155 ns |  0.4722 ns |    47.537 ns |  21,017,706.1 |   7.50 |     0.17 |   378.9063 |  1200000 B |
| ServiceContainerSingletonTest |   577.192 ns | 16.8483 ns | 25.2178 ns |   590.905 ns |   1,732,524.5 |  90.99 |     4.33 |  7218.7500 | 22800000 B |
|    ServiceContainerScopedTest |   595.382 ns |  5.9631 ns |  8.9253 ns |   593.293 ns |   1,679,594.8 |  93.85 |     2.37 |  7218.7500 | 22800000 B |
| ServiceContainerTransientTest | 1,356.705 ns | 14.3341 ns | 21.4547 ns | 1,357.348 ns |     737,080.1 | 213.87 |     5.51 | 20312.5000 | 64000000 B |
