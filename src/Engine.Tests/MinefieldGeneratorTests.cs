using FluentAssertions;
using Xunit;

namespace Dgt.Minesweeper.Engine
{
    public class MinefieldGeneratorTests
    {
        [Fact]
        public void GenerateMinefield_Should_GenerateMinefieldWithSuppliedDimensions()
        {
            // Arrange
            var sut = new MinefieldGenerator();
            
            // Act
            var minefield = sut.GenerateMinefield(4, 3);
            
            // Assert
            minefield.Columns.Should().Be(4);
            minefield.Rows.Should().Be(3);
        }
        
        [Fact]
        public void GenerateMinefield_Should_PlaceMinesInCellsThatAreSpecified()
        {
            // Arrange
            var sut = new MinefieldGenerator();
            
            // Act
            var minefield = sut.GenerateMinefield(4, 3, new Cell(0, 0), new Cell(3, 2));

            // Assert
            minefield.IsMined(0, 0).Should().BeTrue();
            minefield.IsMined(3, 2).Should().BeTrue();
        }
        
        [Fact]
        public void GenerateMinefield_Should_NotPlaceMinesInCellsThatAreNotSpecified()
        {
            // Arrange
            var sut = new MinefieldGenerator();
            
            // Act
            var minefield = sut.GenerateMinefield(4, 3, new Cell(0, 0), new Cell(3, 2));

            // Assert
            minefield.IsMined(1, 1).Should().BeFalse();
            minefield.IsMined(3, 0).Should().BeFalse();
        }
    }
}