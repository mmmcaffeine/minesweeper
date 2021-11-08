using System;
using Dgt.Minesweeper.Engine;
using FakeItEasy;
using FluentAssertions;
using Xunit;

namespace Dgt.Minesweeper.ConsoleUI
{
    public class GameRendererTests
    {
        [Fact]
        public void Ctor_Should_ThrowWhenCellRendererIsNull()
        {
            // Arrange, Act
            Action act = () => _ = new GameRenderer(null!);
            
            // Assert
            act.Should().Throw<ArgumentNullException>().WithParameterName("cellRenderer");
        }

        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(-1)]
        [InlineData(0)]
        public void RenderRow_Should_ThrowIfRowIndexIsNotPositiveNonZero(int rowIndex)
        {
            // Arrange
            var fakeCellRenderer = A.Fake<ICellRenderer>();
            var sut = new GameRenderer(fakeCellRenderer);
            
            // Act
            Action act = () => sut.RenderRow(rowIndex, int.MaxValue, Array.Empty<Cell>());
            
            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>()
                .WithMessage("Value must be a positive, non-zero integer.*")
                .WithParameterName("rowIndex")
                .Where(ex => ex.ActualValue!.Equals(rowIndex));
        }

        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(-1)]
        [InlineData(0)]
        public void RenderRow_Should_ThrowIfNumberOfRowsIsNotPositiveNonZero(int numberOfRows)
        {
            // Arrange
            var fakeCellRenderer = A.Fake<ICellRenderer>();
            var sut = new GameRenderer(fakeCellRenderer);
            
            // Act
            Action act = () => sut.RenderRow(1, numberOfRows, Array.Empty<Cell>());
            
            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>()
                .WithMessage("Value must be a positive, non-zero integer.*")
                .WithParameterName("numberOfRows")
                .Where(ex => ex.ActualValue!.Equals(numberOfRows));
        }

        [Fact]
        public void RenderRow_Should_ThrowIfRowIndexIsGreaterThanNumberOfRows()
        {
            // Arrange
            var fakeCellRenderer = A.Fake<ICellRenderer>();
            var sut = new GameRenderer(fakeCellRenderer);
            
            // Act
            Action act = () => sut.RenderRow(2, 1, Array.Empty<Cell>());
            
            // Assert
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("The row index must be less than the number of rows.")
                .Where(ex => ex.Data.Contains("rowIndex") && ex.Data["rowIndex"]!.Equals(2))
                .Where(ex => ex.Data.Contains("numberOfRows") && ex.Data["numberOfRows"]!.Equals(1));
        }

        [Theory]
        [InlineData(1, 8, "1 ")]
        [InlineData(3, 15, " 3 ")]
        [InlineData(9, 100, "  9 ")]
        [InlineData(27, 100, " 27 ")]
        [InlineData(100, 100, "100 ")]
        public void RenderRow_Should_StartWithRightAlignedRowIndex(int rowIndex, int numberOfRows, string expected)
        {
            // Arrange
            var fakeCellRenderer = A.Fake<ICellRenderer>();
            var sut = new GameRenderer(fakeCellRenderer);

            A.CallTo(() => fakeCellRenderer.RenderCell(A<Cell>._)).Returns('.');

            // Act
            var renderedRow = sut.RenderRow(rowIndex, numberOfRows, Array.Empty<Cell>()).ToString();

            // Assert
            renderedRow.Should().StartWith(expected);
        }
    }
}