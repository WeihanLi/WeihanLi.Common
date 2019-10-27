``` ini

BenchmarkDotNet=v0.10.14, OS=Windows 10.0.18362
Intel Core i5-6300U CPU 2.40GHz (Skylake), 1 CPU, 4 logical and 2 physical cores
.NET Core SDK=3.0.100
  [Host]     : .NET Core 2.1.13 (CoreCLR 4.6.28008.01, CoreFX 4.6.28008.01), 64bit RyuJIT
  Job-OAEGHU : .NET Core 2.1.13 (CoreCLR 4.6.28008.01, CoreFX 4.6.28008.01), 64bit RyuJIT

RemoveOutliers=False  Runtime=Core  RunStrategy=Throughput  

```
|                        Method |         Mean |       Error |      StdDev |       Median |          Op/s | Scaled | ScaledSD |  Gen 0 | Allocated |
|------------------------------ |-------------:|------------:|------------:|-------------:|--------------:|-------:|---------:|-------:|----------:|
|                          NoDI |     6.136 ns |   0.1444 ns |   0.1351 ns |     6.130 ns | 162,965,922.7 |   1.00 |     0.00 | 0.0152 |      24 B |
|                     Singleton |   148.111 ns |  13.3853 ns |  39.4669 ns |   134.727 ns |   6,751,676.4 |  24.15 |     6.42 |      - |       0 B |
|                        Scoped |   149.587 ns |  12.0026 ns |  35.3899 ns |   135.096 ns |   6,685,055.5 |  24.39 |     5.77 |      - |       0 B |
|                     Transient |    55.535 ns |   1.6135 ns |   4.7575 ns |    54.303 ns |  18,006,606.1 |   9.05 |     0.80 | 0.0151 |      24 B |
| ServiceContainerSingletonTest |   570.686 ns |   5.4923 ns |   5.1375 ns |   570.990 ns |   1,752,275.8 |  93.04 |     2.14 | 0.2890 |     456 B |
|    ServiceContainerScopedTest |   612.280 ns |  30.0027 ns |  88.4635 ns |   578.585 ns |   1,633,240.2 |  99.83 |    14.51 | 0.2890 |     456 B |
| ServiceContainerTransientTest | 1,759.219 ns | 148.7697 ns | 438.6507 ns | 1,560.286 ns |     568,434.1 | 286.82 |    71.44 | 0.8125 |    1280 B |
