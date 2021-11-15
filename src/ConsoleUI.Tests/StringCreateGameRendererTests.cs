using System;
using System.Linq;
using Dgt.Minesweeper.Engine;
using FakeItEasy;
using FluentAssertions;
using Xunit;

namespace Dgt.Minesweeper.ConsoleUI
{
    public class StringCreateGameRendererTests
    {
        [Fact]
        public void Ctor_Should_ThrowWhenGameIsNull()
        {
            // Arrange
            var fakeCellRenderer = A.Fake<ICellRenderer>();

            // Act
            Action act = () => _ = new StringCreateGameRenderer(null!, fakeCellRenderer);

            // Assert
            act.Should().Throw<ArgumentNullException>().WithParameterName("game");
        }

        [Fact]
        public void Ctor_Should_ThrowWhenCellRendererIsNull()
        {
            // Arrange
            var fakeMinefield = A.Fake<IMinefield>();
            var game = new Game(fakeMinefield);

            // Act
            Action act = () => _ = new StringCreateGameRenderer(game, null!);

            // Assert
            act.Should().Throw<ArgumentNullException>().WithParameterName("cellRenderer");
        }

        [Fact]
        public void Render_Should_RenderCurrentGame()
        {
            // Arrange
            var minefield = new Minefield(4, 3, Array.Empty<Location>());
            var game = new Game(minefield);

            var fakeCellRenderer = A.Fake<ICellRenderer>();
            A.CallTo(() => fakeCellRenderer.RenderCell(A<Cell>._)).Returns('.');

            var sut = new StringCreateGameRenderer(game, fakeCellRenderer);

            // Act
            var renderedGame = sut.Render().ToList();

            // Assert
            var expectedRows = new[]
            {
                "  ╔═╦═╦═╦═╗",
                "3 ║.║.║.║.║",
                "  ╠═╬═╬═╬═╣",
                "2 ║.║.║.║.║",
                "  ╠═╬═╬═╬═╣",
                "1 ║.║.║.║.║",
                "  ╚═╩═╩═╩═╝",
                string.Empty,
                "   A B C D"
            };
            renderedGame.Should().HaveSameCount(expectedRows).And.ContainInOrder(expectedRows);
        }
    }
}