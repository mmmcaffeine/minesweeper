using System;
using System.Linq;
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

        // Alt+186 (Unicode 2551 and 9553 as an int) for double pipe, but this might look better with
        // the single pipe of Alt+179 (Unicode 2502 and 9474 as an int)
        [Fact]
        public void RenderRow_Should_EndWithCellsRenderedInSequenceAndDelimitedByDoublePipes()
        {
            // Arrange
            var fakeCellRenderer = A.Fake<ICellRenderer>();
            A.CallTo(() => fakeCellRenderer.RenderCell(A<Cell>._)).ReturnsNextFromSequence('1', '2', '3', '4', '5');

            var cells = new[] { "A1", "B1", "C1", "D1", "E1" }.Select(s => new Cell(Location.Parse(s), false, 0));
            var sut = new GameRenderer(fakeCellRenderer);
            
            // Act
            var renderedRow = sut.RenderRow(1, 3, cells).ToString();
            
            // Assert
            renderedRow.Should().EndWith(" ║1║2║3║4║5║");
        }
        
        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(-1)]
        [InlineData(0)]
        public void RenderColumnNames_Should_ThrowIfNumberOfRowsIsNotPositiveNonZero(int numberOfRows)
        {
            // Arrange
            var fakeCellRenderer = A.Fake<ICellRenderer>();
            var sut = new GameRenderer(fakeCellRenderer);
            
            // Act
            Action act = () => _ = sut.RenderColumnNames(numberOfRows, Array.Empty<ColumnName>()).ToList();
            
            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>()
                .WithMessage("Value must be a positive, non-zero integer.*")
                .WithParameterName("numberOfRows")
                .Where(ex => ex.ActualValue!.Equals(numberOfRows));
        }

        [Fact]
        public void RenderColumnNames_ShouldThrowIfColumnNamesIsNull()
        {
            // Arrange
            var fakeCellRenderer = A.Fake<ICellRenderer>();
            var sut = new GameRenderer(fakeCellRenderer);
            
            // Act
            Action act = () => _ = sut.RenderColumnNames(10, null!).ToList();
            
            // Assert
            act.Should().Throw<ArgumentNullException>().WithParameterName("columnNames");
        }
        
        [Fact]
        public void RenderColumnNames_Should_RenderColumnNamesTopToBottomWithSpaceForRowNumbers()
        {
            // Arrange
            var columnNames = new[]
            {
                new ColumnName("A"), new ColumnName("ABCD"), new ColumnName("BB"), new ColumnName("CCC")
            };
            var fakeCellRenderer = A.Fake<ICellRenderer>();
            var sut = new GameRenderer(fakeCellRenderer);

            // Act
            var actual = sut.RenderColumnNames(10, columnNames).ToList();

            // Assert
            // We're looking to create a layout like
            //     A A B C
            //       B B C
            //       C   C
            //       D
            // With leading space for row numbers. In this case the row numbers go up to 10 so we need two spaces,
            // another space to separate the row numbers from the box art, and a final space for the left-hand
            // edge of the box art itself
            actual.Should().BeEquivalentTo("    A A B C", "      B B C", "      C   C", "      D    ");
        }
    }
}