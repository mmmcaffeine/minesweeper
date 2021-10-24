using System;
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
            var hint = minefield.GetHint(column, row);
            
            // Assert
            hint.Should().Be(expectedHint);
        }

        // TODO We could make our exception better by:
        // * Putting the actual cell on the exception (maybe in Data if we don't want to write our own exception type)
        // * Indicating whether it is column, row, or both that are out of range
        [Fact]
        public void IsMined_Should_ThrowWhenCellIsNotInMinefield()
        {
            // Arrange
            var minefield = new Minefield(4, 3, new[] { new Cell(0, 0), new Cell(2, 1) });
            var cellToTest = new Cell(4, 0);
            
            // Act, Assert
            minefield.Invoking(x => x.IsMined(cellToTest)).Should().Throw<ArgumentException>()
                .WithMessage("The cell does not exist in the minefield.*")
                .WithParameterName("cell");
        }

        [Fact]
        public void IsMined_Should_ThrowWhenColumnIsOutOfRange()
        {
            // Arrange
            var minefield = new Minefield(4, 3, new[] { new Cell(0, 0), new Cell(2, 1) });
            
            // Act, Assert
            minefield.Invoking(x => x.IsMined(4, 0)).Should().Throw<ArgumentOutOfRangeException>()
                .WithMessage("Value must be greater than or equal to zero, and less than the number of columns.*")
                .WithParameterName("columnIndex")
                .And.ActualValue.Should().Be(4);
        }
        
        [Fact]
        public void IsMined_Should_ThrowWhenRowIsOutOfRange()
        {
            // Arrange
            var minefield = new Minefield(4, 3, new[] { new Cell(0, 0), new Cell(2, 1) });
            
            // Act, Assert
            minefield.Invoking(x => x.IsMined(0, 3)).Should().Throw<ArgumentOutOfRangeException>()
                .WithMessage("Value must be greater than or equal to zero, and less than the number of rows.*")
                .WithParameterName("rowIndex")
                .And.ActualValue.Should().Be(3);

        }
    }
}