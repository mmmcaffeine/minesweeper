using FluentAssertions;
using Xunit;

namespace Dgt.Minesweeper.Engine
{
    public class MinefieldGeneratorTests
    {
        // TODO We're supposed to be testing the generator, but we're reliant on the implementation of Minefield.
        //      That, in turn is reliant on the implementation of Location and ColumnName.
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

            minefield.IsMined(Location.Parse("A3")).Should().BeTrue();
            minefield.IsMined(Location.Parse("C2")).Should().BeTrue();
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
        public void GenerateMinefield_Should_PlaceMinesInLocationsThatAreSpecified()
        {
            // Arrange
            var sut = new MinefieldGenerator();
            
            // Act
            var minefield = sut.GenerateMinefield(4, 3, Location.Parse("A3"), Location.Parse("C2"));

            // Assert
            minefield.IsMined(Location.Parse("A3")).Should().BeTrue();
            minefield.IsMined(Location.Parse("C2")).Should().BeTrue();
        }
        
        [Fact]
        public void GenerateMinefield_Should_NotPlaceMinesInLocationsThatAreNotSpecified()
        {
            // Arrange
            var sut = new MinefieldGenerator();
            
            // Act
            var minefield = sut.GenerateMinefield(4, 3, Location.Parse("A3"), Location.Parse("C2"));

            // Assert
            minefield.IsMined(Location.Parse("B2")).Should().BeFalse();
            minefield.IsMined(Location.Parse("D3")).Should().BeFalse();
        }
    }
}