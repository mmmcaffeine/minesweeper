using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using Dgt.Minesweeper.Engine;

namespace Dgt.Minesweeper.Benchmarks
{
    [MemoryDiagnoser]
    public class MinefieldBenchmarks
    {
        public static IEnumerable<int> ValuesForNumberOfMines
        {
            get
            {
                yield return 2;
                yield return 10;
                yield return 100;
                yield return 1000;
            }
        }

        private readonly HashSet<Location> _minedLocations = new();

        private void PopulateMinefield(int numberOfMines)
        {
            for (var i = 1; i < numberOfMines; i++)
            {
                _minedLocations.Add(new Location((ColumnName)i, i));
            }
        }

        [ArgumentsSource(nameof(ValuesForNumberOfMines))]
        [Benchmark(Baseline = true)]
        public int GetHint_By_CountingAdjacentLocationsThatAreMined(int numberOfMines)
        {
            PopulateMinefield(numberOfMines);

            var location = Location.Parse("B1");
            var adjacentLocations = GetAdjacentLocations(location, numberOfMines);

            return adjacentLocations.Count(l => _minedLocations.Contains(l));
        }

        // Originally lifted from the implementation of Minefield, and tweaked to account for us not knowing the size
        // of the "minefield" until the specific benchmark runs. However, since switching Minefield to use Location
        // and thus ColumnName that implementation has now been split up. Location now knows whether two Locations
        // are adjacent. However, being able to convert a ColumnName into an integer (index) value is now in
        // ColumnName. Although no longer reflective of the actual implementation it is still a reasonable test
        private static IEnumerable<Location> GetAdjacentLocations(Location location, int numberOfMines)
        {
            var locationColumnIndex = (int)location.ColumnName;
            
            for (var columnIndex = locationColumnIndex - 1; columnIndex <= locationColumnIndex + 1; columnIndex++)
            {
                for (var rowIndex = location.RowIndex - 1; rowIndex <= location.RowIndex + 1; rowIndex++)
                {
                    if (!HasLocation(columnIndex, rowIndex, numberOfMines))
                    {
                        continue;
                    }
                    
                    var currentColumnName = (ColumnName)columnIndex;
                    var currentLocation = new Location(currentColumnName, rowIndex);
                    
                    yield return currentLocation;
                }
            }
        }
        
        // Lifted from the implementation of Minefield
        private static bool HasLocation(int columnIndex, int rowIndex, int numberOfMines)
        {
            return columnIndex > 0
                   && columnIndex <= numberOfMines
                   && rowIndex > 0
                   && rowIndex <= numberOfMines;
        }

        [ArgumentsSource(nameof(ValuesForNumberOfMines))]
        [Benchmark]
        public int GetHint_By_CountingMinedLocationsThatAreAdjacent(int numberOfMines)
        {
            PopulateMinefield(numberOfMines);

            var location = Location.Parse("B1");
            
            return _minedLocations.Count(l => l.IsAdjacentTo(location));
        }
    }
}