``` ini

BenchmarkDotNet=v0.12.1, OS=Windows 10.0.19041.610 (2004/?/20H1)
Intel Core i5-6300U CPU 2.40GHz (Skylake), 1 CPU, 4 logical and 2 physical cores
.NET Core SDK=5.0.100-rc.2.20479.15
  [Host]     : .NET Core 3.1.9 (CoreCLR 4.700.20.47201, CoreFX 4.700.20.47203), X64 RyuJIT
  DefaultJob : .NET Core 3.1.9 (CoreCLR 4.700.20.47201, CoreFX 4.700.20.47203), X64 RyuJIT


```
|              Method |       Mean |    Error |   StdDev |     Median | Ratio | RatioSD |
|-------------------- |-----------:|---------:|---------:|-----------:|------:|--------:|
| AutoMapperBenchmark |   167.6 ns |  4.51 ns | 12.57 ns |   161.4 ns |  1.00 |    0.00 |
|  MapHelperBenchmark | 4,335.0 ns | 26.20 ns | 23.22 ns | 4,332.7 ns | 26.31 |    1.22 |
