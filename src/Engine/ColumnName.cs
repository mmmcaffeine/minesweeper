﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Dgt.Minesweeper.Engine
{
    public record ColumnName
    {
        private static class Errors
        {
            public const string CannotBeNull = "Value cannot be null.";
            public const string CannotBeEmpty = "Value cannot be whitespace or an empty string.";
            public const string NotCorrectFormat = "Input string was not in a correct format.";
            public const string InvalidCast = "Specified cast is not valid.";
        }
        
        private const string ColumnNameRequirement = "Value must be a string consisting only of letters e.g. 'ABC'.";
        private const string ColumnIndexRequirement = "Value must be a positive, non-zero integer.";
        
        private const int AsciiCodeForA = 65;
        private const int Radix = 26;
        private const int Offset = 1;

        public ColumnName(string value)
        {
            if (value! is null) throw new ArgumentNullException(nameof(value), $"{Errors.CannotBeNull} {ColumnNameRequirement}");
            if (string.IsNullOrWhiteSpace(value)) throw CreateInvalidValueException(Errors.CannotBeEmpty);
            if (value.Any(c => !char.IsLetter(c))) throw CreateInvalidValueException(Errors.NotCorrectFormat);
            
            Value = value;

            Exception CreateInvalidValueException(string error) =>
                new ArgumentException($"{error} {ColumnNameRequirement}", nameof(value))
                {
                    Data = { { nameof(value), value } }
                };
        }

        // Used to bypass validation of Value if we have already done it, either explicitly (explicit conversion from a
        // string), or implicitly (explicit conversion from an int)
        private ColumnName()
        {
            Value = string.Empty;
        }

        public string Value { get; private init; }

        public static explicit operator ColumnName(string value)
        {
            if (value! is null) throw CreateInvalidValueException();
            if (string.IsNullOrWhiteSpace(value)) throw CreateInvalidValueException();
            if (value.Any(c => !char.IsLetter(c))) throw CreateInvalidValueException();

            return new ColumnName { Value = value };
            
            Exception CreateInvalidValueException() =>
                new InvalidCastException($"{Errors.InvalidCast} {ColumnNameRequirement}")
                {
                    Data = { { nameof(value), value } }
                };
        }
        
        // We have to do a little bit of trickery because we're sort of base 26 but not really! We use 'A' to
        // represent 1 so we don't have the concept of a zero. In other words we're base 26 in so far as we
        // have a radix of 26, but our values run from 1 to 26, not 0 to 25!
        public static explicit operator ColumnName(int value)
        {
            var quotient = value > 0
                ? value
                : throw new InvalidCastException($"Specified cast is not valid. {ColumnIndexRequirement}")
                {
                    Data = { {nameof(value), value }}
                };
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
            var columnNameValue = new string(characters);

            return new ColumnName { Value = columnNameValue };
        }

        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        // People can use the null forgiving operator to pass nulls for columnName and cause NullReferenceExceptions
        public static implicit operator string(ColumnName columnName) =>
            columnName != null ? columnName.Value : string.Empty;

        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        // People can use the null forgiving operator to pass nulls for columnName and cause NullReferenceExceptions
        public static implicit operator int(ColumnName columnName)
        {
            if (columnName is null) return 0;
            
            var remainders = GetRemainders(columnName.Value).ToArray();
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