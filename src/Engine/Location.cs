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
        
        private const string ColumnRequirement = "Value must be one or more letters, with no other characters e.g. 'AA'.";
        private const string RowRequirement = "Value must be a positive, non-zero integer.";
        private const string LocationPattern = @"^\s*(?<column>[A-Z]+)\s*(?<row>\d+)\s*$";
        private static readonly Regex LocationRegex = new(LocationPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public Location(string column, int row)
        {
            if (column! is null) throw new ArgumentNullException(nameof(column), $"{ColumnErrors.CannotBeNull} {ColumnRequirement}");
            if (string.IsNullOrWhiteSpace(column)) throw CreateColumnException(ColumnErrors.CannotBeEmpty);
            if (column.Any(c => !char.IsLetter(c))) throw CreateColumnException(ColumnErrors.NotCorrectFormat);
            if (row <= 0) throw new ArgumentOutOfRangeException(nameof(row), row, RowRequirement);
            
            Column = column.ToUpperInvariant();
            Row = row;

            Exception CreateColumnException(string error) => new ArgumentException($"{error} {ColumnRequirement}", nameof(column))
            {
                Data = { { nameof(column), column } }
            };
        }
        
        // We can bypass constructor validation logic because we have already done the validation when
        // we matched against the Regex
        private Location(Match match)
        {
            Column = match.Groups["column"].Value.ToUpperInvariant();
            Row = int.Parse(match.Groups["row"].Value);
        }
        
        public string Column { get; }
        public int Row { get; }

        public void Deconstruct(out string column, out int row)
        {
            column = Column;
            row = Row;
        }

        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        // People can use the null forgiving operator to pass nulls for location and cause NullReferenceExceptions
        public static bool operator ==(Location location, (string Column, int Row) tuple) =>
            location is not null && location.Column == tuple.Column && location.Row == tuple.Row;

        public static bool operator !=(Location location, (string Column, int Row) tuple) => !(location == tuple);

        public static bool operator ==((string Column, int Row) tuple, Location location) => location == tuple;

        public static bool operator !=((string Column, int Row) tuple, Location location) => !(tuple == location);

        public static implicit operator string(Location location) => $"{location.Column}{location.Row}";

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
        
        // TODO Validate input is not null or empty
        public static Location Parse(string s)
        {
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
        
        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        // People can use the null forgiving operator to pass nulls for location and cause NullReferenceExceptions
        public bool IsAdjacentTo(Location location)
        {
            if (location is null || location == this) return false;

            if (Subtract(Column, location.Column) is < -1 or > 1)
            {
                return false;
            }
            
            if (Row - location.Row is < -1 or > 1)
            {
                return false;
            }

            return true;
        }

        private static int Subtract(string left, string right) => ConvertToInt(left) - ConvertToInt(right);
        
        private static int ConvertToInt(string value)
        {
            var converted = 0;
            
            for (var index = value.Length - 1; index >= 0; index--)
            {
                converted += (value[index] - 65) + (26 * index);
            }

            return converted;
        }
    }
}