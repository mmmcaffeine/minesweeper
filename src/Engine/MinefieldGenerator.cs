namespace Dgt.Minesweeper.Engine
{
    public class MinefieldGenerator
    {
        public Minefield GenerateMinefield(int rows, int columns, params Square[] minedSquares)
        {
            return new Minefield(rows, columns, minedSquares);
        }
    }
}