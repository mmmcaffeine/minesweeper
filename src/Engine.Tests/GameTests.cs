using FakeItEasy;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;

namespace Dgt.Minesweeper.Engine
{
    public class GameTests
    {
        [Fact]
        public void Ctor_Should_InitialiseCellsThatAreMinedToMinedCellState()
        {
            // Arrange
            var minedCell = new Cell(2, 2);
            
            var fakeMinefield = A.Fake<IMinefield>();
            A.CallTo(() => fakeMinefield.NumberOfColumns).Returns(4);
            A.CallTo(() => fakeMinefield.NumberOfRows).Returns(4);
            A.CallTo(() => fakeMinefield.IsMined(A<Cell>._)).Returns(false);
            A.CallTo(() => fakeMinefield.IsMined(minedCell)).Returns(true);

            // Act
            var sut = new Game(fakeMinefield);
            
            // Assert
            sut.GetCellState(minedCell).Should().Be(CellState.Mined);
        }

        [Fact]
        public void Ctor_Should_InitialiseCellsThatAreNotMinedToUnclearedCellState()
        {
            // Arrange
            var fakeMinefield = A.Fake<IMinefield>();
            A.CallTo(() => fakeMinefield.NumberOfColumns).Returns(2);
            A.CallTo(() => fakeMinefield.NumberOfRows).Returns(2);
            A.CallTo(() => fakeMinefield.IsMined(A<Cell>._)).Returns(false);
            
            // Act
            var sut = new Game(fakeMinefield);
            
            // Assert
            using (new AssertionScope())
            {
                sut.GetCellState(new Cell(0, 0)).Should().Be(CellState.Uncleared);
                sut.GetCellState(new Cell(0, 1)).Should().Be(CellState.Uncleared);
                sut.GetCellState(new Cell(1, 0)).Should().Be(CellState.Uncleared);
                sut.GetCellState(new Cell(1, 1)).Should().Be(CellState.Uncleared);
            }
        }
    }
}