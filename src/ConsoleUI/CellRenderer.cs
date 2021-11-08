using Dgt.Minesweeper.Engine;

namespace Dgt.Minesweeper.ConsoleUI
{
    public static class CellRenderer
    {
        public static char RenderCell(Cell cell)
        {
            if (cell.IsRevealed)
            {
                if (cell.IsMined)
                {
                    return '*';
                }
                else
                {
                    return cell.Hint == 0 ? ' ' : cell.Hint.ToString()[0];
                }
            }
            else
            {
                return cell.IsFlagged ? 'F' : '.';
            }
        }
    }
}