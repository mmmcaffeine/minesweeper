using FluentAssertions;
using Xunit;

namespace Dgt.Minesweeper.Engine
{
    public class CellTests
    {
        [Theory]
        [InlineData(false, false, false)]
        [InlineData(true, false, false)]
        [InlineData(false, true, false)]
        [InlineData(true, true, true)]
        public void IsExploded_Should_BeTrueIfCellIsMinedAndRevealed
            (bool isMined, bool isRevealed, bool expectedIsExploded) =>
            new Cell(Location.Parse("A1"), isMined, isRevealed, 0).IsExploded.Should().Be(expectedIsExploded);
    }
}