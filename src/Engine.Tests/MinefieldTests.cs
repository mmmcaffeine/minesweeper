using FluentAssertions;
using Xunit;

namespace Dgt.Minesweeper.Engine
{
    public class MinefieldTests
    {
        // TODO What should the hint for a mined square be?
        // The example input shows "3, 4" meaning 3 rows, amd 4 columns. Our origin is the bottom left, and our coordinate
        // system is 1-based i.e. how the vast majority of the population would think of a minesweeper grid
        [Fact]
        public void GetHint_Should_ReturnNumberOfAdjacentMines()
        {
            var minefield = new Minefield(3, 4, new[] { new Square(3, 1), new Square(2, 3) });

            var hint = minefield.GetHint(3, 2);

            hint.Should().Be(2);
        }
    }
}