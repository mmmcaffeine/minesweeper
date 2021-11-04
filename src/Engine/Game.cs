using System.Collections.Generic;

namespace Dgt.Minesweeper.Engine
{
    public class Game
    {
        private readonly Dictionary<Location, Cell> _cells = new();

        // TODO Validate no nulls (even though we have enabled Nullable Types)
        public Game(IMinefield minefield)
        {
            foreach (var location in minefield)
            {
                _cells[location] = new Cell(location, minefield.IsMined(location), false);
            }

            NumberOfCellsToReveal = minefield.Size - minefield.CountOfMines;
        }
        
        public int NumberOfCellsToReveal { get; private set; }

        public bool IsWon => NumberOfCellsToReveal == 0;
        
        public bool IsLost { get; private set; }

        // TODO Validate the cell exists in our dictionary of cells
        public Cell GetCell(Location location) => _cells[location];
        
        // TODO Validate the location exists in our dictionary of cells
        public Cell Reveal(Location location)
        {
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