namespace Dgt.Minesweeper.Engine
{
    public class MinefieldGenerator
    {
        public Minefield GenerateMinefield(int rows, int columns, params Cell[] minedCells)
        {
            return new Minefield(rows, columns, minedCells);
        }
    }
}