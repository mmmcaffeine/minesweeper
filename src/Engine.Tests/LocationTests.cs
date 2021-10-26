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
    }
}