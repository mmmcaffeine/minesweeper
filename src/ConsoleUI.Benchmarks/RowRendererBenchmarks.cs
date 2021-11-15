using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using Dgt.Minesweeper.Engine;

namespace Dgt.Minesweeper.ConsoleUI
{
    [MemoryDiagnoser]
    public class RowRendererBenchmarks
    {
        private static class BenchmarkCategories
        {
            public const string NaiveRowRenderer = "NaiveRowRenderer";
            public const string StringCreateGameRenderer = "StringCreateGameRenderer";
            public const string CharArrayGameRenderer = "CharArrayGameRenderer";

            public const string CharArray = "CharArray";
            public const string JoinedEnumerableOfString = "JoinedEnumerable";

            public const string RenderTopBorder = "RenderTopBorder";
            public const string RenderRow = "RenderRow";
            public const string RenderColumnNames = "RenderColumnNames";
            public const string GetNumberOfDigits = "GetNumberOfDigits";
        }

        private const int RowIndex = 1;

        private Consumer _consumer = default!;
        private IRowRenderer _naiveRowRenderer = default!;
        private IRowRenderer _stringCreateGameRenderer = default!;
        private IRowRenderer _charArrayGameRenderer = default!;
        private ICellRenderer _cellRenderer = default!;
        private List<Cell> _cells = default!;
        private List<ColumnName> _columnNames = default!;

        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        // BenchmarkDotNet requires public read / write properties for us to use the Params attributes
        [Params(2, 10, 100, 1000)]
        public int NumberOfRowsAndColumns { get; set; }

        [GlobalSetup]
        public void GlobalSetup()
        {
            _consumer = new Consumer();
            _naiveRowRenderer = new NaiveRowRenderer();
            _stringCreateGameRenderer = new StringCreateGameRenderer();
            _charArrayGameRenderer = new CharArrayGameRenderer();
            _cellRenderer = new CellRenderer();
            _cells = CreateCells().ToList();
            _columnNames = _cells.Select(cell => cell.Location.ColumnName).ToList();
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
        [BenchmarkCategory(BenchmarkCategories.RenderTopBorder, BenchmarkCategories.StringCreateGameRenderer)]
        public string RenderTopBorder_Using_StringCreateGameRenderer() =>
            _stringCreateGameRenderer.RenderTopBorder(NumberOfRowsAndColumns, NumberOfRowsAndColumns);

        [Benchmark]
        [BenchmarkCategory(BenchmarkCategories.RenderTopBorder, BenchmarkCategories.CharArrayGameRenderer)]
        public string RenderTopBorder_Using_CharArrayGameRenderer() =>
            _charArrayGameRenderer.RenderTopBorder(NumberOfRowsAndColumns, NumberOfRowsAndColumns);

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
        [BenchmarkCategory(BenchmarkCategories.RenderRow, BenchmarkCategories.StringCreateGameRenderer)]
        public string RenderRow_Using_StringCreateGameRenderer() =>
            _stringCreateGameRenderer.RenderRow(NumberOfRowsAndColumns, RowIndex, _cellRenderer, _cells);

        [Benchmark]
        [BenchmarkCategory(BenchmarkCategories.RenderRow, BenchmarkCategories.CharArrayGameRenderer)]
        public string RenderRow_Using_CharArrayGameRenderer() =>
            _charArrayGameRenderer.RenderRow(NumberOfRowsAndColumns, RowIndex, _cellRenderer, _cells);

        [Benchmark(Baseline = true)]
        [BenchmarkCategory(BenchmarkCategories.RenderColumnNames, BenchmarkCategories.NaiveRowRenderer)]
        public void RenderColumnNames_Using_NaiveRowRenderer() =>
            _naiveRowRenderer.RenderColumnNames(NumberOfRowsAndColumns, _columnNames).Consume(_consumer);

        [Benchmark]
        [BenchmarkCategory(BenchmarkCategories.RenderColumnNames, BenchmarkCategories.StringCreateGameRenderer)]
        public void RenderColumnNames_Using_StringCreateGameRenderer() =>
            _stringCreateGameRenderer.RenderColumnNames(NumberOfRowsAndColumns, _columnNames).Consume(_consumer);

        [Benchmark]
        [BenchmarkCategory(BenchmarkCategories.RenderColumnNames, BenchmarkCategories.StringCreateGameRenderer)]
        public void RenderColumnNames_Using_CharArrayGameRenderer() =>
            _charArrayGameRenderer.RenderColumnNames(NumberOfRowsAndColumns, _columnNames).Consume(_consumer);

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