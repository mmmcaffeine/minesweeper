using BenchmarkDotNet.Running;
using Dgt.Minesweeper.Benchmarks;

_ = BenchmarkRunner.Run<MinefieldBenchmarks>();