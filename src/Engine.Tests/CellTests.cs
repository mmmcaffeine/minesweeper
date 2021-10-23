using FluentAssertions;
using Xunit;

namespace Dgt.Minesweeper.Engine
{
    public class CellTests
    {
        [Fact]
        public void IsAdjacentTo_Should_BeFalseWhenCellsAreTheSame()
        {
            new Cell(1, 1).IsAdjacentTo(new Cell(1, 1)).Should().BeFalse();
        }

        [Theory]
        // column row is either the same, immediately to the left, or immediately to the right, but the column is not
        [InlineData(0, 3)]
        [InlineData(1, 3)]
        [InlineData(2, 3)]
        // The row is either the same, immediately to the left, or immediately to the right, but the column is not
        [InlineData(3, 0)]
        [InlineData(3, 1)]
        [InlineData(3, 2)]
        public void IsAdjacentTo_Should_BeFalseWhenCellIsNotAdjacentColumnOrRow(int column, int row)
        {
            new Cell(1, 1).IsAdjacentTo(new Cell(column, row)).Should().BeFalse();
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(0, 1)]
        [InlineData(0, 2)]
        [InlineData(1, 0)]
        [InlineData(1, 2)]
        [InlineData(2, 0)]
        [InlineData(2, 1)]
        [InlineData(2, 2)]
        public void IsAdjacentTo_Should_BeTrueWhenCellIsInSameOrAdjacentColumnAndRow(int column, int row)
        {
            new Cell(1, 1).IsAdjacentTo(new Cell(column, row)).Should().BeTrue();
        }
    }
}