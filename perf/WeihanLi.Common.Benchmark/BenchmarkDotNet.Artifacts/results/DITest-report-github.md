``` ini

BenchmarkDotNet=v0.10.14, OS=Windows 10.0.18362
Intel Core i5-6300U CPU 2.40GHz (Skylake), 1 CPU, 4 logical and 2 physical cores
.NET Core SDK=3.0.100
  [Host]     : .NET Core 2.1.13 (CoreCLR 4.6.28008.01, CoreFX 4.6.28008.01), 64bit RyuJIT
  Job-FWCQPW : .NET Core 2.1.13 (CoreCLR 4.6.28008.01, CoreFX 4.6.28008.01), 64bit RyuJIT

RemoveOutliers=False  Runtime=Core  LaunchCount=3  
RunStrategy=Throughput  TargetCount=10  WarmupCount=5  

```
|                        Method |         Mean |      Error |     StdDev |          Op/s | Scaled | ScaledSD |   Gen 0 | Allocated |
|------------------------------ |-------------:|-----------:|-----------:|--------------:|-------:|---------:|--------:|----------:|
|                          NoDI |     7.676 ns |  0.4733 ns |  0.7084 ns | 130,278,835.3 |   1.00 |     0.00 |  1.5249 |    2400 B |
|                     Singleton |   125.977 ns |  9.8719 ns | 14.7758 ns |   7,937,974.5 |  16.53 |     2.32 |       - |       0 B |
|                        Scoped |   118.379 ns |  3.7293 ns |  5.5818 ns |   8,447,427.8 |  15.53 |     1.43 |       - |       0 B |
|                     Transient |    58.730 ns |  4.0603 ns |  6.0772 ns |  17,027,171.8 |   7.71 |     1.00 |  1.5106 |    2400 B |
| ServiceContainerSingletonTest |   598.182 ns | 57.5579 ns | 86.1500 ns |   1,671,731.8 |  78.49 |    12.76 | 28.9307 |   45600 B |
|    ServiceContainerScopedTest |   611.045 ns | 31.5689 ns | 47.2508 ns |   1,636,540.0 |  80.18 |     8.81 | 28.9307 |   45600 B |
| ServiceContainerTransientTest | 1,457.991 ns | 58.3151 ns | 87.2833 ns |     685,875.2 | 191.31 |    18.88 | 81.2988 |  128000 B |
