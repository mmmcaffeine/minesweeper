namespace Dgt.Minesweeper.Engine
{
    public class Minefield
    {
        public Minefield(int rows, int columns)
        {
            Rows = rows;
            Columns = columns;
        }

        public int Rows { get; }
        public int Columns { get; }
    }
}