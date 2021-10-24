using FluentAssertions;
using Xunit;

namespace Dgt.Minesweeper.Engine
{
    public class MinefieldGeneratorTests
    {
        // TODO We're supposed to be testing the generator, but we're reliant on the implementation of Minefield.
        //      We might be better off changing this type to return the constructor parameters we would need to pass
        //      i.e. the ints for columns and rows, and then the IEnumerable of Cells
        [Fact]
        public void GenerateMinefield_Should_GenerateMinefieldFromInputStrings()
        {
            // Arrange
            var lines = new[]
            {
                "3 4",
                "*...",
                "..*.",
                "...."
            };
            var sut = new MinefieldGenerator();
            
            // Act
            var minefield = sut.GenerateMinefield(lines);

            // Assert
            minefield.NumberOfColumns.Should().Be(4);
            minefield.NumberOfRows.Should().Be(3);
            minefield.CountOfMines.Should().Be(2);

            minefield.IsMined(0, 0).Should().BeTrue();
            minefield.IsMined(2, 1).Should().BeTrue();
        }
        
        [Fact]
        public void GenerateMinefield_Should_GenerateMinefieldWithSuppliedDimensions()
        {
            // Arrange
            var sut = new MinefieldGenerator();
            
            // Act
            var minefield = sut.GenerateMinefield(4, 3);
            
            // Assert
            minefield.NumberOfColumns.Should().Be(4);
            minefield.NumberOfRows.Should().Be(3);
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