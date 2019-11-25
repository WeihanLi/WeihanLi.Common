``` ini

BenchmarkDotNet=v0.12.0, OS=Windows 10.0.18362
Intel Core i5-3470 CPU 3.20GHz (Ivy Bridge), 1 CPU, 4 logical and 4 physical cores
.NET Core SDK=3.0.100
  [Host]     : .NET Core 2.1.13 (CoreCLR 4.6.28008.01, CoreFX 4.6.28008.01), X64 RyuJIT
  DefaultJob : .NET Core 2.1.13 (CoreCLR 4.6.28008.01, CoreFX 4.6.28008.01), X64 RyuJIT


```
|                        Method |       Mean |     Error |    StdDev |          Op/s |  Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|------------------------------ |-----------:|----------:|----------:|--------------:|-------:|--------:|-------:|------:|------:|----------:|
|                          NoDI |   6.294 ns | 0.0800 ns | 0.0748 ns | 158,885,471.4 |   1.00 |    0.00 | 0.0076 |     - |     - |      24 B |
|                     Singleton | 100.930 ns | 0.3708 ns | 0.3468 ns |   9,907,897.6 |  16.04 |    0.17 |      - |     - |     - |         - |
|                     Transient |  49.680 ns | 0.5275 ns | 0.4934 ns |  20,128,957.4 |   7.89 |    0.10 | 0.0076 |     - |     - |      24 B |
| ServiceContainerSingletonTest | 348.255 ns | 6.6019 ns | 6.1754 ns |   2,871,459.3 |  55.34 |    1.33 | 0.1169 |     - |     - |     368 B |
| ServiceContainerTransientTest | 841.700 ns | 1.9591 ns | 1.7367 ns |   1,188,072.3 | 133.68 |    1.66 | 0.2533 |     - |     - |     800 B |
