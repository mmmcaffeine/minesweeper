using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Dgt.Minesweeper.Engine
{
    // IDEA Reduce memory footprint by using ReadOnlySpan<char>
    // We're likely to be allocating lots of Location instances. We're also likely to only see a small set of values
    // for Column. Can we consume a lot less memory by keeping a list of spans and then pointing different instances
    // of Location to the same span?
    
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
            
            Column = column;
            Row = row;

            Exception CreateColumnException(string error) => new ArgumentException($"{error} {ColumnRequirement}", nameof(column))
            {
                Data = { { nameof(column), column } }
            };
        }
        
        public string Column { get; }
        public int Row { get; }

        public void Deconstruct(out string column, out int row)
        {
            column = Column;
            row = Row;
        }

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
        public static Location Parse(string input)
        {
            var match = LocationRegex.Match(input);

            if (!match.Success)
            {
                throw CreateParsingException(input);
            }

            var column = match.Groups["column"].Value.ToUpperInvariant();
            var row = int.Parse(match.Groups["row"].Value);

            return new Location(column, row);
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
    }
}