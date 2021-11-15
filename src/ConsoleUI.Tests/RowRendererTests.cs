using System;
using System.Collections.Generic;
using System.Linq;
using Dgt.Minesweeper.Engine;
using FakeItEasy;
using FluentAssertions;
using Xunit;

namespace Dgt.Minesweeper.ConsoleUI
{
    public class RowRendererTests
    {
        private static IEnumerable<IRowRenderer> RowRenderers
        {
            get
            {
                yield return new NaiveRowRenderer();
                yield return new StringCreateGameRenderer();
                yield return new CharArrayGameRenderer();
            }
        }

        private static IEnumerable<int> NotPositiveNumbers
        {
            get
            {
                yield return int.MinValue;
                yield return -1;
                yield return 0;
            }
        }

        private static IEnumerable<(int RowIndex, int NumberOfRows, string RowPrefix)> RowPrefixes
        {
            get
            {
                {
                    yield return (1, 8, "1 ");
                    yield return (3, 15, " 3 ");
                    yield return (9, 100, "  9 ");
                    yield return (27, 100, " 27 ");
                    yield return (100, 100, "100 ");
                }
            }
        }

        public static IEnumerable<object[]> RowRendererTestData =>
            RowRenderers.Select(renderer => new object[] { renderer });

        public static IEnumerable<object[]> NotPositiveNumbersTestData =>
            from rowRenderer in RowRenderers
            from number in NotPositiveNumbers
            select new object[] { rowRenderer, number };

        public static IEnumerable<object[]> RowPrefixTestData =>
            from rowRenderer in RowRenderers
            from tuple in RowPrefixes
            select new object[] { rowRenderer, tuple.RowIndex, tuple.NumberOfRows, tuple.RowPrefix };

        [Theory]
        [MemberData(nameof(NotPositiveNumbersTestData))]
        public void RenderTopBorder_Should_ThrowIfNumberOfRowsIsNotPositiveNonZero(IRowRenderer sut, int numberOfRows)
        {
            // Arrange
            // All handled by sut parameter

            // Act
            Action act = () => sut.RenderTopBorder(numberOfRows, 5);
            
            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>()
                .WithMessage("Value must be a positive, non-zero integer.*")
                .WithParameterName("numberOfRows")
                .Where(ex => ex.ActualValue!.Equals(numberOfRows));
        }
        
        [Theory]
        [MemberData(nameof(NotPositiveNumbersTestData))]
        public void RenderTopBorder_Should_ThrowIfNumberOfColumnsIsNotPositiveNonZero(IRowRenderer sut, int numberOfColumns)
        {
            // Arrange
            // All handled by sut parameter

            // Act
            Action act = () => sut.RenderTopBorder(5, numberOfColumns);
            
            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>()
                .WithMessage("Value must be a positive, non-zero integer.*")
                .WithParameterName("numberOfColumns")
                .Where(ex => ex.ActualValue!.Equals(numberOfColumns));
        }
        
        [Theory]
        [MemberData(nameof(RowRendererTestData))]
        public void RenderTopBorder_Should_RenderBoxArtWithSpacesForRowNumbers(IRowRenderer sut)
        {
            // Arrange
            // All handled by sut parameter

            // Act
            var topBorder = sut.RenderTopBorder(10, 3);

            // Assert
            topBorder.Should().Be("   ╔═╦═╦═╗");
        }

        [Theory]
        [MemberData(nameof(NotPositiveNumbersTestData))]
        public void RenderRow_Should_ThrowIfRowIndexIsNotPositiveNonZero(IRowRenderer sut, int rowIndex)
        {
            // Arrange
            var fakeCellRenderer = A.Fake<ICellRenderer>();

            // Act
            Action act = () => sut.RenderRow(int.MaxValue, rowIndex, fakeCellRenderer, Array.Empty<Cell>());
            
            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>()
                .WithMessage("Value must be a positive, non-zero integer.*")
                .WithParameterName("rowIndex")
                .Where(ex => ex.ActualValue!.Equals(rowIndex));
        }

