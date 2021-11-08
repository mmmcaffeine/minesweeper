using Dgt.Minesweeper.Engine;

namespace Dgt.Minesweeper.ConsoleUI
{
    public interface ICellRenderer
    {
        char RenderCell(Cell cell);
    }
}