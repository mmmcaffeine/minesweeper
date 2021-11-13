using BenchmarkDotNet.Running;
using Dgt.Minesweeper.Engine;

_ = BenchmarkSwitcher.FromAssembly(typeof(AssemblyMarker).Assembly).Run(args);