using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using Dgt.Minesweeper.Engine;

namespace Dgt.Minesweeper.ConsoleUI
{
    [MemoryDiagnoser]
    public class GameRendererBenchmarks
    {
        private IGameRenderer _naiveGameRenderer = default!;
        private IGameRenderer _stringCreateGameRenderer = default!;
        private Consumer _consumer = default!;

        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        // BenchmarkDotNet requires public read / write properties for us to use the Params attributes
        [Params(2, 10, 100, 1000)]
        public int NumberOfRowsAndColumns { get; set; }

        [GlobalSetup]
        public void GlobalSetUp()
        {
            var minefield = new Minefield(NumberOfRowsAndColumns, Array.Empty<Location>());
            var game = new Game(minefield);
            var cellRenderer = new CellRenderer();

            _naiveGameRenderer = new NaiveGameRenderer(game, new NaiveRowRenderer(), cellRenderer);
            _stringCreateGameRenderer = new StringCreateGameRenderer(game, cellRenderer);
            _consumer = new Consumer();
        }

        [Benchmark(Baseline = true)]
        public void Render_Using_NaiveGameRenderer() => _naiveGameRenderer.Render().Consume(_consumer);

        [Benchmark]
        public void Render_Using_StringCreateGameRenderer() => _stringCreateGameRenderer.Render().Consume(_consumer);
    }
}