        [Theory]
        [MemberData(nameof(NotPositiveNumbersTestData))]
        public void RenderRow_Should_ThrowIfNumberOfRowsIsNotPositiveNonZero(IRowRenderer sut, int numberOfRows)
        {
            // Arrange
            var fakeCellRenderer = A.Fake<ICellRenderer>();

            // Act
            Action act = () => sut.RenderRow(numberOfRows, 1, fakeCellRenderer, Array.Empty<Cell>());
            
            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>()
                .WithMessage("Value must be a positive, non-zero integer.*")
                .WithParameterName("numberOfRows")
                .Where(ex => ex.ActualValue!.Equals(numberOfRows));
        }

        [Theory]
        [MemberData(nameof(RowRendererTestData))]
        public void RenderRow_Should_ThrowIfRowIndexIsGreaterThanNumberOfRows(IRowRenderer sut)
        {
            // Arrange
            var fakeCellRenderer = A.Fake<ICellRenderer>();

            // Act
            Action act = () => sut.RenderRow(1, 2, fakeCellRenderer, Array.Empty<Cell>());
            
            // Assert
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("The row index must be less than the number of rows.")
                .Where(ex => ex.Data.Contains("rowIndex") && ex.Data["rowIndex"]!.Equals(2))
                .Where(ex => ex.Data.Contains("numberOfRows") && ex.Data["numberOfRows"]!.Equals(1));
        }

        [Theory]
        [MemberData(nameof(RowRendererTestData))]
        public void RenderRow_Should_ThrowIfCellRendererIsNull(IRowRenderer sut)
        {
            // Arrange
            // All handled by sut parameter

            // Act
            Action act = () => sut.RenderRow(10, 5, null!, Array.Empty<Cell>());

            // Assert
            act.Should().Throw<ArgumentNullException>().WithParameterName("cellRenderer");
        }

        [Theory]
        [MemberData(nameof(RowRendererTestData))]
        public void RenderRow_Should_ThrowIfCellsIsNull(IRowRenderer sut)
        {
            // Arrange
            var fakeCellRenderer = A.Fake<ICellRenderer>();

            // Act
            Action act = () => _ = sut.RenderRow(10, 5, fakeCellRenderer, null!);

            // Assert
            act.Should().Throw<ArgumentNullException>().WithParameterName("cells");
        }

        [Theory]
        [MemberData(nameof(RowPrefixTestData))]
        public void RenderRow_Should_StartWithRightAlignedRowIndex(IRowRenderer sut, int rowIndex, int numberOfRows, string expected)
        {
            // Arrange
            var fakeCellRenderer = A.Fake<ICellRenderer>();
            A.CallTo(() => fakeCellRenderer.RenderCell(A<Cell>._)).Returns('.');

            // Act
            var renderedRow = sut.RenderRow(numberOfRows, rowIndex, fakeCellRenderer, Array.Empty<Cell>());

            // Assert
            renderedRow.Should().StartWith(expected);
        }

        // Alt+186 (Unicode 2551 and 9553 as an int) for double pipe, but this might look better with
        // the single pipe of Alt+179 (Unicode 2502 and 9474 as an int)
        [Theory]
        [MemberData(nameof(RowRendererTestData))]
        public void RenderRow_Should_EndWithCellsRenderedInSequenceAndDelimitedByDoublePipes(IRowRenderer sut)
        {
            // Arrange
            var fakeCellRenderer = A.Fake<ICellRenderer>();
            A.CallTo(() => fakeCellRenderer.RenderCell(A<Cell>._)).ReturnsNextFromSequence('1', '2', '3', '4', '5');

            var cells = new[] { "A1", "B1", "C1", "D1", "E1" }.Select(s => new Cell(Location.Parse(s), false, 0));

            // Act
            var renderedRow = sut.RenderRow(3, 1, fakeCellRenderer, cells);
            
            // Assert
            renderedRow.Should().EndWith(" ║1║2║3║4║5║");
        }
        
        [Theory]
        [MemberData(nameof(NotPositiveNumbersTestData))]
        public void RenderRowSeparator_Should_ThrowIfNumberOfRowsIsNotPositiveNonZero(IRowRenderer sut, int numberOfRows)
        {
            // Arrange
            // All handled by sut parameter

            // Act
            Action act = () => sut.RenderRowSeparator(numberOfRows, 5);
            
            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>()
                .WithMessage("Value must be a positive, non-zero integer.*")
                .WithParameterName("numberOfRows")
                .Where(ex => ex.ActualValue!.Equals(numberOfRows));
        }
        
