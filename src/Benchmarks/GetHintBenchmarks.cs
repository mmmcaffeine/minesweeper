using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using Dgt.Minesweeper.Engine;

namespace Dgt.Minesweeper.Benchmarks
{
    [MemoryDiagnoser]
    public class GetHintBenchmarks
    {
        private HashSet<Location> _minedLocations = default!;
        private IMinefield _minefield = default!;
        
        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        // BenchmarkDotNet requires public read / write properties for us to use the Params attributes
        [Params(2, 10, 100, 1000)]
        public int NumberOfMines { get; set; }

        [GlobalSetup]
        public void GlobalSetup()
        {
            _minedLocations = new HashSet<Location>();
            
            PopulateMinedLocations();
            
            _minefield = new Minefield
            (
                _minedLocations.Max(x => x.ColumnIndex),
                _minedLocations.Max(x => x.RowIndex),
                _minedLocations
            );
        }

        private void PopulateMinedLocations()
        {
            for (var i = 1; i <= NumberOfMines; i++)
            {
                _minedLocations.Add(new Location((ColumnName)i, i));
            }
        }

        [Benchmark]
        public int GetHint_By_CountingAdjacentLocationsThatAreMined()
        {
            var location = Location.Parse("B1");
            var adjacentLocations = GetAdjacentLocations(location);

            return adjacentLocations.Count(l => _minedLocations.Contains(l));
        }

        // Originally lifted from the implementation of Minefield, and tweaked to account for us not knowing the size
        // of the "minefield" until the specific benchmark runs. However, since switching Minefield to use Location
        // and thus ColumnName that implementation has now been split up. Location now knows whether two Locations
        // are adjacent. However, being able to convert a ColumnName into an integer (index) value is now in
        // ColumnName. Although no longer reflective of the actual implementation it is still a reasonable test
        private IEnumerable<Location> GetAdjacentLocations(Location location)
        {
            var locationColumnIndex = (int)location.ColumnName;
            
            for (var columnIndex = locationColumnIndex - 1; columnIndex <= locationColumnIndex + 1; columnIndex++)
            {
                for (var rowIndex = location.RowIndex - 1; rowIndex <= location.RowIndex + 1; rowIndex++)
                {
                    if (!HasLocation(columnIndex, rowIndex))
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
        private bool HasLocation(int columnIndex, int rowIndex)
        {
            return columnIndex > 0
                   && columnIndex <= NumberOfMines
                   && rowIndex > 0
                   && rowIndex <= NumberOfMines;
        }

        [Benchmark]
        public int GetHint_By_CountingMinedLocationsThatAreAdjacent()
        {
            var location = Location.Parse("B1");
            
            return _minedLocations.Count(l => l.IsAdjacentTo(location));
        }

        [Benchmark(Baseline = true)]
        public int GetHint_By_UsingMinefield() => _minefield.GetHint(Location.Parse("B1"));
    }
}