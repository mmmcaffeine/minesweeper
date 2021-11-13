using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using Dgt.Minesweeper.Engine;

namespace Dgt.Minesweeper.Benchmarks
{
    [MemoryDiagnoser]
    public class MinefieldBenchmarks
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
        public void GetLocations_Using_NestedForLoop(int numberOfRowsAndColumns)
#pragma warning restore CA1822
        {
            DoGetLocationsUsingNestedForLoop(numberOfRowsAndColumns).Consume(Consumer);
        }

        private static IEnumerable<Location> DoGetLocationsUsingNestedForLoop(int numberOfRowsAndColumns)
        {
            for (var columnIndex = 1; columnIndex <= numberOfRowsAndColumns; columnIndex++)
            {
                for (var rowIndex = 1; rowIndex <= numberOfRowsAndColumns; rowIndex++)
                {
                    yield return new Location((ColumnName)columnIndex, rowIndex);
                }
            }
        }

        [ArgumentsSource(nameof(ValuesForNumberOfRowsAndColumns))]
        [Benchmark]
#pragma warning disable CA1822
        public void GetLocations_Using_NestedForEachLoop(int numberOfRowsAndColumns)
#pragma warning restore CA1822
        {
            DoGetLocationsUsingNestedForEachLoop(numberOfRowsAndColumns).Consume(Consumer);
        }

        private static IEnumerable<Location> DoGetLocationsUsingNestedForEachLoop(int numberOfRowsAndColumns)
        {
            var columnIndices = Enumerable.Range(1, numberOfRowsAndColumns);
            var rowIndices = Enumerable.Range(1, numberOfRowsAndColumns).ToList();

            foreach (var columnIndex in columnIndices)
            {
                foreach (var rowIndex in rowIndices)
                {
                    yield return new Location((ColumnName)columnIndex, rowIndex);
                }
            }
        }

        [ArgumentsSource(nameof(ValuesForNumberOfRowsAndColumns))]
        [Benchmark]
#pragma warning disable CA1822
        public void GetLocations_Using_SelectMany(int numberOfRowsAndColumns)
#pragma warning restore CA1822
        {
            var columnIndices = Enumerable.Range(1, numberOfRowsAndColumns);
            var rowIndices = Enumerable.Range(1, numberOfRowsAndColumns);

            var locations = columnIndices.SelectMany(_ => rowIndices, (ci, ri) => new Location((ColumnName)ci, ri));
            
            locations.Consume(Consumer);
        }
        
        [ArgumentsSource(nameof(ValuesForNumberOfRowsAndColumns))]
        [Benchmark]
#pragma warning disable CA1822
        public void GetLocations_Using_QueryExpression(int numberOfRowsAndColumns)
#pragma warning restore CA1822
        {
            var columnIndices = Enumerable.Range(1, numberOfRowsAndColumns);
            var rowIndices = Enumerable.Range(1, numberOfRowsAndColumns);

            var locations = from columnIndex in columnIndices
                            from rowIndex in rowIndices
                            select new Location((ColumnName)columnIndex, rowIndex);
            
            locations.Consume(Consumer);
        }
    }
}