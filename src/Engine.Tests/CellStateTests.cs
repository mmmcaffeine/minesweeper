using FluentAssertions;
using Xunit;

namespace Dgt.Minesweeper.Engine
{
    public class CellStateTests
    {
        [Fact]
        public void MinedCellState_Should_NotBeEqualToUnclearedCellState()
        {
            CellState.Mined.Should().NotBe(CellState.Uncleared);
        }

        [Fact]
        public void MinedCellState_Should_BeEqualToAnotherMinedCellState()
        {
            new CellState("Mined").Should().Be(CellState.Mined);
        }
    }
}