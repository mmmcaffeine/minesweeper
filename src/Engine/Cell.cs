namespace Dgt.Minesweeper.Engine
{
    public record Cell(int ColumnIndex, int RowIndex)
    {
        public bool IsAdjacentTo(Cell cell)
        {
            if (cell == this) return false;

            if (ColumnIndex - cell.ColumnIndex is < -1 or > 1)
            {
                return false;
            }
            
            if (RowIndex - cell.RowIndex is < -1 or > 1)
            {
                return false;
            }

            return true;
        }

        public bool IsAdjacentTo(int columnIndex, int rowIndex) => IsAdjacentTo(new Cell(columnIndex, rowIndex));
    }
}