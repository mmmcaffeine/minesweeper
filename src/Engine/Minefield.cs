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

        public bool IsMined(Cell cell) => _minedCells.Contains(cell);

        public bool IsMined(int column, int row) => IsMined(new Cell(column, row));

        public int GetHint(Cell cell) => GetAdjacentCells(cell).Count(IsMined);

        public int GetHint(int column, int row) => GetHint(new Cell(column, row));
        
        private IEnumerable<Cell> GetAdjacentCells(Cell cell)
        {
            for (var column = cell.Column - 1; column <= cell.Column + 1; column++)
            {
                for (var row = cell.Row - 1; row <= cell.Row + 1; row++)
                {
                    var currentCell = new Cell(column, row);
                    
                    if (HasCell(currentCell) && currentCell != cell)
                    {
                        yield return new Cell(column, row);
                    }
                }
            }
        }

        private bool HasCell(Cell cell) => HasCell(cell.Column, cell.Row);

        private bool HasCell(int column, int row) => column <= Columns && row <= Rows;
    }
}