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
    }
}