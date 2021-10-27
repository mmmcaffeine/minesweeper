using System;
using System.Collections.Generic;
using System.Linq;

namespace Dgt.Minesweeper.Engine
{
    public static class ColumnNameConverter
    {
        private static class Errors
        {
            public const string CannotBeNull = "Value cannot be null.";
            public const string CannotBeEmpty = "Value cannot be whitespace or an empty string.";
            public const string NotCorrectFormat = "Input string was not in a correct format.";
        }
        
        private const string ColumnNameRequirement = "Value must be a string consisting only of letters e.g. 'ABC'.";
        private const string ColumnIndexRequirement = "Value must be a positive, non-zero integer.";
        
        private const int AsciiCodeForA = 65;
        private const int Radix = 26;
        private const int Offset = 1;
        
        // TODO 1 or greater
        // We have to do a little bit of trickery because we're sort of base 26 but not really! We use 'A' to
        // represent 1 so we don't have the concept of a zero. In other words we're base 26 in so far as we
        // have a radix of 26, but our values run from 1 to 26, not 0 to 25!
        public static string ToColumnName(this int columnIndex)
        {
            var quotient = columnIndex > 0
                ? columnIndex
                : throw new ArgumentOutOfRangeException(nameof(columnIndex), columnIndex, ColumnIndexRequirement);
            var remainders = new Stack<int>();

            while (quotient != 0)
            {
                quotient = Math.DivRem(quotient, Radix, out var remainder);
                
                if (remainder == 0)
                {
                    remainders.Push(Radix);
                    quotient--;
                }
                else
                {
                    remainders.Push(remainder);
                }
            }

            var asciiCodes = remainders.Select(i => i - Offset + AsciiCodeForA);
            var characters = asciiCodes.Select(code => (char)code).ToArray();

            return new string(characters);
        }

        // This validation logic is identical to that in Location. Is that telling me we have the Primitive
        // Obsession anti-pattern, and ColumnName needs to be a type in its own right?
        public static int ToColumnIndex(this string columnName)
        {
            if(columnName! is null) throw new ArgumentNullException(nameof(columnName), $"{Errors.CannotBeNull} {ColumnNameRequirement}");
            if (string.IsNullOrWhiteSpace(columnName)) throw CreateColumnNameException(Errors.CannotBeEmpty);
            if (columnName.Any(c => !char.IsLetter(c))) throw CreateColumnNameException(Errors.NotCorrectFormat);
            
            var remainders = GetRemainders(columnName).ToArray();
            var columnIndex = 0;
            var multiplier = 1;

            for (var i = remainders.Length - 1; i >= 0; i--)
            {
                columnIndex += remainders[i] * multiplier;
                multiplier *= Radix;
            }

            return columnIndex;

            Exception CreateColumnNameException(string error) =>
                new ArgumentException($"{error} {ColumnNameRequirement}", nameof(columnName))
                {
                    Data = { { nameof(columnName), columnName } }
                };
        }
        
        private static IEnumerable<int> GetRemainders(string value)
        {
            return value.Select(c => c - AsciiCodeForA + Offset);
        }
    }
}