using System.Collections.Generic;

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

        // TODO Validate the cell exists on the minefield / in our dictionary of cell states
        public CellState GetCellState(Cell cell) => _cellStates[cell];

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