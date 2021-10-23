namespace Dgt.Minesweeper.Engine
{
    public record Cell(int Column, int Row)
    {
        public bool IsAdjacentTo(Cell cell)
        {
            if (cell == this) return false;

            if (Column - cell.Column is < -1 or > 1)
            {
                return false;
            }
            
            if (Row - cell.Row is < -1 or > 1)
            {
                return false;
            }

            return true;
        }

        public bool IsAdjacentTo(int column, int row) => IsAdjacentTo(new Cell(column, row));
    }
}