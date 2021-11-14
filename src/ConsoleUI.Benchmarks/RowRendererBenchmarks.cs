using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using Dgt.Minesweeper.Engine;

namespace Dgt.Minesweeper.ConsoleUI
{
    [MemoryDiagnoser]
    public class RowRendererBenchmarks
    {
        private static class BenchmarkCategories
        {
            public const string NaiveRowRenderer = "NaiveRowRenderer";
            public const string EfficientGameRenderer = "EfficientGameRenderer";

            public const string StringCreate = "StringCreate";
            public const string CharArray = "CharArray";
            public const string JoinedEnumerableOfString = "JoinedEnumerable";

            public const string RenderTopBorder = "RenderTopBorder";
            public const string RenderRow = "RenderRow";
            public const string GetNumberOfDigits = "GetNumberOfDigits";
        }
        
        private IRowRenderer _naiveRowRenderer = default!;
        private IRowRenderer _efficientGameRenderer = default!;
        private ICellRenderer _cellRenderer = default!;

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
            _cellRenderer = new CellRenderer();
        }

        [Benchmark(Baseline = true)]
        [BenchmarkCategory(BenchmarkCategories.RenderTopBorder, BenchmarkCategories.NaiveRowRenderer)]
        [ArgumentsSource(nameof(ValuesForNumberOfRowsAndColumns))]
        public string RenderTopBorder_Using_NaiveRowRenderer(int numberOfRowsAndColumns) =>
            _naiveRowRenderer.RenderTopBorder(numberOfRowsAndColumns, numberOfRowsAndColumns);

        [Benchmark]
        [BenchmarkCategory(BenchmarkCategories.RenderTopBorder, BenchmarkCategories.EfficientGameRenderer)]
        [ArgumentsSource(nameof(ValuesForNumberOfRowsAndColumns))]
        public string RenderTopBorder_Using_EfficientGameRenderer(int numberOfRowsAndColumns) =>
            _efficientGameRenderer.RenderTopBorder(numberOfRowsAndColumns, numberOfRowsAndColumns);

        [Benchmark]
        [BenchmarkCategory(BenchmarkCategories.RenderTopBorder, BenchmarkCategories.CharArray)]
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
        [BenchmarkCategory(BenchmarkCategories.RenderTopBorder, BenchmarkCategories.JoinedEnumerableOfString)]
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

        [Benchmark(Baseline = true)]
        [BenchmarkCategory(BenchmarkCategories.RenderRow, BenchmarkCategories.NaiveRowRenderer)]
        [ArgumentsSource(nameof(ValuesForNumberOfRowsAndColumns))]
        public string RenderRow_Using_NaiveRowRenderer(int numberOfRowsAndColumns)
        {
            const int rowIndex = 1;
            var cells = GetCells(numberOfRowsAndColumns, rowIndex);

            return _naiveRowRenderer.RenderRow(numberOfRowsAndColumns, rowIndex, _cellRenderer, cells);
        }

        [Benchmark]
        [BenchmarkCategory(BenchmarkCategories.RenderRow, BenchmarkCategories.EfficientGameRenderer)]
        [ArgumentsSource(nameof(ValuesForNumberOfRowsAndColumns))]
        public string RenderRow_Using_EfficientGameRenderer(int numberOfRowsAndColumns)
        {
            const int rowIndex = 1;
            var cells = GetCells(numberOfRowsAndColumns, rowIndex);

            return _efficientGameRenderer.RenderRow(numberOfRowsAndColumns, rowIndex, _cellRenderer, cells);
        }

        [Benchmark]
        [BenchmarkCategory(BenchmarkCategories.RenderRow, BenchmarkCategories.CharArray)]
        [ArgumentsSource(nameof(ValuesForNumberOfRowsAndColumns))]
        public string RenderRow_Using_CharArray(int numberOfRowsAndColumns)
        {
            const int rowIndex = 1;
            var cells = GetCells(numberOfRowsAndColumns, rowIndex).ToList();
            var rowHeaderLength = numberOfRowsAndColumns switch
            {
                < 10 => 1,
                < 100 => 2,
                < 1_000 => 3,
                _ => numberOfRowsAndColumns.ToString().Length
            };
            var totalLength = rowHeaderLength + 1 + cells.Count * 2 + 1;
            var chars = new char[totalLength];
            var rowIndexChars = rowIndex.ToString().ToCharArray();

            for (var i = 0; i < rowHeaderLength; i++)
            {
                chars[i] = rowHeaderLength - i <= rowIndexChars.Length
                    ? rowIndexChars[i - rowHeaderLength + rowIndexChars.Length]
                    : ' ';
            }

            chars[rowHeaderLength] = ' ';

            for (var i = 0; i < cells.Count; i++)
            {
                chars[rowHeaderLength + 1 + (i * 2)] = '║';
                chars[rowHeaderLength + 2 + (i * 2)] = _cellRenderer.RenderCell(cells[i]);
            }

            chars[^1] = '║';

            return new string(chars);
        }

        private static IEnumerable<Cell> GetCells(int numberOfColumns, int rowIndex)
        {
            for (var i = 0; i < numberOfColumns; i++)
            {
                var location = new Location(i + 1, rowIndex);
                yield return new Cell(location, false, 0);
            }
        }

        [Benchmark]
        [BenchmarkCategory(BenchmarkCategories.GetNumberOfDigits)]
        [ArgumentsSource(nameof(ValuesForNumberOfRowsAndColumns))]
#pragma warning disable CA1822
        public int GetNumberOfDigits_Using_StringLength(int numberOfRowsAndColumns) =>
#pragma warning restore CA1822
            numberOfRowsAndColumns.ToString().Length;

        [Benchmark]
        [BenchmarkCategory(BenchmarkCategories.GetNumberOfDigits)]
        [ArgumentsSource(nameof(ValuesForNumberOfRowsAndColumns))]
#pragma warning disable CA1822
        public int GetNumberOfDigits_Using_Lookup(int numberOfRowsAndColumns)
#pragma warning restore CA1822
        {
            // This clearly does not work with negative numbers!
            return numberOfRowsAndColumns switch
            {
                < 10 => 1,
                < 100 => 2,
                < 1_000 => 3,
                < 10_000 => 4,
                < 100_000 => 5,
                < 1_000_000 => 6,
                _ => numberOfRowsAndColumns.ToString().Length
            };
        }
    }
}