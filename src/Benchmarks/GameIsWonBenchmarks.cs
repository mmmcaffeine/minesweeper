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
        private Dictionary<Cell, CellState> _cellStates =default!;

        [GlobalSetup]
        public void GlobalSetUp()
        {
            var minedCells = GetMinedCells(NumberOfMines, NumberOfRowsAndColumns).ToList();
            
            _minefield = new Minefield(NumberOfRowsAndColumns, NumberOfRowsAndColumns, minedCells);
            _cellStates = new Dictionary<Cell, CellState>();

            foreach (var cell in _minefield)
            {
                var cellState = (minedCells.Contains(cell), IsWon) switch
                {
                    (true, _) => CellState.Mined,
                    (_, true) => CellState.Cleared,
                    (_, false) => CellState.Uncleared
                };

                _cellStates[cell] = cellState;
            }
        }

        private static IEnumerable<Cell> GetMinedCells(int numberOfMines, int numberOfRowsAndColumns)
        {
            var columnIndex = -1;
            var rowIndex = 0;
            var minesReturned = 0;

            while (minesReturned < numberOfMines)
            {
                columnIndex++;

                if (columnIndex >= numberOfRowsAndColumns)
                {
                    rowIndex++;
                    columnIndex = 0;
                }

                if (rowIndex >= numberOfRowsAndColumns)
                {
                    throw new InvalidOperationException("There are too many mines to place on the minefield");
                }

                yield return new Cell(columnIndex, rowIndex);

                minesReturned++;
            }
        }

        [Benchmark(Baseline = true)]
        public bool IsWon_By_FilteringMinesThenCheckingCellState()
        {
            return _cellStates.Keys
                .Where(cell => !_minefield.IsMined(cell))
                .All(cell => _cellStates[cell] == CellState.Cleared);
        }

        [Benchmark]
        public bool IsWon_By_CheckingCellStateThenCheckingMinedStatus()
        {
            // Test the cell state _first_ because it is the faster check
            return _cellStates.All(kvPair => kvPair.Value == CellState.Cleared || !_minefield.IsMined(kvPair.Key));
        }
    }
}