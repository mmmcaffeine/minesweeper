using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dgt.Minesweeper.Engine
{
    public class Game
    {
        private readonly IMinefield _minefield;
        private readonly Dictionary<Location, Cell> _cells = new();

        public Game(IMinefield minefield)
        {
            _minefield = minefield ?? throw new ArgumentNullException(nameof(minefield));
            
            foreach (var location in _minefield)
            {
                _cells[location] = new Cell(location, _minefield.IsMined(location), _minefield.GetHint(location));
            }

            NumberOfCellsToReveal = _minefield.Size - _minefield.CountOfMines;
        }

        public IEnumerable<ColumnName> ColumnNames => _cells.Keys
            .Select(loc => loc.ColumnName)
            .Distinct()
            .OrderBy(x => x.Value);

        public int NumberOfColumns => _minefield.NumberOfColumns;

        public int NumberOfRows => _minefield.NumberOfRows;
        
        public int NumberOfCellsToReveal { get; private set; }

        public bool IsWon => NumberOfCellsToReveal == 0;
        
        public bool IsLost { get; private set; }

        public bool IsOver => IsLost || IsWon;

        public IEnumerable<Cell> GetColumn(string columnName) => GetColumn((ColumnName)columnName);

        public IEnumerable<Cell> GetColumn(int columnIndex) => GetColumn((ColumnName)columnIndex);

        public IEnumerable<Cell> GetColumn(ColumnName columnName)
        {
            if (NumberOfRows - columnName < 0) throw CreateColumnNameOutOfRangeException(columnName);
            
            return _cells.Values.Where(cell => cell.Location.ColumnName == columnName)
                .OrderBy(cell => cell.Location.RowIndex);
        }

        private Exception CreateColumnNameOutOfRangeException(ColumnName columnName)
        {
            var messageBuilder = new StringBuilder("Value must be a ColumnName that exists in the Game and IMinefield.");
            
            messageBuilder.Append($" Expected between \"{((ColumnName)1).Value}\" and \"{((ColumnName)NumberOfColumns).Value}\".");

            return new ArgumentOutOfRangeException(nameof(columnName), columnName, messageBuilder.ToString())
            {
                Data = { { nameof(NumberOfColumns), NumberOfColumns } }
            };
        }
        
        public IEnumerable<Cell> GetRow(int rowIndex)
        {
            if (rowIndex <= 0) throw new ArgumentOutOfRangeException(nameof(rowIndex), rowIndex, "Value must be a positive, non-zero integer.");
            if (rowIndex > NumberOfRows) throw CreateRowIndexTooHighException(rowIndex);
            
            return _cells.Values.Where(cell => cell.Location.RowIndex == rowIndex)
                .OrderBy(cell => cell.Location.ColumnIndex);
        }

        private Exception CreateRowIndexTooHighException(int rowIndex)
        {
            var message = $"Value must be less than the number of rows. Expected less than {NumberOfRows}.";

            return new ArgumentOutOfRangeException(nameof(rowIndex), rowIndex, message)
            {
                Data = { { "NumberOfRows", NumberOfRows } }
            };
        }

        public Cell GetCell(string location) => GetCell(Location.Parse(location));

        public Cell GetCell(Location location)
        {
            if (location is null) throw new ArgumentNullException(nameof(location));
            if (!_minefield.Contains(location))
            {
                throw new InvalidLocationException(location, _minefield.NumberOfColumns, _minefield.NumberOfRows);
            }
            
            return _cells[location];
        }

        public Cell ToggleFlag(string location) => ToggleFlag(Location.Parse(location));

        public Cell ToggleFlag(Location location)
        {
            if (IsOver) throw new GameOverException(IsWon);
            
            var oldCell = GetCell(location);

            if (oldCell.IsRevealed) throw new InvalidMoveException(oldCell, "revealed", "A revealed Cell cannot be flagged");
            
            var newCell = oldCell with { IsFlagged = !oldCell.IsFlagged };

            _cells[location] = newCell;
            
            return newCell;
        }

        public Cell Reveal(string location) => Reveal(Location.Parse(location));
        
        public Cell Reveal(Location location)
        {
            if (IsOver) throw new GameOverException(IsWon);
            
            var oldCell = GetCell(location);
            
            if (oldCell.IsRevealed) throw new InvalidMoveException(oldCell, "revealed", "A revealed Cell cannot be revealed again");
            if (oldCell.IsFlagged) throw new InvalidMoveException(oldCell, "revealed", "A flagged Cell cannot be revealed");
            
            var newCell = oldCell with { IsRevealed = true };

            if (oldCell.IsMined)
            {
                IsLost = true;
            }
            else if (!oldCell.IsRevealed)
            {
                NumberOfCellsToReveal--;
            }
            
            _cells[location] = newCell;
            
            return newCell;
        }
    }
}