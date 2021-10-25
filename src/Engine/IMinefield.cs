using System.Collections;
using System.Collections.Generic;

namespace Dgt.Minesweeper.Engine
{
    public interface IMinefield : IEnumerable<Cell>
    {
        int NumberOfColumns { get; }
        int NumberOfRows { get; }
        bool IsMined(Cell cell);
        int GetHint(Cell cell);

        private IEnumerable<Cell> GetCells()
        {
            for (var columnIndex = 0; columnIndex < NumberOfColumns; columnIndex++)
            {
                for (var rowIndex = 0; rowIndex < NumberOfRows; rowIndex++)
                {
                    yield return new Cell(columnIndex, rowIndex);
                }
            }
        }

        IEnumerator<Cell> IEnumerable<Cell>.GetEnumerator() => GetCells().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}