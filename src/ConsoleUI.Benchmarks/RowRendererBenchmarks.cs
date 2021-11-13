using System.Collections.Generic;
using BenchmarkDotNet.Attributes;

namespace Dgt.Minesweeper.ConsoleUI
{
    [MemoryDiagnoser]
    public class RowRendererBenchmarks
    {
        private static class BenchmarkCategories
        {
            public const string NaiveRowRenderer = "NaiveRowRenderer";
            public const string EfficientGameRenderer = "EfficientGameRenderer";
            public const string RenderTopBorder = "RenderTopBorder";
            public const string StringCreate = "StringCreate";
        }
        
        private IRowRenderer _naiveRowRenderer = default!;
        private IRowRenderer _efficientGameRenderer = default!;

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
            _naiveRowRenderer = new NaiveRowRenderer();
            _efficientGameRenderer = new EfficientGameRenderer();
        }

        [Benchmark(Baseline = true)]
        [BenchmarkCategory(BenchmarkCategories.NaiveRowRenderer, BenchmarkCategories.RenderTopBorder)]
        [ArgumentsSource(nameof(ValuesForNumberOfRowsAndColumns))]
        public string RenderTopBorder_Using_NaiveRowRenderer(int numberOfRowsAndColumns) =>
            _naiveRowRenderer.RenderTopBorder(numberOfRowsAndColumns, numberOfRowsAndColumns);

        [Benchmark]
        [BenchmarkCategory(BenchmarkCategories.EfficientGameRenderer, BenchmarkCategories.RenderTopBorder)]
        [ArgumentsSource(nameof(ValuesForNumberOfRowsAndColumns))]
        public string RenderTopBorder_Using_EfficientGameRenderer(int numberOfRowsAndColumns) =>
            _efficientGameRenderer.RenderTopBorder(numberOfRowsAndColumns, numberOfRowsAndColumns);
    }
}