        [Theory]
        [MemberData(nameof(NotPositiveNumbersTestData))]
        public void RenderRowSeparator_Should_ThrowIfNumberOfColumnsIsNotPositiveNonZero(IRowRenderer sut, int numberOfColumns)
        {
            // Arrange
            // All handled by sut parameter
            
            // Act
            Action act = () => sut.RenderRowSeparator(5, numberOfColumns);
            
            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>()
                .WithMessage("Value must be a positive, non-zero integer.*")
                .WithParameterName("numberOfColumns")
                .Where(ex => ex.ActualValue!.Equals(numberOfColumns));
        }
        
        [Theory]
        [MemberData(nameof(RowRendererTestData))]
        public void RenderRowSeparator_Should_RenderBoxArtWithSpacesForRowNumbers(IRowRenderer sut)
        {
            // Arrange
            // All handled by sut parameter

            // Act
            var rowSeparator = sut.RenderRowSeparator(10, 3);

            // Assert
            rowSeparator.Should().Be("   ╠═╬═╬═╣");
        }
        
        [Theory]
        [MemberData(nameof(NotPositiveNumbersTestData))]
        public void RenderBottomBorder_Should_ThrowIfNumberOfRowsIsNotPositiveNonZero(IRowRenderer sut, int numberOfRows)
        {
            // Arrange
            // All handled by sut parameter
            
            // Act
            Action act = () => sut.RenderBottomBorder(numberOfRows, 5);
            
            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>()
                .WithMessage("Value must be a positive, non-zero integer.*")
                .WithParameterName("numberOfRows")
                .Where(ex => ex.ActualValue!.Equals(numberOfRows));
        }
        
        [Theory]
        [MemberData(nameof(NotPositiveNumbersTestData))]
        public void RenderBottomBorder_Should_ThrowIfNumberOfColumnsIsNotPositiveNonZero(IRowRenderer sut, int numberOfColumns)
        {
            // Arrange
            // All handled by sut parameter
            
            // Act
            Action act = () => sut.RenderBottomBorder(5, numberOfColumns);
            
            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>()
                .WithMessage("Value must be a positive, non-zero integer.*")
                .WithParameterName("numberOfColumns")
                .Where(ex => ex.ActualValue!.Equals(numberOfColumns));
        }
        
        [Theory]
        [MemberData(nameof(RowRendererTestData))]
        public void RenderBottomBorder_Should_RenderBoxArtWithSpacesForRowNumbers(IRowRenderer sut)
        {
            // Arrange
            // All handled by sut parameter

            // Act
            var bottomBorder = sut.RenderBottomBorder(10, 3);

            // Assert
            bottomBorder.Should().Be("   ╚═╩═╩═╝");
        }
        
        [Theory]
        [MemberData(nameof(NotPositiveNumbersTestData))]
        public void RenderColumnNames_Should_ThrowIfNumberOfRowsIsNotPositiveNonZero(IRowRenderer sut, int numberOfRows)
        {
            // Arrange
            // All handled by sut parameter
            
            // Act
            Action act = () => _ = sut.RenderColumnNames(numberOfRows, Array.Empty<ColumnName>()).ToList();
            
            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>()
                .WithMessage("Value must be a positive, non-zero integer.*")
                .WithParameterName("numberOfRows")
                .Where(ex => ex.ActualValue!.Equals(numberOfRows));
        }

        [Theory]
        [MemberData(nameof(RowRendererTestData))]
        public void RenderColumnNames_ShouldThrowIfColumnNamesIsNull(IRowRenderer sut)
        {
            // Arrange
            // All handled by sut parameter

            // Act
            Action act = () => _ = sut.RenderColumnNames(10, null!).ToList();
            
            // Assert
            act.Should().Throw<ArgumentNullException>().WithParameterName("columnNames");
        }
        
        [Theory]
        [MemberData(nameof(RowRendererTestData))]
        public void RenderColumnNames_Should_RenderColumnNamesTopToBottomWithSpaceForRowNumbers(IRowRenderer sut)
        {
            // Arrange
            var columnNames = new[]
            {
                new ColumnName("A"), new ColumnName("ABCD"), new ColumnName("BB"), new ColumnName("CCC")
            };

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