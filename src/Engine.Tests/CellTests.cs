using FluentAssertions;
using Xunit;

namespace Dgt.Minesweeper.Engine
{
    public class CellTests
    {
        [Fact]
        public void Ctor_Should_InitialiseIsRevealedToFalse()
        {
            new Cell(Location.Parse("A1"), false, 0).IsRevealed.Should().BeFalse();
        }

        [Fact]
        public void Ctor_Should_InitialiseIsFlaggedToFalse()
        {
            new Cell(Location.Parse("A1"), false, 0).IsFlagged.Should().BeFalse();
        }
        
        [Theory]
        [InlineData(false, false, false)]
        [InlineData(true, false, false)]
        [InlineData(false, true, false)]
        [InlineData(true, true, true)]
        public void IsExploded_Should_BeTrueIfCellIsMinedAndRevealed
            (bool isMined, bool isRevealed, bool expectedIsExploded)
        {
            // Arrange, Act
            var sut = new Cell(Location.Parse("A1"), isMined, 0) { IsRevealed = isRevealed };

            // Assert
            sut.IsExploded.Should().Be(expectedIsExploded);
        }
    }
}