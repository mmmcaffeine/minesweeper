using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using Dgt.Minesweeper.Engine;

namespace Dgt.Minesweeper.Benchmarks
{
    [MemoryDiagnoser]
    public class MinefieldBenchmarks
    {
        private static readonly HashSet<Cell> MinedCells = new();

        private static void PopulateMinefield()
        {
            MinedCells.Clear();
            
            for (int column = 0; column < 50; column++)
            {
                for (int row = 0; row < 50; row++)
                {
                    MinedCells.Add(new Cell(column, row));
                }
            }
        }

        [Benchmark(Baseline = true)]
#pragma warning disable CA1822
        public int GetHint_By_CountingAdjacentCellsThatAreMined()
#pragma warning restore CA1822
        {
            PopulateMinefield();
            
            return GetAdjacentCells(new Cell(1, 0)).Count(cell => MinedCells.Contains(cell));
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

        [Benchmark]
#pragma warning disable CA1822
        public int GetHint_ByCountingMinedCellsThatAreAdjacent()
#pragma warning restore CA1822
        {
            PopulateMinefield();
            
            return MinedCells.Count(cell => cell.IsAdjacentTo(new Cell(1, 0)));
        }
    }
}