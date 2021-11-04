using System.Collections.Generic;
using System.Linq;

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
        }

        // TODO Do we want to improve performance by storing a value here, rather than calculating it every time?
        public bool IsWon => _cells.Values.All(cell => cell.IsRevealed || cell.IsMined);
        
        // TODO Do we want to improve performance by storing a value here, rather than calculating it every time?
        public bool IsLost => _cells.Values.Any(cell => cell.IsExploded);

        // TODO Validate the cell exists in our dictionary of cells
        public Cell GetCell(Location location) => _cells[location];
        
        // TODO Validate the location exists in our dictionary of cells
        public Cell Reveal(Location location)
        {
            var oldCell = _cells[location];
            var newCell = oldCell with { IsRevealed = true };
            
            _cells[location] = newCell;
            
            return newCell;
        }
    }
}