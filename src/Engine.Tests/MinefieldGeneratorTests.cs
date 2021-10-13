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
            var minefield = sut.GenerateMinefield(3, 4);
            
            // Assert
            minefield.Rows.Should().Be(3);
            minefield.Columns.Should().Be(4);
        }
        
        [Fact]
        public void GenerateMinefield_Should_PlaceMinesInSquaresThatAreSpecified()
        {
            // Arrange
            var sut = new MinefieldGenerator();
            
            // Act
            var minefield = sut.GenerateMinefield(3, 4, new Square(1, 1), new Square(3, 4));

            // Assert
            minefield.IsMined(1, 1).Should().BeTrue();
            minefield.IsMined(3, 4).Should().BeTrue();
        }
        
        [Fact]
        public void GenerateMinefield_Should_NotPlaceMinesInSquaresThatAreNotSpecified()
        {
            // Arrange
            var sut = new MinefieldGenerator();
            
            // Act
            var minefield = sut.GenerateMinefield(3, 4, new Square(1, 1), new Square(3, 4));

            // Assert
            minefield.IsMined(2, 2).Should().BeFalse();
            minefield.IsMined(1, 4).Should().BeFalse();
        }
    }
}