using System;
using System.Collections.Generic;
using System.Linq;

namespace Dgt.Minesweeper.Engine
{
    public class Game
    {
        private readonly IMinefield _minefield;
        private readonly Dictionary<Cell, CellState> _cellStates = new();

        // TODO Validate no nulls (even though we have enabled Nullable Types)
        public Game(IMinefield minefield)
        {
            _minefield = minefield;
            
            foreach (var cell in Cells)
            {
                _cellStates[cell] = _minefield.IsMined(cell)
                    ? CellState.Mined
                    : CellState.Uncleared;
            }
        }

        // TODO Do we want to improve performance by storing a value here, rather than calculating it every time?
        // TODO If we calculate this can we improve this by embedding whether a CellState is for a mined or not mined cell?
        public bool IsWon => _cellStates.Keys
            .Where(cell => !_minefield.IsMined(cell))
            .All(cell => _cellStates[cell] == CellState.Cleared);
        
        // TODO Do we want to improve performance by storing a value here, rather than calculating it every time?
        public bool IsLost => _cellStates.Values.Any(state => state == CellState.Exploded);

        // TODO Validate the cell exists on the minefield / in our dictionary of cell states
        public CellState GetCellState(Cell cell) => _cellStates[cell];

        // TODO Validate the cell exists on the minefield / in our dictionary of cell states
        // At this point it is looking like we're not getting a whole lot of value by _not_ using an enum
        // if we _don't_ have different classes to represent the different states, then delegate to implementations
        // of e.g. Reveal on them. This switch is harder for people to grok than a switch on an enum
        public CellState Reveal(Cell cell)
        {
            var oldCellState = GetCellState(cell);
            var newCellState = oldCellState switch
            {
                var s when s == CellState.Uncleared => CellState.Cleared,
                var s when s == CellState.Mined => CellState.Exploded,
                _ => throw new NotImplementedException()
            };

            return _cellStates[cell] = newCellState;
        }

        // There might be better ways to arrange this. It could be an extension method against IMinefield, or
        // we might want to make IMinefield implement IEnumerable<Cell>
        private IEnumerable<Cell> Cells
        {
            get
            {
                for (var columnIndex = 0; columnIndex < _minefield.NumberOfColumns; columnIndex++)
                {
                    for (var rowIndex = 0; rowIndex < _minefield.NumberOfRows; rowIndex++)
                    {
                        yield return new Cell(columnIndex, rowIndex);
                    }
                }
            }
        }
    }
}