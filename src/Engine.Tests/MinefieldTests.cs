using FluentAssertions;
using Xunit;

namespace Dgt.Minesweeper.Engine
{
    public class MinefieldTests
    {
        // TODO What should the hint for a mined square be?
        // The example input shows "3, 4" meaning 3 rows, amd 4 columns. Our origin is the bottom left, and our coordinate
        // system is 1-based i.e. how the vast majority of the population would think of a minesweeper grid
        [Theory]
        [InlineData(3, 2, 2)]
        [InlineData(1, 1, 0)]
        [InlineData(1, 4, 1)]
        public void GetHint_Should_ReturnNumberOfAdjacentMines(int row, int column, int expectedHint)
        {
            // Arrange
            var minefield = new Minefield(3, 4, new[] { new Square(3, 1), new Square(2, 3) });

            // Act
            var hint = minefield.GetHint(row, column);
            
            // Assert
            hint.Should().Be(expectedHint);
        }
    }
}