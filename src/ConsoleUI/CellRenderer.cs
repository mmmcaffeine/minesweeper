using Dgt.Minesweeper.Engine;

namespace Dgt.Minesweeper.ConsoleUI
{
    public static class CellRenderer
    {
        public static char RenderCell(Cell cell) => cell.IsFlagged ? 'F' : '.';
    }
}