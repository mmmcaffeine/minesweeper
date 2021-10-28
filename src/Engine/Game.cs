using System;
using System.Collections.Generic;
using System.Linq;

namespace Dgt.Minesweeper.Engine
{
    public class Game
    {
        private readonly IMinefield _minefield;
        private readonly Dictionary<Location, CellState> _cellStates = new();

        // TODO Validate no nulls (even though we have enabled Nullable Types)
        public Game(IMinefield minefield)
        {
            _minefield = minefield;
            
            foreach (var location in _minefield)
            {
                _cellStates[location] = _minefield.IsMined(location)
                    ? CellState.Mined
                    : CellState.Uncleared;
            }
        }

        // TODO Do we want to improve performance by storing a value here, rather than calculating it every time?
        // TODO If we calculate this can we improve this by embedding whether a CellState is for a mined or not mined cell?
        public bool IsWon => _cellStates.Keys
            .Where(location => !_minefield.IsMined(location))
            .All(location => _cellStates[location] == CellState.Cleared);
        
        // TODO Do we want to improve performance by storing a value here, rather than calculating it every time?
        public bool IsLost => _cellStates.Values.Any(state => state == CellState.Exploded);

        // TODO Validate the cell exists on the minefield / in our dictionary of cell states
        public CellState GetCellState(Location location) => _cellStates[location];

        // TODO Validate the location exists in the minefield / in our dictionary of cell states
        // At this point it is looking like we're not getting a whole lot of value by _not_ using an enum
        // if we _don't_ have different classes to represent the different states, then delegate to implementations
        // of e.g. Reveal on them. This switch is harder for people to grok than a switch on an enum
        public CellState Reveal(Location location)
        {
            var oldCellState = GetCellState(location);
            var newCellState = oldCellState switch
            {
                var s when s == CellState.Uncleared => CellState.Cleared,
                var s when s == CellState.Mined => CellState.Exploded,
                _ => throw new NotImplementedException()
            };

            return _cellStates[location] = newCellState;
        }
    }
}