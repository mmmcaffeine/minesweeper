using System.Collections;
using System.Collections.Generic;

namespace Dgt.Minesweeper.Engine
{
    public interface IMinefield : IEnumerable<Location>
    {
        int NumberOfColumns { get; }
        int NumberOfRows { get; }
        int CountOfMines { get; }
        bool IsMined(Location location);
        int GetHint(Location location);

        private IEnumerable<Location> GetLocations()
        {
            for (var columnIndex = 1; columnIndex <= NumberOfColumns; columnIndex++)
            {
                for (var rowIndex = 1; rowIndex <= NumberOfRows; rowIndex++)
                {
                    yield return new Location((ColumnName)columnIndex, rowIndex);
                }
            }
        }

        IEnumerator<Location> IEnumerable<Location>.GetEnumerator() => GetLocations().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}