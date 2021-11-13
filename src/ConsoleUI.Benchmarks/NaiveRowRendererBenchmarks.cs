using System.Collections.Generic;
using BenchmarkDotNet.Attributes;

namespace Dgt.Minesweeper.ConsoleUI
{
    [MemoryDiagnoser]
    public class NaiveRowRendererBenchmarks
    {
        private static class BenchmarkCategories
        {
            public const string NaiveRowRenderer = "NaiveRowRenderer";
            public const string RenderTopBorder = "RenderTopBorder";
        }
        
        private IRowRenderer _rowRenderer = default!;

        public static IEnumerable<int> ValuesForNumberOfRowsAndColumns
        {
            get
            {
                yield return 2;
                yield return 10;
                yield return 100;
                yield return 1000;
            }
        }

        [GlobalSetup]
        public void GlobalSetup()
        {
            var cellRenderer = new CellRenderer();
            
            _rowRenderer = new NaiveRowRenderer(cellRenderer);
        }

        [Benchmark(Baseline = true)]
        [BenchmarkCategory(BenchmarkCategories.NaiveRowRenderer, BenchmarkCategories.RenderTopBorder)]
        [ArgumentsSource(nameof(ValuesForNumberOfRowsAndColumns))]
        public string RenderTopBorder(int numberOfRowsAndColumns) =>
            _rowRenderer.RenderTopBorder(numberOfRowsAndColumns, numberOfRowsAndColumns);
    }
}