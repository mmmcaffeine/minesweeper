﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            if (!((IMinefield)this).Contains(location))
            {
                throw CreateLocationNotInMinefieldException(location, nameof(location));
            }

            return _minedLocations.Contains(location);
        }
        
        public int GetHint(Location location) => GetAdjacentLocations(location).Count(l => _minedLocations.Contains(l));

        public IEnumerable<Location> GetAdjacentLocations(Location location)
        {
            var (columnName, rowIndex) = location;
            var columnIndices = GetIndices(columnName, NumberOfColumns);
            var rowIndices = GetIndices(rowIndex, NumberOfRows);
            
            return columnIndices
                .SelectMany(ci => rowIndices.Select(ri => new Location(ci, ri)))
                .Except(new[] { location });

            IEnumerable<int> GetIndices(int index, int maximum)
            {
                if (index > 1) yield return index - 1;
                yield return index;
                if (index < maximum) yield return index + 1;
            }
        }

        private Exception CreateLocationNotInMinefieldException(Location location, string paramName)
        {
            var builder = new StringBuilder();

            builder.Append("The location does not exist in the minefield.");

            if (location.ColumnIndex > NumberOfColumns) builder.Append(" The column is out of bounds.");
            if (location.RowIndex > NumberOfRows) builder.Append(" The row is out of bounds.");

            return new ArgumentException(builder.ToString(), paramName)
            {
                Data = { { nameof(location), location } }
            };
        }
    }
}