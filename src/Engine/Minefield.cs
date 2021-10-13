using System.Collections.Generic;
using System.Linq;

namespace Dgt.Minesweeper.Engine
{
    public class Minefield
    {
        private readonly HashSet<Cell> _minedCells;

        public Minefield(int rows, int columns, IEnumerable<Cell> minedCells)
        {
            Rows = rows;
            Columns = columns;
            _minedCells = new HashSet<Cell>(minedCells);
        }

        public int Rows { get; }
        public int Columns { get; }
        
        public bool IsMined(int row, int column)
        {
            return _minedCells.Contains(new Cell(row, column));
        }
        
        public int GetHint(int row, int column)
        {
            List<Cell> adjacentCells = new()
            {
                new Cell(row + 1, column - 1),
                new Cell(row + 1, column),
                new Cell(row + 1, column + 1),
                new Cell(row, column - 1),
                new Cell(row, column + 1),
                new Cell(row - 1, column + 1),
                new Cell(row - 1, column),
                new Cell(row - 1, column - 1)
            };

            return adjacentCells.Count(cell => HasCell(cell.Row, cell.Column) && IsMined(cell.Row, cell.Column));
        }
        
        private bool HasCell(int row, int column) => row <= Rows && column <= Columns;
    }
}