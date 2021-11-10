using System;
using System.Linq;
using Dgt.Minesweeper.Engine;
using FakeItEasy;
using FluentAssertions;
using Xunit;

namespace Dgt.Minesweeper.ConsoleUI
{
    public class NaiveRowRendererTests
    {
        [Fact]
        public void Ctor_Should_ThrowWhenCellRendererIsNull()
        {
            // Arrange, Act
            Action act = () => _ = new NaiveRowRenderer(null!);
            
            // Assert
            act.Should().Throw<ArgumentNullException>().WithParameterName("cellRenderer");
        }
        
        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(-1)]
        [InlineData(0)]
        public void RenderTopBorder_Should_ThrowIfNumberOfRowsIsNotPositiveNonZero(int numberOfRows)
        {
            // Arrange
            var fakeCellRenderer = A.Fake<ICellRenderer>();
            var sut = new NaiveRowRenderer(fakeCellRenderer);
            
            // Act
            Action act = () => sut.RenderTopBorder(numberOfRows, 5);
            
            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>()
                .WithMessage("Value must be a positive, non-zero integer.*")
                .WithParameterName("numberOfRows")
                .Where(ex => ex.ActualValue!.Equals(numberOfRows));
        }
        
        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(-1)]
        [InlineData(0)]
        public void RenderTopBorder_Should_ThrowIfNumberOfColumnsIsNotPositiveNonZero(int numberOfColumns)
        {
            // Arrange
            var fakeCellRenderer = A.Fake<ICellRenderer>();
            var sut = new NaiveRowRenderer(fakeCellRenderer);
            
            // Act
            Action act = () => sut.RenderTopBorder(5, numberOfColumns);
            
            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>()
                .WithMessage("Value must be a positive, non-zero integer.*")
                .WithParameterName("numberOfColumns")
                .Where(ex => ex.ActualValue!.Equals(numberOfColumns));
        }
        
        [Fact]
        public void RenderTopBorder_Should_RenderBoxArtWithSpacesForRowNumbers()
        {
            // Arrange
            var fakeCellRenderer = A.Fake<ICellRenderer>();
            var sut = new NaiveRowRenderer(fakeCellRenderer);

            // Act
            var topBorder = sut.RenderTopBorder(10, 3).ToString();

            // Assert
            topBorder.Should().Be("   ╔═╦═╦═╗");
        }

        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(-1)]
        [InlineData(0)]
        public void RenderRow_Should_ThrowIfRowIndexIsNotPositiveNonZero(int rowIndex)
        {
            // Arrange
            var fakeCellRenderer = A.Fake<ICellRenderer>();
            var sut = new NaiveRowRenderer(fakeCellRenderer);
            
            // Act
            Action act = () => sut.RenderRow(int.MaxValue, rowIndex, Array.Empty<Cell>());
            
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
            var sut = new NaiveRowRenderer(fakeCellRenderer);
            
            // Act
            Action act = () => sut.RenderRow(numberOfRows, 1, Array.Empty<Cell>());
            
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
            var sut = new NaiveRowRenderer(fakeCellRenderer);
            
            // Act
            Action act = () => sut.RenderRow(1, 2, Array.Empty<Cell>());
            
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
            var sut = new NaiveRowRenderer(fakeCellRenderer);

            A.CallTo(() => fakeCellRenderer.RenderCell(A<Cell>._)).Returns('.');

            // Act
            var renderedRow = sut.RenderRow(numberOfRows, rowIndex, Array.Empty<Cell>()).ToString();

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
            var sut = new NaiveRowRenderer(fakeCellRenderer);
            
            // Act
            var renderedRow = sut.RenderRow(3, 1, cells).ToString();
            
            // Assert
            renderedRow.Should().EndWith(" ║1║2║3║4║5║");
        }
        
        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(-1)]
        [InlineData(0)]
        public void RenderRowSeparator_Should_ThrowIfNumberOfRowsIsNotPositiveNonZero(int numberOfRows)
        {
            // Arrange
            var fakeCellRenderer = A.Fake<ICellRenderer>();
            var sut = new NaiveRowRenderer(fakeCellRenderer);
            
            // Act
            Action act = () => sut.RenderRowSeparator(numberOfRows, 5);
            
            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>()
                .WithMessage("Value must be a positive, non-zero integer.*")
                .WithParameterName("numberOfRows")
                .Where(ex => ex.ActualValue!.Equals(numberOfRows));
        }
        
        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(-1)]
        [InlineData(0)]
        public void RenderRowSeparator_Should_ThrowIfNumberOfColumnsIsNotPositiveNonZero(int numberOfColumns)
        {
            // Arrange
            var fakeCellRenderer = A.Fake<ICellRenderer>();
            var sut = new NaiveRowRenderer(fakeCellRenderer);
            
            // Act
            Action act = () => sut.RenderRowSeparator(5, numberOfColumns);
            
            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>()
                .WithMessage("Value must be a positive, non-zero integer.*")
                .WithParameterName("numberOfColumns")
                .Where(ex => ex.ActualValue!.Equals(numberOfColumns));
        }
        
        [Fact]
        public void RenderRowSeparator_Should_RenderBoxArtWithSpacesForRowNumbers()
        {
            // Arrange
            var fakeCellRenderer = A.Fake<ICellRenderer>();
            var sut = new NaiveRowRenderer(fakeCellRenderer);

            // Act
            var rowSeparator = sut.RenderRowSeparator(10, 3).ToString();

            // Assert
            rowSeparator.Should().Be("   ╠═╬═╬═╣");
        }
        
        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(-1)]
        [InlineData(0)]
        public void RenderBottomBorder_Should_ThrowIfNumberOfRowsIsNotPositiveNonZero(int numberOfRows)
        {
            // Arrange
            var fakeCellRenderer = A.Fake<ICellRenderer>();
            var sut = new NaiveRowRenderer(fakeCellRenderer);
            
            // Act
            Action act = () => sut.RenderBottomBorder(numberOfRows, 5);
            
            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>()
                .WithMessage("Value must be a positive, non-zero integer.*")
                .WithParameterName("numberOfRows")
                .Where(ex => ex.ActualValue!.Equals(numberOfRows));
        }
        
        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(-1)]
        [InlineData(0)]
        public void RenderBottomBorder_Should_ThrowIfNumberOfColumnsIsNotPositiveNonZero(int numberOfColumns)
        {
            // Arrange
            var fakeCellRenderer = A.Fake<ICellRenderer>();
            var sut = new NaiveRowRenderer(fakeCellRenderer);
            
            // Act
            Action act = () => sut.RenderBottomBorder(5, numberOfColumns);
            
            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>()
                .WithMessage("Value must be a positive, non-zero integer.*")
                .WithParameterName("numberOfColumns")
                .Where(ex => ex.ActualValue!.Equals(numberOfColumns));
        }
        
        [Fact]
        public void RenderBottomBorder_Should_RenderBoxArtWithSpacesForRowNumbers()
        {
            // Arrange
            var fakeCellRenderer = A.Fake<ICellRenderer>();
            var sut = new NaiveRowRenderer(fakeCellRenderer);

            // Act
            var bottomBorder = sut.RenderBottomBorder(10, 3).ToString();

            // Assert
            bottomBorder.Should().Be("   ╚═╩═╩═╝");
        }
        
        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(-1)]
        [InlineData(0)]
        public void RenderColumnNames_Should_ThrowIfNumberOfRowsIsNotPositiveNonZero(int numberOfRows)
        {
            // Arrange
            var fakeCellRenderer = A.Fake<ICellRenderer>();
            var sut = new NaiveRowRenderer(fakeCellRenderer);
            
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
            var sut = new NaiveRowRenderer(fakeCellRenderer);
            
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
            var sut = new NaiveRowRenderer(fakeCellRenderer);

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