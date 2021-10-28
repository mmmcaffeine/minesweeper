using System;
using System.Collections.Generic;
using System.Linq;

namespace Dgt.Minesweeper.Engine
{
    public class Minefield : IMinefield
    {
        private readonly HashSet<Location> _minedLocations;

        // TODO Validate positive non-zero integers (and maybe more than 1 makes sense?
        // TODO Validate non-null enumerable for mined cells
        public Minefield(int numberOfColumns, int numberOfRows, IEnumerable<Location> minedLocations)
        {
            NumberOfColumns = numberOfColumns;
            NumberOfRows = numberOfRows;
            _minedLocations = new HashSet<Location>(minedLocations);
        }

        public int NumberOfColumns { get; }
        public int NumberOfRows { get; }
        public int CountOfMines => _minedLocations.Count;

        public bool IsMined(Location location)
        {
            if (!HasLocation(location)) throw CreateLocationNotInMinefieldException(nameof(location));

            return _minedLocations.Contains(location);
        }

        public int GetHint(Location location) => _minedLocations.Count(c => c.IsAdjacentTo(location));

        private static Exception CreateLocationNotInMinefieldException(string paramName)
        {
            return new ArgumentException("The location does not exist in the minefield.", paramName);
        }

        private bool HasLocation(Location location) => HasLocation((int)location.ColumnName, location.RowIndex);

        // I think this is off by one as we inherited it from when we were using a zero-based coordinate system
        private bool HasLocation(int columnIndex, int rowIndex) => columnIndex >= 0
                                                     && columnIndex < NumberOfColumns
                                                     && rowIndex >= 0
                                                     && rowIndex < NumberOfRows;
    }
}