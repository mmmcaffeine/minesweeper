using System.Collections.Generic;
using System.Linq;
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
            public const string CharArray = "CharArray";
            public const string JoinedEnumerableOfString = "JoinedEnumerable";
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

        [Benchmark]
        [BenchmarkCategory(BenchmarkCategories.CharArray, BenchmarkCategories.RenderTopBorder)]
        [ArgumentsSource(nameof(ValuesForNumberOfRowsAndColumns))]
#pragma warning disable CA1822
        public string RenderTopBorder_Using_CharArray(int numberOfRowsAndColumns)
#pragma warning restore CA1822
        {
            var prefixLength = numberOfRowsAndColumns.ToString().Length;
            var totalLength = prefixLength + 1 + numberOfRowsAndColumns * 2 + 1;
            var chars = new char[totalLength];

            for (var i = 0; i <= prefixLength + 1; i++)
            {
                chars[i] = ' ';
            }

            chars[prefixLength + 1] = '╔';

            for (var i = chars.Length - 3; i >= prefixLength + 3; i -= 2)
            {
                chars[i] = '╦';
                chars[i - 1] = '═';
            }

            chars[^2] = '═';
            chars[^1] = '╗';

            return new string(chars);
        }

        [Benchmark]
        [BenchmarkCategory(BenchmarkCategories.JoinedEnumerableOfString, BenchmarkCategories.RenderTopBorder)]
        [ArgumentsSource(nameof(ValuesForNumberOfRowsAndColumns))]
#pragma warning disable CA1822
        public string RenderTopBorder_Using_JoinedEnumerableOfString(int numberOfRowsAndColumns)
#pragma warning restore CA1822
        {
            var prefixLength = numberOfRowsAndColumns.ToString().Length;
            var columns = Enumerable.Range(0, numberOfRowsAndColumns - 1).Select(_ => "═╦");
            var prefix = Enumerable.Range(0, prefixLength).Select(_ => ' ');

            return $"{string.Join(null, prefix)} ╔{string.Join(null, columns)}═╗";
        }
    }
}