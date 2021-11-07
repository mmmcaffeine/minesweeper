using Dgt.Minesweeper.Engine;
using FluentAssertions;
using Xunit;

namespace Dgt.Minesweeper.ConsoleUI
{
    public class CellRendererTests
    {
        [Theory]
        [InlineData(false, 0)]
        [InlineData(false, 1)]
        [InlineData(true, 0)]
        [InlineData(true, 4)]
        public void RenderCell_Should_RenderNotRevealedAndNotFlaggedCellAsPeriod(bool isMined, int hint)
        {
            // Arrange
            var cell = new Cell(Location.Parse("A1"), isMined, hint)
            {
                IsRevealed = false,
                IsFlagged = false
            };

            // Act
            var character = CellRenderer.RenderCell(cell);

            // Assert
            character.Should().Be('.');
        }

        [Theory]
        [InlineData(false, 0)]
        [InlineData(false, 1)]
        [InlineData(true, 0)]
        [InlineData(true, 4)]
        public void RenderCell_Should_RenderNotRevealedAndFlaggedCellAsF(bool isMined, int hint)
        {
            // Arrange
            var cell = new Cell(Location.Parse("A1"), isMined, hint)
            {
                IsRevealed = false,
                IsFlagged = true
            };

            // Act
            var character = CellRenderer.RenderCell(cell);

            // Assert
            character.Should().Be('F');
        }
    }
}