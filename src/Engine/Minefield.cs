using System;
using System.Collections.Generic;
using System.Linq;

namespace Dgt.Minesweeper.Engine
{
    public class Minefield : IMinefield
    {
        private readonly HashSet<Cell> _minedCells;

        // TODO Validate positive non-zero integers (and maybe more than 1 makes sense?
        // TODO Validate non-null enumerable for mined cells
        public Minefield(int numberOfColumns, int numberOfRows, IEnumerable<Cell> minedCells)
        {
            NumberOfColumns = numberOfColumns;
            NumberOfRows = numberOfRows;
            _minedCells = new HashSet<Cell>(minedCells);
        }

        public int NumberOfColumns { get; }
        public int NumberOfRows { get; }
        public int CountOfMines => _minedCells.Count;

        public bool IsMined(Cell cell)
        {
            if (!HasCell(cell)) throw CreateCellNotInMinefieldException(nameof(cell));

            return _minedCells.Contains(cell);
        }

        // TODO Validate row in the same way
        public bool IsMined(int columnIndex, int rowIndex)
        {
            if (columnIndex < 0 || columnIndex >= NumberOfColumns) throw CreateColumnOutOfRangeException(columnIndex);
            if (rowIndex < 0 || rowIndex >= NumberOfRows) throw CreateRowOutOfRangeException(rowIndex);
            
            return IsMined(new Cell(columnIndex, rowIndex));
        }

        public int GetHint(Cell cell) => _minedCells.Count(c => c.IsAdjacentTo(cell));

        public int GetHint(int columnIndex, int rowIndex) => GetHint(new Cell(columnIndex, rowIndex));

        private static Exception CreateCellNotInMinefieldException(string paramName)
        {
            return new ArgumentException("The cell does not exist in the minefield.", paramName);
        }

        private static Exception CreateColumnOutOfRangeException(int columnIndex)
        {
            return new ArgumentOutOfRangeException(nameof(columnIndex), columnIndex, "Value must be greater than or equal to zero, and less than the number of columns.");
        }
        
        private static Exception CreateRowOutOfRangeException(int rowIndex)
        {
            return new ArgumentOutOfRangeException(nameof(rowIndex), rowIndex, "Value must be greater than or equal to zero, and less than the number of rows.");
        }

        private bool HasCell(Cell cell) => HasCell(cell.ColumnIndex, cell.RowIndex);

        private bool HasCell(int columnIndex, int rowIndex) => columnIndex >= 0
                                                     && columnIndex < NumberOfColumns
                                                     && rowIndex >= 0
                                                     && rowIndex < NumberOfRows;
    }
}