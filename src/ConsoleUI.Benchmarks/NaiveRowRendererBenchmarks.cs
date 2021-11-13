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
            public const string StringCreate = "StringCreate";
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

        [Benchmark]
        [BenchmarkCategory(BenchmarkCategories.StringCreate, BenchmarkCategories.RenderTopBorder)]
        [ArgumentsSource(nameof(ValuesForNumberOfRowsAndColumns))]
#pragma warning disable CA1822
        public string RenderTopBorder_Using_StringCreate(int numberOfRowsAndColumns)
#pragma warning restore CA1822
        {
            var prefixLength = numberOfRowsAndColumns.ToString().Length;
            var totalLength = prefixLength + 1 + numberOfRowsAndColumns * 2 + 1;

            return string.Create(totalLength, prefixLength, (span, i) =>
            {
                // Would it be more efficient to _not_ pre-fill the span, but fill it within the loop?
                span.Fill('═');

                span[..(prefixLength + 1)].Fill(' ');
                span[prefixLength + 1] = '╔';

                for (var x = span.Length - 3; x >= i + 3; x -= 2)
                {
                    span[x] = '╦';
                }

                span[^1] = '╗';
            });
        }
    }
}