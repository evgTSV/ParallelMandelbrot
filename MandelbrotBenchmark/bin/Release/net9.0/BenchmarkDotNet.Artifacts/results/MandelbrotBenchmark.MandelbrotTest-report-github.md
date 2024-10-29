```

BenchmarkDotNet v0.13.12, Windows 11 (10.0.22631.3737/23H2/2023Update/SunValley3)
AMD Ryzen 5 2600X, 1 CPU, 12 logical and 6 physical cores
.NET SDK 9.0.100-preview.5.24307.3
  [Host]     : .NET 9.0.0 (9.0.24.30607), X64 RyuJIT AVX2
  DefaultJob : .NET 9.0.0 (9.0.24.30607), X64 RyuJIT AVX2


```
| Method                      | Mean      | Error    | StdDev    | Gen0      | Gen1     | Gen2     | Allocated |
|---------------------------- |----------:|---------:|----------:|----------:|---------:|---------:|----------:|
| SequentialProcessing        | 500.66 ms | 9.864 ms | 12.114 ms |         - |        - |        - |  91.55 MB |
| ParallelProcessing          |  78.98 ms | 0.434 ms |  0.384 ms |  857.1429 | 857.1429 | 857.1429 |  91.56 MB |
| ParallelProcessingSaturated | 115.90 ms | 1.136 ms |  1.063 ms | 1333.3333 | 333.3333 | 333.3333 |  95.96 MB |
