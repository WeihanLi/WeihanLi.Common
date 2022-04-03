``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.22581
Intel Core i5-6300U CPU 2.40GHz (Skylake), 1 CPU, 4 logical and 2 physical cores
.NET SDK=7.0.100-preview.2.22153.17
  [Host]     : .NET 6.0.3 (6.0.322.12309), X64 RyuJIT
  DefaultJob : .NET 6.0.3 (6.0.322.12309), X64 RyuJIT


```
|            Method |     Mean |    Error |   StdDev | Ratio | RatioSD |
|------------------ |---------:|---------:|---------:|------:|--------:|
| ValueTaskPipeline | 778.2 ns | 27.81 ns | 82.01 ns |  1.00 |    0.00 |
|      TaskPipeline | 623.1 ns | 12.47 ns | 32.86 ns |  0.78 |    0.08 |
