using FluentAssertions;
using Xunit;

namespace Dgt.Minesweeper.Engine
{
    public class CountMinedLocationsThatAreAdjacentGetHintStrategyTests
    {
        [Theory]
        [InlineData("A1", 1)]
        [InlineData("A2", 1)]
        [InlineData("A3", 1)]
        [InlineData("B1", 2)]
        [InlineData("B2", 2)]
        [InlineData("B3", 1)]
        [InlineData("C1", 0)]
        [InlineData("C2", 0)]
        [InlineData("C3", 0)]
        public void GetHint_Should_ReturnCountOfAdjacentLocationsThatAreMined(string location, int expectedHint)
        {
            // Arrange
            var minefield = new Minefield(3, new[] { Location.Parse("A1"), Location.Parse("A2") });
            var sut = new CountAdjacentLocationsThatAreMinedGetHintStrategy();
            
            // Act
            var hint = sut.GetHint(Location.Parse(location), minefield);
            
            // Assert
            hint.Should().Be(expectedHint);
        }
    }
}