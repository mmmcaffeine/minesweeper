using BenchmarkDotNet.Running;
using Dgt.Minesweeper.ConsoleUI;

_ = BenchmarkSwitcher.FromAssembly(typeof(AssemblyMarker).Assembly).Run(args);