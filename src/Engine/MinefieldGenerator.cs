namespace Dgt.Minesweeper.Engine
{
    public class MinefieldGenerator
    {
        public Minefield GenerateMinefield(int columns, int rows, params Cell[] minedCells)
        {
            return new Minefield(columns, rows, minedCells);
        }
    }
}