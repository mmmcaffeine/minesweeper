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

        private readonly HashSet<Cell> _minedCells = new();

        private void PopulateMinefield(int numberOfMines)
        {
            for (var i = 0; i < numberOfMines; i++)
            {
                _minedCells.Add(new Cell(i, i));
            }
        }

        [ArgumentsSource(nameof(ValuesForNumberOfMines))]
        [Benchmark(Baseline = true)]
        public int GetHint_By_CountingAdjacentCellsThatAreMined(int numberOfMines)
        {
            PopulateMinefield(numberOfMines);
            
            return GetAdjacentCells(new Cell(1, 0)).Count(cell => _minedCells.Contains(cell));
        }

        // Lifted from the implementation of Minefield
        private static IEnumerable<Cell> GetAdjacentCells(Cell cell)
        {
            for (var column = cell.Column - 1; column <= cell.Column + 1; column++)
            {
                for (var row = cell.Row - 1; row <= cell.Row + 1; row++)
                {
                    var currentCell = new Cell(column, row);
                    
                    if (HasCell(currentCell.Column, currentCell.Row) && currentCell != cell)
                    {
                        yield return new Cell(column, row);
                    }
                }
            }
        }
        
        // Lifted from the implementation of Minefield, but with hard-wired values for Column and Row because
        // we don't have them
        private static bool HasCell(int column, int row) => column >= 0
                                                     && column < 4
                                                     && row >= 0
                                                     && row < 3;

        [ArgumentsSource(nameof(ValuesForNumberOfMines))]
        [Benchmark]
        public int GetHint_ByCountingMinedCellsThatAreAdjacent(int numberOfMines)
        {
            PopulateMinefield(numberOfMines);
            
            return _minedCells.Count(cell => cell.IsAdjacentTo(new Cell(1, 0)));
        }
    }
}