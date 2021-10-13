using System;
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
        public int CountOfMines => _minedCells.Count;

        public bool IsMined(Cell cell)
        {
            if (!HasCell(cell)) throw CreateCellNotInMinefieldException();

            return _minedCells.Contains(cell);
        }

        // TODO Validate row in the same way
        public bool IsMined(int column, int row)
        {
            if (column < 0 || column >= Columns) throw CreateColumnOutOfRangeException(column);
            if (row < 0 || row >= Rows) throw CreateRowOutOfRangeException(row);
            
            return IsMined(new Cell(column, row));
        }

        public int GetHint(Cell cell) => GetAdjacentCells(cell).Count(IsMined);

        public int GetHint(int column, int row) => GetHint(new Cell(column, row));

        private Exception CreateCellNotInMinefieldException()
        {
            return new ArgumentException("The cell does not exist in the minefield.", "cell");
        }

        private Exception CreateColumnOutOfRangeException(int column)
        {
            return new ArgumentOutOfRangeException(nameof(column), column, "Value must be greater than or equal to zero, and less than the number of columns.");
        }
        
        private Exception CreateRowOutOfRangeException(int row)
        {
            return new ArgumentOutOfRangeException(nameof(row), row, "Value must be greater than or equal to zero, and less than the number of rows.");
        }

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

        private bool HasCell(int column, int row) => column >= 0
                                                     && column < Columns
                                                     && row >= 0
                                                     && row < Rows;
    }
}