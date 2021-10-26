﻿using System;
using FluentAssertions;
using Xunit;

namespace Dgt.Minesweeper.Engine
{
    public class LocationTests
    {
        [Theory]
        [InlineData("A1", "A", 1)]
        [InlineData("a1", "A", 1)]
        [InlineData("H8", "H", 8)]
        [InlineData("AA99", "AA", 99)]
        [InlineData("ZZZ1000", "ZZZ", 1000)]
        public void Parse_Should_ParseColumnAndRowWhenInputIsProperlyFormatted
            (string input, string expectedColumn, int expectedRow)
        {
            // Arrange, Act
            var (column, row) = Location.Parse(input);
            
            // Assert
            column.Should().Be(expectedColumn);
            row.Should().Be(expectedRow);
        }

        [Theory]
        [InlineData("    A1", "A", 1)]
        [InlineData("A1     ", "A", 1)]
        [InlineData("A       1", "A", 1)]
        [InlineData("   A  1   ", "A", 1)]
        public void Parse_Should_IgnoreWhitespaceWhenParsingInput
            (string input, string expectedColumn, int expectedRow)
        {
            // Arrange, Act
            var (column, row) = Location.Parse(input);
            
            // Assert
            column.Should().Be(expectedColumn);
            row.Should().Be(expectedRow);
        }

        [Theory]
        [InlineData("A")]
        [InlineData("1")]
        [InlineData("1A")]
        [InlineData("Nope!")]
        public void Parse_Should_ThrowFormatExceptionWhenInputStringIsNotProperlyFormatted(string input)
        {
            // Arrange, Act
            Action act = () => Location.Parse(input);

            // Assert
            act.Should().Throw<FormatException>()
                .WithMessage("*Input string was not in a correct format.*")
                .WithMessage("*Locations must be of the form 'A1'.*")
                .WithMessage($"*Actual value was '{input}'.*")
                .Where(ex => ex.Data.Contains("input") && ex.Data["input"]!.Equals(input));
        }

        [Theory]
        [InlineData("A", 1, "A1")]
        [InlineData("ZZ", 1, "ZZ1")]
        [InlineData("A", 99, "A99")]
        public void ConvertToString_Should_FormatAsColumnAndRow(string column, int row, string expected)
        {
            // Arrange
            var location = new Location(column, row);

            // Act
            string actual = location;
            
            // Assert
            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData("A1", "A", 1)]
        [InlineData("a1", "A", 1)]
        [InlineData("H8", "H", 8)]
        [InlineData("AA99", "AA", 99)]
        [InlineData("ZZZ1000", "ZZZ", 1000)]
        public void ConvertFromString_Should_ParseColumnAndStringWhenInputIsProperlyFormatted
            (string input, string expectedColumn, int expectedRow)
        {
            // Arrange, Act
            var location = (Location)input;
            
            // Assert
            location.Column.Should().Be(expectedColumn);
            location.Row.Should().Be(expectedRow);
        }

        [Theory]
        [InlineData("A")]
        [InlineData("1")]
        [InlineData("1A")]
        [InlineData("Nope!")]
        public void ConvertFromString_ShouldThrowInvalidCastExceptionWhenInputIsNotProperlyFormatted(string input)
        {
            // Arrange, Act
            Action act = () => _ = (Location)input;
            
            // Assert
            act.Should().Throw<InvalidCastException>()
                .WithMessage("Specified cast is not valid.*")
                .WithInnerException<FormatException>();
        }
    }
}