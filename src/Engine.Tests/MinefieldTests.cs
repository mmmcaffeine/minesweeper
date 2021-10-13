using FluentAssertions;
using Xunit;

namespace Dgt.Minesweeper.Engine
{
    public class MinefieldTests
    {
        // TODO What should the hint for a mined cell be?
        [Theory]
        [InlineData(1, 0, 2)]
        [InlineData(0, 2, 0)]
        [InlineData(3, 2, 1)]
        public void GetHint_Should_ReturnNumberOfAdjacentMines(int column, int row, int expectedHint)
        {
            // Arrange
            var minefield = new Minefield(4, 3, new[] { new Cell(0, 0), new Cell(2, 1) });

            // Act
            var hint = minefield.GetHint(row, column);
            
            // Assert
            hint.Should().Be(expectedHint);
        }
    }
}