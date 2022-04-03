``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.22581
Intel Core i5-6300U CPU 2.40GHz (Skylake), 1 CPU, 4 logical and 2 physical cores
.NET SDK=7.0.100-preview.2.22153.17
  [Host]     : .NET 6.0.3 (6.0.322.12309), X64 RyuJIT
  DefaultJob : .NET 6.0.3 (6.0.322.12309), X64 RyuJIT


```
|            Method |     Mean |     Error |    StdDev | Ratio | RatioSD |     Gen 0 | Allocated |
|------------------ |---------:|----------:|----------:|------:|--------:|----------:|----------:|
| ValueTaskPipeline | 3.329 ms | 0.1639 ms | 0.4833 ms |  1.00 |    0.00 | 1988.2813 |      3 MB |
|      TaskPipeline | 2.775 ms | 0.1464 ms | 0.4316 ms |  0.85 |    0.18 | 1988.2813 |      3 MB |
