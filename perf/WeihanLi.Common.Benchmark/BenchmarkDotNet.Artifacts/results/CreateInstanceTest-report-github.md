``` ini

BenchmarkDotNet=v0.10.14, OS=Windows 10.0.18362
Intel Core i5-6300U CPU 2.40GHz (Skylake), 1 CPU, 4 logical and 2 physical cores
.NET Core SDK=2.2.300
  [Host]     : .NET Core 2.1.12 (CoreCLR 4.6.27817.01, CoreFX 4.6.27818.01), 64bit RyuJIT
  DefaultJob : .NET Core 2.1.12 (CoreCLR 4.6.27817.01, CoreFX 4.6.27818.01), 64bit RyuJIT


```
|                       Method |          Mean |       Error |      StdDev |
|----------------------------- |--------------:|------------:|------------:|
|      NewInstanceByExpression | 110,455.34 ns | 857.5566 ns | 669.5237 ns |
|      NewInstanceByReflection |      61.80 ns |   1.3049 ns |   1.2816 ns |
| NewInstanceByActivatorHelper |     311.31 ns |   5.2091 ns |   4.8726 ns |
|                  NewInstance |      10.07 ns |   0.3579 ns |   0.3172 ns |
