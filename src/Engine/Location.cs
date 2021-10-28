using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Dgt.Minesweeper.Engine
{
    public record Location
    {
        private static class ColumnErrors
        {
            public const string CannotBeNull = "Value cannot be null.";
            public const string CannotBeEmpty = "Value cannot be whitespace or an empty string.";
            public const string NotCorrectFormat = "Input string was not in a correct format.";
        }
        
        private const int ColumnBase = 26;
        private const int AsciiCodeForA = 65;
        
        private const string ColumnRequirement = "Value must be one or more letters, with no other characters e.g. 'AA'.";
        private const string RowRequirement = "Value must be a positive, non-zero integer.";
        private const string LocationPattern = @"^\s*(?<column>[A-Z]+)\s*(?<row>\d+)\s*$";
        private static readonly Regex LocationRegex = new(LocationPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public Location(string column, int row)
            : this(row)
        {
            if (column! is null) throw new ArgumentNullException(nameof(column), $"{ColumnErrors.CannotBeNull} {ColumnRequirement}");
            if (string.IsNullOrWhiteSpace(column)) throw CreateColumnException(ColumnErrors.CannotBeEmpty);
            if (column.Any(c => !char.IsLetter(c))) throw CreateColumnException(ColumnErrors.NotCorrectFormat);

            ColumnName = (ColumnName)column;

            Exception CreateColumnException(string error) => new ArgumentException($"{error} {ColumnRequirement}", nameof(column))
            {
                Data = { { nameof(column), column } }
            };
        }

        public Location(ColumnName columnName, int row)
            : this(row)
        {
            ColumnName = columnName ?? throw new ArgumentNullException(nameof(columnName));
        }

        private Location(int row)
        {
            if (row <= 0) throw new ArgumentOutOfRangeException(nameof(row), row, RowRequirement);
            
            Row = row;
        }
        
        // We can bypass constructor validation logic because we have already done the validation when
        // we matched against the Regex
        private Location(Match match)
        {
            ColumnName = (ColumnName)match.Groups["column"].Value;
            Row = int.Parse(match.Groups["row"].Value);
        }

        public ColumnName ColumnName { get; } = default!;
        public int Row { get; }

        public void Deconstruct(out string column, out int row)
        {
            column = ColumnName;
            row = Row;
        }

        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        // People can use the null forgiving operator to pass nulls for location and cause NullReferenceExceptions
        public static bool operator ==(Location location, (string Column, int Row) tuple) =>
            location is not null && location.ColumnName == tuple.Column && location.Row == tuple.Row;

        public static bool operator !=(Location location, (string Column, int Row) tuple) => !(location == tuple);

        public static bool operator ==((string Column, int Row) tuple, Location location) => location == tuple;

        public static bool operator !=((string Column, int Row) tuple, Location location) => !(tuple == location);

        public static implicit operator string(Location location) => $"{location.ColumnName.Value}{location.Row}";

        public static explicit operator Location(string location)
        {
            try
            {
                return Parse(location);
            }
            catch (FormatException exception)
            {
                throw new InvalidCastException("Specified cast is not valid.", exception);
            }
        }
        
        public static Location Parse(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) throw CreateParsingException(s);
            
            var match = LocationRegex.Match(s);

            if (!match.Success)
            {
                throw CreateParsingException(s);
            }

            return new Location(match);
        }

        public static bool TryParse(string s, [NotNullWhen(true)] out Location? result)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                result = null;
            }
            else
            {
                var match = LocationRegex.Match(s);

                result = match.Success
                    ? new Location(match)
                    : null;
            }

            return result != null;
        }

        private static Exception CreateParsingException(string input)
        {
            var builder = new StringBuilder("Input string was not in a correct format.");

            builder.Append(" Locations must be of the form 'A1'.");
            builder.Append($" Actual value was '{input}'.");

            return new FormatException(builder.ToString())
            {
                Data = { [nameof(input)] = input }
            };
        }
        
        public bool IsAdjacentTo(Location location)
        {
            return AreDifferentLocations(this, location)
                   && RowsAreAdjacent(this, location)
                   && ColumnsAreAdjacent(this, location);
        }

        private static bool AreDifferentLocations(Location left, Location right)
        {
            return (left, right) switch
            {
                (null, _) => false,
                (_, null) => false,
                var (x, y) when x == y => false,
                _ => true
            };
        }

        private static bool RowsAreAdjacent(Location left, Location right) => left.Row - right.Row is >= -1 and <= 1;

        private static bool ColumnsAreAdjacent(Location left, Location right) =>
            (int)left.ColumnName - (int)right.ColumnName is >= -1 and <= 1;
    }
}