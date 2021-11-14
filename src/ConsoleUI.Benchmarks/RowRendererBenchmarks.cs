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

            public const string CharArray = "CharArray";
            public const string JoinedEnumerableOfString = "JoinedEnumerable";

            public const string RenderTopBorder = "RenderTopBorder";
            public const string RenderRow = "RenderRow";
            public const string GetNumberOfDigits = "GetNumberOfDigits";
        }

        private const int RowIndex = 1;
        
        private IRowRenderer _naiveRowRenderer = default!;
        private IRowRenderer _efficientGameRenderer = default!;
        private ICellRenderer _cellRenderer = default!;
        private List<Cell> _cells = default!;

        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        // BenchmarkDotNet requires public read / write properties for us to use the Params attributes
        [Params(2, 10, 100, 1000)]
        public int NumberOfRowsAndColumns { get; set; }

        [GlobalSetup]
        public void GlobalSetup()
        {
            _naiveRowRenderer = new NaiveRowRenderer();
            _efficientGameRenderer = new EfficientGameRenderer();
            _cellRenderer = new CellRenderer();
            _cells = CreateCells().ToList();
        }

        private IEnumerable<Cell> CreateCells()
        {
            for (var i = 0; i < NumberOfRowsAndColumns; i++)
            {
                var location = new Location(i + 1, 1);

                yield return new Cell(location, false, 0);
            }
        }

        [Benchmark(Baseline = true)]
        [BenchmarkCategory(BenchmarkCategories.RenderTopBorder, BenchmarkCategories.NaiveRowRenderer)]
        public string RenderTopBorder_Using_NaiveRowRenderer() =>
            _naiveRowRenderer.RenderTopBorder(NumberOfRowsAndColumns, NumberOfRowsAndColumns);

        [Benchmark]
        [BenchmarkCategory(BenchmarkCategories.RenderTopBorder, BenchmarkCategories.EfficientGameRenderer)]
        public string RenderTopBorder_Using_EfficientGameRenderer() =>
            _efficientGameRenderer.RenderTopBorder(NumberOfRowsAndColumns, NumberOfRowsAndColumns);

        [Benchmark]
        [BenchmarkCategory(BenchmarkCategories.RenderTopBorder, BenchmarkCategories.CharArray)]
        public string RenderTopBorder_Using_CharArray()
        {
            var prefixLength = NumberOfRowsAndColumns.ToString().Length;
            var totalLength = prefixLength + 1 + NumberOfRowsAndColumns * 2 + 1;
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
        public string RenderTopBorder_Using_JoinedEnumerableOfString()
        {
            var prefixLength = NumberOfRowsAndColumns.ToString().Length;
            var columns = Enumerable.Range(0, NumberOfRowsAndColumns - 1).Select(_ => "═╦");
            var prefix = Enumerable.Range(0, prefixLength).Select(_ => ' ');

            return $"{string.Join(null, prefix)} ╔{string.Join(null, columns)}═╗";
        }

        [Benchmark(Baseline = true)]
        [BenchmarkCategory(BenchmarkCategories.RenderRow, BenchmarkCategories.NaiveRowRenderer)]
        public string RenderRow_Using_NaiveRowRenderer() =>
            _naiveRowRenderer.RenderRow(NumberOfRowsAndColumns, RowIndex, _cellRenderer, _cells);

        [Benchmark]
        [BenchmarkCategory(BenchmarkCategories.RenderRow, BenchmarkCategories.EfficientGameRenderer)]
        public string RenderRow_Using_EfficientGameRenderer() =>
            _efficientGameRenderer.RenderRow(NumberOfRowsAndColumns, RowIndex, _cellRenderer, _cells);

        [Benchmark]
        [BenchmarkCategory(BenchmarkCategories.RenderRow, BenchmarkCategories.CharArray)]
        public string RenderRow_Using_CharArray()
        {
            const int rowIndex = 1;
            var rowHeaderLength = NumberOfRowsAndColumns switch
            {
                < 10 => 1,
                < 100 => 2,
                < 1_000 => 3,
                _ => NumberOfRowsAndColumns.ToString().Length
            };
            var totalLength = rowHeaderLength + 1 + _cells.Count * 2 + 1;
            var chars = new char[totalLength];
            var rowIndexChars = rowIndex.ToString().ToCharArray();

            for (var i = 0; i < rowHeaderLength; i++)
            {
                chars[i] = rowHeaderLength - i <= rowIndexChars.Length
                    ? rowIndexChars[i - rowHeaderLength + rowIndexChars.Length]
                    : ' ';
            }

            chars[rowHeaderLength] = ' ';

            for (var i = 0; i < _cells.Count; i++)
            {
                chars[rowHeaderLength + 1 + (i * 2)] = '║';
                chars[rowHeaderLength + 2 + (i * 2)] = _cellRenderer.RenderCell(_cells[i]);
            }

            chars[^1] = '║';

            return new string(chars);
        }

        [Benchmark]
        [BenchmarkCategory(BenchmarkCategories.GetNumberOfDigits)]
        public int GetNumberOfDigits_Using_StringLength() => NumberOfRowsAndColumns.ToString().Length;

        [Benchmark]
        [BenchmarkCategory(BenchmarkCategories.GetNumberOfDigits)]
        public int GetNumberOfDigits_Using_Lookup()
        {
            // This clearly does not work with negative numbers!
            return NumberOfRowsAndColumns switch
            {
                < 10 => 1,
                < 100 => 2,
                < 1_000 => 3,
                < 10_000 => 4,
                < 100_000 => 5,
                < 1_000_000 => 6,
                _ => NumberOfRowsAndColumns.ToString().Length
            };
        }
    }
}