using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using Dgt.Minesweeper.Engine;

namespace Dgt.Minesweeper.Benchmarks
{
    [MemoryDiagnoser]
    public class GameIsWonBenchmarks
    {
        // BenchmarkDotNet requires public read / write properties for us to use the Params or
        // ParamsAllValues attributes
        
        // ReSharper disable MemberCanBePrivate.Global
        // ReSharper disable UnusedAutoPropertyAccessor.Global
        
        [Params(10, 16, 30, 100)]
        public int NumberOfRowsAndColumns { get; set; }
        
        [Params(1, 40, 99)]
        public int NumberOfMines { get; set; }
        
        [ParamsAllValues]
        public bool IsWon { get; set; }
        
        // ReSharper restore UnusedAutoPropertyAccessor.Global
        // ReSharper restore MemberCanBePrivate.Global

        private IMinefield _minefield = default!;
        private Game _game = default!;
        private Dictionary<Location, Cell> _cells = default!;

        [GlobalSetup]
        public void GlobalSetUp()
        {
            var minedLocations = GetMinedLocations(NumberOfMines, NumberOfRowsAndColumns).ToList();
            
            _minefield = new Minefield(NumberOfRowsAndColumns, NumberOfRowsAndColumns, minedLocations);
            _game = new Game(_minefield);
            _cells = new Dictionary<Location, Cell>();

            foreach (var location in _minefield)
            {
                var isMined = minedLocations.Contains(location);
                var isRevealed = !isMined && IsWon;

                if (isRevealed)
                {
                    _game.Reveal(location);
                }

                _cells[location] = new Cell(location, isMined, _minefield.GetHint(location))
                {
                    IsRevealed = isRevealed
                };
            }
        }

        private static IEnumerable<Location> GetMinedLocations(int numberOfMines, int numberOfRowsAndColumns)
        {
            var columnIndex = 0;
            var rowIndex = 1;
            var minesReturned = 0;

            while (minesReturned < numberOfMines)
            {
                columnIndex++;

                if (columnIndex > numberOfRowsAndColumns)
                {
                    rowIndex++;
                    columnIndex = 1;
                }

                if (rowIndex > numberOfRowsAndColumns)
                {
                    throw new InvalidOperationException("There are too many mines to place on the minefield");
                }

                yield return new Location((ColumnName)columnIndex, rowIndex);

                minesReturned++;
            }
        }

        [Benchmark(Baseline = true)]
        public bool IsWon_By_CheckingValuesForIsRevealedAndIsMined()
        {
            return _cells.Values.All(cell => cell.IsRevealed || cell.IsMined);
        }

        [Benchmark]
        public bool IsWon_By_CheckingDictionaryEntriesForIsRevealedAndIsMined()
        {
            return _cells.All(kvPair => kvPair.Value.IsRevealed || kvPair.Value.IsMined);
        }

        [Benchmark]
        public bool IsWon_By_CountingNumberOfCellsToReveal() => _game.IsWon;
    }
}