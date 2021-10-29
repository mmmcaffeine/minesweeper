using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Dgt.Minesweeper.Engine
{
    public record Location
    {
        private static class ColumnNameErrors
        {
            public const string CannotBeNull = "Value cannot be null.";
            public const string CannotBeEmpty = "Value cannot be whitespace or an empty string.";
            public const string NotCorrectFormat = "Input string was not in a correct format.";
        }
        
        private const string ColumnNameRequirement = "Value must be one or more letters, with no other characters e.g. 'AA'.";
        private const string RowIndexRequirement = "Value must be a positive, non-zero integer.";
        private const string LocationPattern = @"^\s*(?<columnName>[A-Z]+)\s*(?<rowIndex>\d+)\s*$";
        private static readonly Regex LocationRegex = new(LocationPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public Location(string columnName, int rowIndex)
            : this(rowIndex)
        {
            if (columnName! is null) throw new ArgumentNullException(nameof(columnName), $"{ColumnNameErrors.CannotBeNull} {ColumnNameRequirement}");
            if (string.IsNullOrWhiteSpace(columnName)) throw CreateColumnNameException(ColumnNameErrors.CannotBeEmpty);
            if (columnName.Any(c => !char.IsLetter(c))) throw CreateColumnNameException(ColumnNameErrors.NotCorrectFormat);

            ColumnName = (ColumnName)columnName;

            Exception CreateColumnNameException(string error) =>
                new ArgumentException($"{error} {ColumnNameRequirement}", nameof(columnName))
                {
                    Data = { { nameof(columnName), columnName } }
                };
        }

        public Location(ColumnName columnName, int rowIndex)
            : this(rowIndex)
        {
            ColumnName = columnName ?? throw new ArgumentNullException(nameof(columnName));
        }

        private Location(int rowIndex)
        {
            if (rowIndex <= 0) throw new ArgumentOutOfRangeException(nameof(rowIndex), rowIndex, RowIndexRequirement);
            
            RowIndex = rowIndex;
        }
        
        // We can bypass constructor validation logic because we have already done the validation when
        // we matched against the Regex
        private Location(Match match)
        {
            ColumnName = (ColumnName)match.Groups["columnName"].Value;
            RowIndex = int.Parse(match.Groups["rowIndex"].Value);
        }

        public ColumnName ColumnName { get; } = default!;
        public int RowIndex { get; }

        public void Deconstruct(out ColumnName columnName, out int rowIndex)
        {
            columnName = ColumnName;
            rowIndex = RowIndex;
        }

        public static (int ColumnDifference, int RowDifference) operator -(Location left, Location right)
        {
            // ReSharper disable ConditionIsAlwaysTrueOrFalse
            // People can use the null forgiving operator to pass nulls for location and cause NullReferenceExceptions
            if (left is null || right is null) return default;
            // ReSharper restore ConditionIsAlwaysTrueOrFalse
            
            var columnDifference = (int)left.ColumnName - (int)right.ColumnName;
            var rowDifference = left.RowIndex - right.RowIndex;

            return (columnDifference, rowDifference);
        }

        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        // People can use the null forgiving operator to pass nulls for location and cause NullReferenceExceptions
        public static bool operator ==(Location location, (string ColumnName, int RowIndex) tuple) =>
            location is not null && location.ColumnName == tuple.ColumnName && location.RowIndex == tuple.RowIndex;

        public static bool operator !=(Location location, (string ColumnName, int RowIndex) tuple) => !(location == tuple);

        public static bool operator ==((string ColumnName, int RowIndex) tuple, Location location) => location == tuple;

        public static bool operator !=((string ColumnName, int RowIndex) tuple, Location location) => !(tuple == location);

        public static implicit operator string(Location location) => $"{location.ColumnName.Value}{location.RowIndex}";

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
                Data = { { nameof(input), input } }
            };
        }
        
        public bool IsAdjacentTo(Location location)
        {
            if (!AreDifferentLocations(this, location))
            {
                return false;
            }

            return (this - location) switch
            {
                (< -1 or > 1, _) => false,
                (_, < -1 or > 1) => false,
                (_, _) => true
            };
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
    }
}