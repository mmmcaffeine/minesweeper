using System;
using System.Collections.Generic;
using System.Linq;

namespace Dgt.Minesweeper.Engine
{
    public static class ColumnNameConverter
    {
        private const int AsciiCodeForA = 65;
        private const int Radix = 26;
        private const int Offset = 1;
        
        // TODO 1 or greater
        // We have to do a little bit of trickery because we're sort of base 26 but not really! We use 'A' to
        // represent 1 so we don't have the concept of a zero. In other words we're base 26 in so far as we
        // have a radix of 26, but our values run from 1 to 26, not 0 to 25!
        public static string ToColumnName(this int columnIndex)
        {
            var remainders = new Stack<int>();
            var quotient = columnIndex;

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

        // TODO No nulls or empty strings
        // TODO No strings that contain non-chars
        // TODO Case insensitive
        public static int ToColumnIndex(this string columnName)
        {
            var remainders = GetRemainders(columnName).ToArray();
            var columnIndex = 0;
            var multiplier = 1;

            for (var i = remainders.Length - 1; i >= 0; i--)
            {
                columnIndex += remainders[i] * multiplier;
                multiplier *= Radix;
            }

            return columnIndex;
        }
        
        private static IEnumerable<int> GetRemainders(string value)
        {
            return value.Select(c => c - AsciiCodeForA + Offset);
        }
    }
}