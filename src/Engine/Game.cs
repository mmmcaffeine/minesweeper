using System;
using System.Collections.Generic;

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
        
        public int NumberOfCellsToReveal { get; private set; }

        public bool IsWon => NumberOfCellsToReveal == 0;
        
        public bool IsLost { get; private set; }

        public bool IsOver => IsLost || IsWon;

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