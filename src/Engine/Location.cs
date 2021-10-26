using System;
using System.Text.RegularExpressions;

namespace Dgt.Minesweeper.Engine
{
    // IDEA Reduce memory footprint by using ReadOnlySpan<char>
    // We're likely to be allocating lots of Location instances. We're also likely to only see a small set of values
    // for Column. Can we consume a lot less memory by keeping a list of spans and then pointing different instances
    // of Location to the same span?
    
    // TODO The constructor needs to validate the format of Column
    // TODO The constructor needs to validate Row is positive, non-zero
    public record Location(string Column, int Row)
    {
        private const string LocationPattern = @"^\s*(?<column>[A-Z]+)\s*(?<row>\d+)\s*$";
        private static readonly Regex LocationRegex = new(LocationPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        
        // TODO Validate input is not null or empty
        public static Location Parse(string input)
        {
            var match = LocationRegex.Match(input);

            if (!match.Success)
            {
                throw new FormatException();
            }

            var column = match.Groups["column"].Value.ToUpperInvariant();
            var row = int.Parse(match.Groups["row"].Value);

            return new Location(column, row);
        }
    }
}