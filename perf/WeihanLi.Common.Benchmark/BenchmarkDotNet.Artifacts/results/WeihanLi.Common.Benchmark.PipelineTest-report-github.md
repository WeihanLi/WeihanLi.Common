``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.22581
Intel Core i5-6300U CPU 2.40GHz (Skylake), 1 CPU, 4 logical and 2 physical cores
.NET SDK=7.0.100-preview.2.22153.17
  [Host]     : .NET 6.0.3 (6.0.322.12309), X64 RyuJIT
  DefaultJob : .NET 6.0.3 (6.0.322.12309), X64 RyuJIT


```
|                  Method |         Mean |      Error |     StdDev |       Median |      Gen 0 | Allocated |
|------------------------ |-------------:|-----------:|-----------:|-------------:|-----------:|----------:|
|       ValueTaskPipeline |     18.69 μs |   1.402 μs |   4.133 μs |     16.55 μs |    19.3176 |     30 KB |
| ValueTaskPipelineInvoke | 21,669.82 μs | 403.984 μs | 449.027 μs | 21,635.27 μs | 19875.0000 | 30,470 KB |
|            TaskPipeline |     14.00 μs |   0.279 μs |   0.443 μs |     13.87 μs |    19.3329 |     30 KB |
|      TaskPipelineInvoke | 17,902.60 μs | 346.586 μs | 508.021 μs | 17,881.09 μs | 19875.0000 | 30,470 KB |
