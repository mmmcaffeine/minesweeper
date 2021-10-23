using BenchmarkDotNet.Running;
using Dgt.Minesweeper.Benchmarks;

_ = BenchmarkSwitcher.FromAssembly(typeof(AssemblyMarker).Assembly).Run(args);