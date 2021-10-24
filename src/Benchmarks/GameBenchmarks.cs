using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using Dgt.Minesweeper.Engine;

namespace Dgt.Minesweeper.Benchmarks
{
    [MemoryDiagnoser]
    public class GameBenchmarks
    {
        private static readonly Consumer Consumer = new();
        
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

        [ArgumentsSource(nameof(ValuesForNumberOfRowsAndColumns))]
        [Benchmark]
#pragma warning disable CA1822
        public void GetCells_Using_NestedForLoop(int numberOfRowsAndColumns)
#pragma warning restore CA1822
        {
            DoGetCellsUsingNestedForLoop(numberOfRowsAndColumns).Consume(Consumer);
        }

        private static IEnumerable<Cell> DoGetCellsUsingNestedForLoop(int numberOfRowsAndColumns)
        {
            for (var columnIndex = 0; columnIndex < numberOfRowsAndColumns; columnIndex++)
            {
                for (var rowIndex = 0; rowIndex < numberOfRowsAndColumns; rowIndex++)
                {
                    yield return new Cell(columnIndex, rowIndex);
                }
            }
        }

        [ArgumentsSource(nameof(ValuesForNumberOfRowsAndColumns))]
        [Benchmark]
#pragma warning disable CA1822
        public void GetCells_Using_NestedForEachLoop(int numberOfRowsAndColumns)
#pragma warning restore CA1822
        {
            DoGetCellsUsingNestedForEachLoop(numberOfRowsAndColumns).Consume(Consumer);
        }

        private static IEnumerable<Cell> DoGetCellsUsingNestedForEachLoop(int numberOfRowsAndColumns)
        {
            var columnIndices = Enumerable.Range(0, numberOfRowsAndColumns);
            var rowIndices = Enumerable.Range(0, numberOfRowsAndColumns).ToList();

            foreach (var columnIndex in columnIndices)
            {
                foreach (var rowIndex in rowIndices)
                {
                    yield return new Cell(columnIndex, rowIndex);
                }
            }
        }

        [ArgumentsSource(nameof(ValuesForNumberOfRowsAndColumns))]
        [Benchmark]
#pragma warning disable CA1822
        public void GetCells_Using_SelectMany(int numberOfRowsAndColumns)
#pragma warning restore CA1822
        {
            var columnIndices = Enumerable.Range(0, numberOfRowsAndColumns);
            var rowIndices = Enumerable.Range(0, numberOfRowsAndColumns);
            
            var cells = columnIndices.SelectMany(_ => rowIndices, (ci, ri) => new Cell(ci, ri));
            
            cells.Consume(Consumer);
        }
        
        [ArgumentsSource(nameof(ValuesForNumberOfRowsAndColumns))]
        [Benchmark]
#pragma warning disable CA1822
        public void GetCells_Using_QueryExpression(int numberOfRowsAndColumns)
#pragma warning restore CA1822
        {
            var columnIndices = Enumerable.Range(0, numberOfRowsAndColumns);
            var rowIndices = Enumerable.Range(0, numberOfRowsAndColumns);
            
            var cells = from columnIndex in columnIndices
                        from rowIndex in rowIndices
                        select new Cell(columnIndex, rowIndex);
            
            cells.Consume(Consumer);
        }
    }
}