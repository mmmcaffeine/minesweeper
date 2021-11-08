using System;
using Dgt.Minesweeper.Engine;

namespace Dgt.Minesweeper.ConsoleUI
{
    public static class CellRenderer
    {
        public static char RenderCell(Cell cell)
        {
            if (cell is null) throw new ArgumentNullException(nameof(cell));

            return (cell.IsRevealed, cell.IsMined) switch
            {
                (false, _)    => cell.IsFlagged ? 'F' : '.',
                (true, false) => cell.Hint == 0 ? ' ' : cell.Hint.ToString()[0],
                (true, true)  => '*',
            };
        }
    }
}