using System.Collections.Generic;
using System.Linq;

namespace Dgt.Minesweeper.Engine
{
    public class Minefield
    {
        private readonly HashSet<Cell> _minedCells;

        public Minefield(int columns, int rows, IEnumerable<Cell> minedCells)
        {
            Columns = columns;
            Rows = rows;
            _minedCells = new HashSet<Cell>(minedCells);
        }

        public int Columns { get; }
        public int Rows { get; }
        
        public bool IsMined(int column, int row)
        {
            return _minedCells.Contains(new Cell(column, row));
        }
        
        public int GetHint(int column, int row)
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

            return adjacentCells.Count(cell => HasCell(cell.Column, cell.Row) && IsMined(cell.Column, cell.Row));
        }
        
        private bool HasCell(int column, int row) => column <= Columns && row <= Rows;
    }
}