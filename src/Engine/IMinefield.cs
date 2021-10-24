namespace Dgt.Minesweeper.Engine
{
    public interface IMinefield
    {
        int Columns { get; }
        int Rows { get; }
        bool IsMined(Cell cell);
    }
}