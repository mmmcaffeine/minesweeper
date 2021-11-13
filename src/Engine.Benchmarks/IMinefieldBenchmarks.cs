using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace Dgt.Minesweeper.Engine
{
    // ReSharper disable once InconsistentNaming
    // Resharper isn't keen on the name of this test fixture, and I'm not either. It sounds like it should be an
    // interface. However, the benchmarks in this type are for default implementations on the IMinefield interface
    [MemoryDiagnoser]
    public class IMinefieldBenchmarks
    {
        private static class BenchmarkCategories
        {
            public const string Contains = "Contains";
            public const string Size = "Size";
            public const string Linq = "LINQ";
            public const string DefaultInterfaceImplementation = "Default Interface Implementation";
        }
        
        private class BenchmarkMinefield : IMinefield
        {
            public BenchmarkMinefield(int numberOfColumnsAndRows)
            {
                NumberOfColumns = numberOfColumnsAndRows;
                NumberOfRows = numberOfColumnsAndRows;
            }

            public int NumberOfColumns { get; }
            public int NumberOfRows { get; }
            public int CountOfMines => 0;
            public bool IsMined(Location location) => throw new NotSupportedException();
            public int GetHint(Location location) => throw new NotSupportedException();
            public IEnumerable<Location> GetAdjacentLocations(Location location) => throw new NotSupportedException();
            public IEnumerable<Location> GetMinedLocations() => throw new NotSupportedException();
        }
        
        public enum LocationPosition
        {
            BottomLeft,
            Middle,
            TopRight
        }

        private IMinefield _minefield = default!;
        private Location _location = default!;
        
        // ReSharper disable MemberCanBePrivate.Global
        // ReSharper disable UnusedAutoPropertyAccessor.Global
        // BenchmarkDotNet requires public read / write properties for us to use the Params attributes
        
        [Params(10, 100, 1000)]
        public int NumberOfRowsAndColumns { get; set; }
        
        [ParamsAllValues]
        public LocationPosition Position { get; set; }
        
        // ReSharper restore UnusedAutoPropertyAccessor.Global
        // ReSharper restore MemberCanBePrivate.Global
        
        [GlobalSetup]
        public void GlobalSetup()
        {
            _minefield = new BenchmarkMinefield(NumberOfRowsAndColumns);
            _location = GetLocation();
        }

        [Benchmark(Baseline = true)]
        [BenchmarkCategory(BenchmarkCategories.Contains, BenchmarkCategories.Linq)]
        public bool ContainsUsingLinq()
        {
            // Cast is required to ensure we get the Linq implementation, not the interface implementation
            return ((BenchmarkMinefield)_minefield).Contains(_location);
        }
        
        [Benchmark]
        [BenchmarkCategory(BenchmarkCategories.Contains, BenchmarkCategories.DefaultInterfaceImplementation)]
        public bool ContainsUsingDefaultInterfaceImplementation()
        {
            return _minefield.Contains(_location);
        }

        private Location GetLocation()
        {
            var index = Position switch
            {
                LocationPosition.BottomLeft => 1,
                LocationPosition.Middle => Math.DivRem(NumberOfRowsAndColumns, 2, out _),
                LocationPosition.TopRight => NumberOfRowsAndColumns,
                _ => throw new InvalidOperationException()
            };
            
            return new Location(index, index);
        }

        [Benchmark(Baseline = true)]
        [BenchmarkCategory(BenchmarkCategories.Size, BenchmarkCategories.Linq)]
        public int CountUsingLinq()
        {
            return _minefield.Count();
        }

        [Benchmark]
        [BenchmarkCategory(BenchmarkCategories.Size, BenchmarkCategories.DefaultInterfaceImplementation)]
        public int SizeUsingDefaultInterfaceImplementation()
        {
            return _minefield.Size;
        }
    }
}