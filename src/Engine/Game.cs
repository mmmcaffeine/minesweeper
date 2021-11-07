using System;
using System.Collections.Generic;

namespace Dgt.Minesweeper.Engine
{
    public class Game
    {
        private readonly IMinefield _minefield;
        private readonly Dictionary<Location, Cell> _cells = new();

        // TODO Validate no nulls (even though we have enabled Nullable Types)
        public Game(IMinefield minefield)
        {
            _minefield = minefield;
            
            foreach (var location in _minefield)
            {
                _cells[location] = new Cell(location, _minefield.IsMined(location), _minefield.GetHint(location));
            }

            NumberOfCellsToReveal = _minefield.Size - _minefield.CountOfMines;
        }
        
        public int NumberOfCellsToReveal { get; private set; }

        public bool IsWon => NumberOfCellsToReveal == 0;
        
        public bool IsLost { get; private set; }

        // TODO Validate the cell exists in our dictionary of cells
        public Cell GetCell(Location location) => _cells[location];

        // TODO You cannot toggle flag on an exploded cell
        // TODO You cannot toggle flag on a revealed cell
        // TODO You cannot toggle flag if the game is won or lost
        public Cell ToggleFlag(Location location)
        {
            if (location is null) throw new ArgumentNullException(nameof(location));
            if (!_minefield.Contains(location))
            {
                throw new InvalidLocationException(location, _minefield.NumberOfColumns, _minefield.NumberOfRows);
            }
            
            var oldCell = _cells[location];
            var newCell = oldCell with { IsFlagged = !oldCell.IsFlagged };

            _cells[location] = newCell;
            
            return newCell;
        }
        
        // TODO You cannot reveal an exploded cell
        // TODO You cannot reveal a revealed cell
        // TODO You cannot reveal a flagged cell
        // TODO You cannot reveal if the game is won or lost 
        public Cell Reveal(Location location)
        {
            if (location is null) throw new ArgumentNullException(nameof(location));
            if (!_minefield.Contains(location))
            {
                throw new InvalidLocationException(location, _minefield.NumberOfColumns, _minefield.NumberOfRows);
            }
            
            var oldCell = _cells[location];
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