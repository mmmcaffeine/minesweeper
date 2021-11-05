namespace Dgt.Minesweeper.Engine
{
    public interface IGetHintStrategy
    {
        int GetHint(Location location, IMinefield minefield);
    }
}