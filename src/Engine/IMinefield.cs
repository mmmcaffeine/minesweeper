namespace Dgt.Minesweeper.Engine
{
    public interface IMinefield
    {
        int NumberOfColumns { get; }
        int NumberOfRows { get; }
        bool IsMined(Cell cell);
    }
}