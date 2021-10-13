namespace Dgt.Minesweeper.Engine
{
    public class MinefieldGenerator
    {
        public Minefield GenerateMinefield(int rows, int columns)
        {
            return new Minefield(rows, columns);
        }
    }
}