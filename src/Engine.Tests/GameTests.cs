using System.Linq;
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
            var enumerator = Enumerable.Range(0, 4)
                .SelectMany(_ => Enumerable.Range(0, 4), (column, row) => new Cell(column, row))
                .GetEnumerator();
            
            var fakeMinefield = A.Fake<IMinefield>();
            A.CallTo(() => fakeMinefield.NumberOfColumns).Returns(4);
            A.CallTo(() => fakeMinefield.NumberOfRows).Returns(4);
            A.CallTo(() => fakeMinefield.IsMined(A<Cell>._)).Returns(false);
            A.CallTo(() => fakeMinefield.IsMined(minedCell)).Returns(true);
            A.CallTo(() => fakeMinefield.GetEnumerator()).Returns(enumerator);

            // Act
            var sut = new Game(fakeMinefield);
            
            // Assert
            sut.GetCellState(minedCell).Should().Be(CellState.Mined);
        }

        [Fact]
        public void Ctor_Should_InitialiseCellsThatAreNotMinedToUnclearedCellState()
        {
            // Arrange
            var enumerator = Enumerable.Range(0, 2)
                .SelectMany(_ => Enumerable.Range(0, 2), (column, row) => new Cell(column, row))
                .GetEnumerator();
            
            var fakeMinefield = A.Fake<IMinefield>();
            A.CallTo(() => fakeMinefield.NumberOfColumns).Returns(2);
            A.CallTo(() => fakeMinefield.NumberOfRows).Returns(2);
            A.CallTo(() => fakeMinefield.IsMined(A<Cell>._)).Returns(false);
            A.CallTo(() => fakeMinefield.GetEnumerator()).Returns(enumerator);

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

        [Fact]
        public void Ctor_Should_InitialiseGameToStateThatIsNotWonAndNotLost()
        {
            // Arrange
            var minedCell = new Cell(2, 2);
            var enumerator = Enumerable.Range(0, 4)
                .SelectMany(_ => Enumerable.Range(0, 4), (column, row) => new Cell(column, row))
                .GetEnumerator();
            
            var fakeMinefield = A.Fake<IMinefield>();
            A.CallTo(() => fakeMinefield.NumberOfColumns).Returns(4);
            A.CallTo(() => fakeMinefield.NumberOfRows).Returns(4);
            A.CallTo(() => fakeMinefield.IsMined(A<Cell>._)).Returns(false);
            A.CallTo(() => fakeMinefield.IsMined(minedCell)).Returns(true);
            A.CallTo(() => fakeMinefield.GetEnumerator()).Returns(enumerator);
            
            // Act
            var sut = new Game(fakeMinefield);
            
            // Assert
            using (new AssertionScope())
            {
                sut.IsWon.Should().BeFalse();
                sut.IsLost.Should().BeFalse();
            }
        }
        
        [Fact]
        public void Reveal_Should_PutMinedCellIntoExplodedState()
        {
            // Arrange
            var minedCell = new Cell(2, 2);
            var enumerator = Enumerable.Range(0, 4)
                .SelectMany(_ => Enumerable.Range(0, 4), (column, row) => new Cell(column, row))
                .GetEnumerator();
            
            var fakeMinefield = A.Fake<IMinefield>();
            A.CallTo(() => fakeMinefield.NumberOfColumns).Returns(4);
            A.CallTo(() => fakeMinefield.NumberOfRows).Returns(4);
            A.CallTo(() => fakeMinefield.IsMined(A<Cell>._)).Returns(false);
            A.CallTo(() => fakeMinefield.IsMined(minedCell)).Returns(true);
            A.CallTo(() => fakeMinefield.GetEnumerator()).Returns(enumerator);

            var sut = new Game(fakeMinefield);
            
            // Act
            var returnedCellState = sut.Reveal(minedCell);
            var retrievedCellState = sut.GetCellState(minedCell);
            
            // Assert
            using (new AssertionScope())
            {
                returnedCellState.Should().Be(CellState.Exploded);
                retrievedCellState.Should().Be(CellState.Exploded);
            }
        }
        
        [Fact]
        public void Reveal_Should_PutNotMinedCellIntoClearedState()
        {
            // Arrange
            var minedCell = new Cell(2, 2);
            var notMinedCell = new Cell(1, 1);
            var enumerator = Enumerable.Range(0, 4)
                .SelectMany(_ => Enumerable.Range(0, 4), (column, row) => new Cell(column, row))
                .GetEnumerator();
            
            var fakeMinefield = A.Fake<IMinefield>();
            A.CallTo(() => fakeMinefield.NumberOfColumns).Returns(4);
            A.CallTo(() => fakeMinefield.NumberOfRows).Returns(4);
            A.CallTo(() => fakeMinefield.IsMined(A<Cell>._)).Returns(false);
            A.CallTo(() => fakeMinefield.IsMined(minedCell)).Returns(true);
            A.CallTo(() => fakeMinefield.GetEnumerator()).Returns(enumerator);

            var sut = new Game(fakeMinefield);
            
            // Act
            var returnedCellState = sut.Reveal(notMinedCell);
            var retrievedCellState = sut.GetCellState(notMinedCell);
            
            // Assert
            using (new AssertionScope())
            {
                returnedCellState.Should().Be(CellState.Cleared);
                retrievedCellState.Should().Be(CellState.Cleared);
            }
        }

        [Fact]
        public void Reveal_Should_LoseGameIfRevealedCellIsMined()
        {
            // Arrange
            var minedCell = new Cell(2, 2);
            var enumerator = Enumerable.Range(0, 4)
                .SelectMany(_ => Enumerable.Range(0, 4), (column, row) => new Cell(column, row))
                .GetEnumerator();
            
            var fakeMinefield = A.Fake<IMinefield>();
            A.CallTo(() => fakeMinefield.NumberOfColumns).Returns(4);
            A.CallTo(() => fakeMinefield.NumberOfRows).Returns(4);
            A.CallTo(() => fakeMinefield.IsMined(A<Cell>._)).Returns(false);
            A.CallTo(() => fakeMinefield.IsMined(minedCell)).Returns(true);
            A.CallTo(() => fakeMinefield.GetEnumerator()).Returns(enumerator);

            var sut = new Game(fakeMinefield);
            
            // Act
            _ = sut.Reveal(minedCell);
            
            // Assert
            using (new AssertionScope())
            {
                sut.IsWon.Should().BeFalse();
                sut.IsLost.Should().BeTrue();
            }
        }

        [Fact]
        public void Reveal_Should_NotLoseGameIfRevealedCellIsNotMined()
        {
            // Arrange
            var minedCell = new Cell(2, 2);
            var notMinedCell = new Cell(1, 1);
            var enumerator = Enumerable.Range(0, 4)
                .SelectMany(_ => Enumerable.Range(0, 4), (column, row) => new Cell(column, row))
                .GetEnumerator();
            
            var fakeMinefield = A.Fake<IMinefield>();
            A.CallTo(() => fakeMinefield.NumberOfColumns).Returns(4);
            A.CallTo(() => fakeMinefield.NumberOfRows).Returns(4);
            A.CallTo(() => fakeMinefield.IsMined(A<Cell>._)).Returns(false);
            A.CallTo(() => fakeMinefield.IsMined(minedCell)).Returns(true);
            A.CallTo(() => fakeMinefield.GetEnumerator()).Returns(enumerator);

            var sut = new Game(fakeMinefield);
            
            // Act
            _ = sut.Reveal(notMinedCell);
            
            // Assert
            using (new AssertionScope())
            {
                sut.IsWon.Should().BeFalse();
                sut.IsLost.Should().BeFalse();
            }
        }

        [Fact]
        public void Reveal_Should_WinGameIfAllNotMinedCellsAreNowRevealed()
        {
            // Arrange
            var minedCell = new Cell(0, 0);
            var notMinedCells = new[]
            {
                new Cell(0, 1),
                new Cell(1, 0),
                new Cell(1, 1)
            };
            var enumerator = Enumerable.Range(0, 2)
                .SelectMany(_ => Enumerable.Range(0, 2), (column, row) => new Cell(column, row))
                .GetEnumerator();
            
            var fakeMinefield = A.Fake<IMinefield>();
            A.CallTo(() => fakeMinefield.NumberOfColumns).Returns(2);
            A.CallTo(() => fakeMinefield.NumberOfRows).Returns(2);
            A.CallTo(() => fakeMinefield.IsMined(A<Cell>._)).Returns(false);
            A.CallTo(() => fakeMinefield.IsMined(minedCell)).Returns(true);
            A.CallTo(() => fakeMinefield.GetEnumerator()).Returns(enumerator);

            var sut = new Game(fakeMinefield);
            
            // Act
            foreach (var cell in notMinedCells)
            {
                _ = sut.Reveal(cell);
            }

            // Assert
            using (new AssertionScope())
            {
                sut.IsWon.Should().BeTrue();
                sut.IsLost.Should().BeFalse();
            }
        }
    }
}