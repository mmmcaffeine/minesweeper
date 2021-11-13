using System;
using System.Collections.Generic;
using System.Linq;
using Dgt.Minesweeper.Engine;
using FakeItEasy;
using FluentAssertions;
using Xunit;

namespace Dgt.Minesweeper.ConsoleUI
{
    public class NaiveGameRendererTests
    {
        [Fact]
        public void Ctor_Should_ThrowWhenGameIsNull()
        {
            // Arrange
            var fakeRowRenderer = A.Fake<IRowRenderer>();
            var fakeCellRenderer = A.Fake<ICellRenderer>();
            
            // Act
            Action act = () => _ = new NaiveGameRenderer(null!, fakeRowRenderer, fakeCellRenderer);
            
            // Assert
            act.Should().Throw<ArgumentNullException>().WithParameterName("game");
        }

        [Fact]
        public void Ctor_Should_ThrowWhenRowRendererIsNull()
        {
            // Arrange
            var fakeMinefield = A.Fake<IMinefield>();
            var game = new Game(fakeMinefield);
            var fakeCellRenderer = A.Fake<ICellRenderer>();
            
            // Act
            Action act = () => _ = new NaiveGameRenderer(game, null!, fakeCellRenderer);
            
            // Assert
            act.Should().Throw<ArgumentNullException>().WithParameterName("rowRenderer");
        }

        [Fact]
        public void Ctor_Should_ThrowWhenCellRendererIsNull()
        {
            // Arrange
            var fakeMinefield = A.Fake<IMinefield>();
            var game = new Game(fakeMinefield);
            var fakeRowRenderer = A.Fake<IRowRenderer>();

            // Act
            Action act = () => _ = new NaiveGameRenderer(game, fakeRowRenderer, null!);

            // Assert
            act.Should().Throw<ArgumentNullException>().WithParameterName("cellRenderer");
        }
        
        // These kind of tests (with lots of interactions) are often quite brittle, but there doesn't
        // immediately seem to be a whole lot we can do about it
        [Fact]
        public void Render_Should_RenderCurrentGame()
        {
            // Arrange
            const int numberOfColumns = 5;
            const int numberOfRows = 3;
            
            var minefield = new Minefield(numberOfColumns, numberOfRows, Array.Empty<Location>());
            var game = new Game(minefield);
            
            var fakeRowRenderer = A.Fake<IRowRenderer>();
            A.CallTo(() => fakeRowRenderer.RenderTopBorder(numberOfRows, numberOfColumns)).Returns("Top Border");
            A.CallTo(() => fakeRowRenderer.RenderRow(numberOfRows, A<int>._, A<ICellRenderer>._, An<IEnumerable<Cell>>._))
                .ReturnsNextFromSequence("Row 3", "Row 2", "Row 1");
            A.CallTo(() => fakeRowRenderer.RenderRowSeparator(numberOfRows, numberOfColumns)).Returns("Row Separator");
            A.CallTo(() => fakeRowRenderer.RenderBottomBorder(numberOfRows, numberOfColumns)).Returns("Bottom Border");
            A.CallTo(() => fakeRowRenderer.RenderColumnNames(numberOfRows, An<IEnumerable<ColumnName>>._))
                .Returns(new[] { "Column Names" });

            var fakeCellRenderer = A.Fake<ICellRenderer>();

            var sut = new NaiveGameRenderer(game, fakeRowRenderer, fakeCellRenderer);
            
            // Assert
            var renderedGame = sut.Render().ToList();

            // Assert
             var expectedRows = new[]
            {
                "Top Border",
                "Row 3",
                "Row Separator",
                "Row 2",
                "Row Separator",
                "Row 1",
                "Bottom Border",
                string.Empty,
                "Column Names"
            };
            renderedGame.Should().HaveSameCount(expectedRows).And.ContainInOrder(expectedRows);
        }
    }
}