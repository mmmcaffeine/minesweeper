﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Dgt.Minesweeper.Engine
{
    public class Minefield : IMinefield
    {
        private readonly HashSet<Location> _minedLocations;

        public Minefield(int numberOfRowsAndColumns, IEnumerable<string> minedLocations)
            : this
            (
                numberOfRowsAndColumns,
                (minedLocations ?? throw new ArgumentNullException(nameof(minedLocations))).Select(Location.Parse)
            )
        {
        }

        public Minefield(int numberOfRowsAndColumns, IEnumerable<Location> minedLocations)
            : this
            (
                numberOfRowsAndColumns > 0
                    ? numberOfRowsAndColumns
                    : throw CreateNumberOfException(numberOfRowsAndColumns, nameof(numberOfRowsAndColumns)),
                numberOfRowsAndColumns,
                minedLocations
            )
        {
        }

        public Minefield(int numberOfColumns, int numberOfRows, IEnumerable<string> minedLocations)
            : this
            (
                numberOfColumns,
                numberOfRows,
                (minedLocations ?? throw new ArgumentNullException(nameof(minedLocations))).Select(Location.Parse)
            )
        {
        }

        public Minefield(int numberOfColumns, int numberOfRows, IEnumerable<Location> minedLocations)
        {
            NumberOfColumns = numberOfColumns > 0
                ? numberOfColumns
                : throw CreateNumberOfException(numberOfColumns, nameof(numberOfColumns));
            NumberOfRows = numberOfRows > 0
                ? numberOfRows
                : throw CreateNumberOfException(numberOfRows, nameof(numberOfRows));
            _minedLocations = minedLocations is not null
                ? new HashSet<Location>(minedLocations)
                : throw new ArgumentNullException(nameof(minedLocations));
        }

        private static Exception CreateNumberOfException(int numberOf, string paramName) =>
            new ArgumentOutOfRangeException(paramName, numberOf, "Value must be a positive, non-zero integer.");

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

        private bool HasLocation(int columnIndex, int rowIndex) => columnIndex > 0
                                                     && columnIndex <= NumberOfColumns
                                                     && rowIndex > 0
                                                     && rowIndex <= NumberOfRows;
    }
}