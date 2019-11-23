``` ini

BenchmarkDotNet=v0.12.0, OS=Windows 10.0.18362
Intel Core i5-6300U CPU 2.40GHz (Skylake), 1 CPU, 4 logical and 2 physical cores
.NET Core SDK=3.0.100
  [Host]     : .NET Core 2.1.13 (CoreCLR 4.6.28008.01, CoreFX 4.6.28008.01), X64 RyuJIT
  DefaultJob : .NET Core 2.1.13 (CoreCLR 4.6.28008.01, CoreFX 4.6.28008.01), X64 RyuJIT


```
|                        Method |       Mean |      Error |     StdDev |          Op/s |  Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|------------------------------ |-----------:|-----------:|-----------:|--------------:|-------:|--------:|-------:|------:|------:|----------:|
|                          NoDI |   6.292 ns |  0.0535 ns |  0.0417 ns | 158,942,541.6 |   1.00 |    0.00 | 0.0153 |     - |     - |      24 B |
|                     Singleton | 131.331 ns |  4.3002 ns | 12.3381 ns |   7,614,333.2 |  20.61 |    2.25 |      - |     - |     - |         - |
|                     Transient |  56.098 ns |  1.1180 ns |  1.8679 ns |  17,825,890.0 |   8.99 |    0.41 | 0.0152 |     - |     - |      24 B |
| ServiceContainerSingletonTest | 353.726 ns |  4.5762 ns |  3.8214 ns |   2,827,049.0 |  56.26 |    0.74 | 0.2338 |     - |     - |     368 B |
| ServiceContainerTransientTest | 890.285 ns | 13.0626 ns | 10.9079 ns |   1,123,235.6 | 141.38 |    2.10 | 0.5083 |     - |     - |     800 B |
