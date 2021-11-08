using System;
using Dgt.Minesweeper.Engine;
using FluentAssertions;
using Xunit;

namespace Dgt.Minesweeper.ConsoleUI
{
    public class CellRendererTests
    {
        [Fact]
        public void RenderCell_Should_ThrowWhenCellIsNull()
        {
            // Arrange
            var sut = new CellRenderer();
            
            // Act
            Action act = () => _ = sut.RenderCell(null!);
            
            // Assert
            act.Should().Throw<ArgumentNullException>().WithParameterName("cell");
        }
        
        [Theory]
        [InlineData(false, 0)]
        [InlineData(false, 1)]
        [InlineData(true, 0)]
        [InlineData(true, 4)]
        public void RenderCell_Should_RenderNotRevealedAndNotFlaggedCellAsPeriod(bool isMined, int hint)
        {
            // Arrange
            var sut = new CellRenderer();
            var cell = new Cell(Location.Parse("A1"), isMined, hint)
            {
                IsRevealed = false,
                IsFlagged = false
            };

            // Act
            var character = sut.RenderCell(cell);

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
            var sut = new CellRenderer();
            var cell = new Cell(Location.Parse("A1"), isMined, hint)
            {
                IsRevealed = false,
                IsFlagged = true
            };

            // Act
            var character = sut.RenderCell(cell);

            // Assert
            character.Should().Be('F');
        }

        [Fact]
        public void RenderCell_Should_RenderRevealedCellWithHintOfZeroAsSpace()
        {
            // Arrange
            var sut = new CellRenderer();
            var cell = new Cell(Location.Parse("B2"), false, 0)
            {
                IsRevealed = true,
                IsFlagged = false
            };
            
            // Act
            var character = sut.RenderCell(cell);
            
            // Assert
            character.Should().Be(' ');
        }

        // Don't just cast hint to char. Doing so treats the int as the ASCII code so e.g. 1 would come out as
        // SOH (start of heading). We could do ToString()[0] but that is less clear
        [Theory]
        [InlineData(1, '1')]
        [InlineData(3, '3')]
        [InlineData(5, '5')]
        public void RenderCell_Should_RenderRevealedCellWithNonZeroHintAsHint(int hint, char expected)
        {
            // Arrange
            var sut = new CellRenderer();
            var cell = new Cell(Location.Parse("B2"), false, hint)
            {
                IsRevealed = true,
                IsFlagged = false
            };
            
            // Act
            var character = sut.RenderCell(cell);
            
            // Assert
            character.Should().Be(expected);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(4)]
        public void RenderCell_Should_RenderRevealedAndMinedCellAsAsterisk(int hint)
        {
            // Arrange
            var sut = new CellRenderer();
            var cell = new Cell(Location.Parse("C3"), true, hint)
            {
                IsRevealed = true,
                IsFlagged = false
            };
            
            // Act
            var character = sut.RenderCell(cell);
            
            // Assert
            character.Should().Be('*');
        }
    }
}