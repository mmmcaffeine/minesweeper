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
        public void Ctor_Should_InitialiseLocationsThatAreMinedToMinedCellState()
        {
            // Arrange
            var minedLocation = Location.Parse("B2");
            var enumerator = Enumerable.Range(0, 4)
                .SelectMany(_ => Enumerable.Range(0, 4), (column, row) => new Location((ColumnName)(column + 1), row + 1))
                .GetEnumerator();
            
            var fakeMinefield = A.Fake<IMinefield>();
            A.CallTo(() => fakeMinefield.NumberOfColumns).Returns(4);
            A.CallTo(() => fakeMinefield.NumberOfRows).Returns(4);
            A.CallTo(() => fakeMinefield.IsMined(A<Location>._)).Returns(false);
            A.CallTo(() => fakeMinefield.IsMined(minedLocation)).Returns(true);
            A.CallTo(() => fakeMinefield.GetEnumerator()).Returns(enumerator);

            // Act
            var sut = new Game(fakeMinefield);
            
            // Assert
            sut.GetCellState(minedLocation).Should().Be(CellState.Mined);
        }

        [Fact]
        public void Ctor_Should_InitialiseLocationsThatAreNotMinedToUnclearedCellState()
        {
            // Arrange
            var enumerator = Enumerable.Range(0, 2)
                .SelectMany(_ => Enumerable.Range(0, 2), (column, row) => new Location((ColumnName)(column + 1), row + 1))
                .GetEnumerator();
            
            var fakeMinefield = A.Fake<IMinefield>();
            A.CallTo(() => fakeMinefield.NumberOfColumns).Returns(2);
            A.CallTo(() => fakeMinefield.NumberOfRows).Returns(2);
            A.CallTo(() => fakeMinefield.IsMined(A<Location>._)).Returns(false);
            A.CallTo(() => fakeMinefield.GetEnumerator()).Returns(enumerator);
        
            // Act
            var sut = new Game(fakeMinefield);
            
            // Assert
            using (new AssertionScope())
            {
                sut.GetCellState(Location.Parse("A1")).Should().Be(CellState.Uncleared);
                sut.GetCellState(Location.Parse("A2")).Should().Be(CellState.Uncleared);
                sut.GetCellState(Location.Parse("B1")).Should().Be(CellState.Uncleared);
                sut.GetCellState(Location.Parse("B2")).Should().Be(CellState.Uncleared);
            }
        }
        
        [Fact]
        public void Ctor_Should_InitialiseGameToStateThatIsNotWonAndNotLost()
        {
            // Arrange
            var minedLocation = Location.Parse("B2");
            var enumerator = Enumerable.Range(0, 4)
                .SelectMany(_ => Enumerable.Range(0, 4), (column, row) => new Location((ColumnName)(column + 1), row + 1))
                .GetEnumerator();
            
            var fakeMinefield = A.Fake<IMinefield>();
            A.CallTo(() => fakeMinefield.NumberOfColumns).Returns(4);
            A.CallTo(() => fakeMinefield.NumberOfRows).Returns(4);
            A.CallTo(() => fakeMinefield.IsMined(A<Location>._)).Returns(false);
            A.CallTo(() => fakeMinefield.IsMined(minedLocation)).Returns(true);
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
        public void Reveal_Should_PutMinedLocationIntoExplodedState()
        {
            // Arrange
            var minedLocation = Location.Parse("B2");
            var enumerator = Enumerable.Range(0, 4)
                .SelectMany(_ => Enumerable.Range(0, 4), (column, row) => new Location((ColumnName)(column + 1), row + 1))
                .GetEnumerator();
            
            var fakeMinefield = A.Fake<IMinefield>();
            A.CallTo(() => fakeMinefield.NumberOfColumns).Returns(4);
            A.CallTo(() => fakeMinefield.NumberOfRows).Returns(4);
            A.CallTo(() => fakeMinefield.IsMined(A<Location>._)).Returns(false);
            A.CallTo(() => fakeMinefield.IsMined(minedLocation)).Returns(true);
            A.CallTo(() => fakeMinefield.GetEnumerator()).Returns(enumerator);
        
            var sut = new Game(fakeMinefield);
            
            // Act
            var returnedCellState = sut.Reveal(minedLocation);
            var retrievedCellState = sut.GetCellState(minedLocation);
            
            // Assert
            using (new AssertionScope())
            {
                returnedCellState.Should().Be(CellState.Exploded);
                retrievedCellState.Should().Be(CellState.Exploded);
            }
        }
        
        [Fact]
        public void Reveal_Should_PutNotMinedLocationIntoClearedState()
        {
            // Arrange
            var minedLocation = Location.Parse("B2");
            var notMinedLocation = Location.Parse("A1");
            var enumerator = Enumerable.Range(0, 4)
                .SelectMany(_ => Enumerable.Range(0, 4), (column, row) => new Location((ColumnName)(column + 1), row + 1))
                .GetEnumerator();
            
            var fakeMinefield = A.Fake<IMinefield>();
            A.CallTo(() => fakeMinefield.NumberOfColumns).Returns(4);
            A.CallTo(() => fakeMinefield.NumberOfRows).Returns(4);
            A.CallTo(() => fakeMinefield.IsMined(A<Location>._)).Returns(false);
            A.CallTo(() => fakeMinefield.IsMined(minedLocation)).Returns(true);
            A.CallTo(() => fakeMinefield.GetEnumerator()).Returns(enumerator);
        
            var sut = new Game(fakeMinefield);
            
            // Act
            var returnedCellState = sut.Reveal(notMinedLocation);
            var retrievedCellState = sut.GetCellState(notMinedLocation);
            
            // Assert
            using (new AssertionScope())
            {
                returnedCellState.Should().Be(CellState.Cleared);
                retrievedCellState.Should().Be(CellState.Cleared);
            }
        }
        
        [Fact]
        public void Reveal_Should_LoseGameIfRevealedLocationIsMined()
        {
            // Arrange
            var minedLocation = Location.Parse("B2");
            var enumerator = Enumerable.Range(0, 4)
                .SelectMany(_ => Enumerable.Range(0, 4), (column, row) => new Location((ColumnName)(column + 1), row + 1))
                .GetEnumerator();
            
            var fakeMinefield = A.Fake<IMinefield>();
            A.CallTo(() => fakeMinefield.NumberOfColumns).Returns(4);
            A.CallTo(() => fakeMinefield.NumberOfRows).Returns(4);
            A.CallTo(() => fakeMinefield.IsMined(A<Location>._)).Returns(false);
            A.CallTo(() => fakeMinefield.IsMined(minedLocation)).Returns(true);
            A.CallTo(() => fakeMinefield.GetEnumerator()).Returns(enumerator);
        
            var sut = new Game(fakeMinefield);
            
            // Act
            _ = sut.Reveal(minedLocation);
            
            // Assert
            using (new AssertionScope())
            {
                sut.IsWon.Should().BeFalse();
                sut.IsLost.Should().BeTrue();
            }
        }
        
        [Fact]
        public void Reveal_Should_NotLoseGameIfRevealedLocationIsNotMined()
        {
            // Arrange
            var minedLocation = Location.Parse("B2");
            var notMinedLocation = Location.Parse("A1");
            var enumerator = Enumerable.Range(0, 4)
                .SelectMany(_ => Enumerable.Range(0, 4), (column, row) => new Location((ColumnName)(column + 1), row + 1))
                .GetEnumerator();
            
            var fakeMinefield = A.Fake<IMinefield>();
            A.CallTo(() => fakeMinefield.NumberOfColumns).Returns(4);
            A.CallTo(() => fakeMinefield.NumberOfRows).Returns(4);
            A.CallTo(() => fakeMinefield.IsMined(A<Location>._)).Returns(false);
            A.CallTo(() => fakeMinefield.IsMined(minedLocation)).Returns(true);
            A.CallTo(() => fakeMinefield.GetEnumerator()).Returns(enumerator);
        
            var sut = new Game(fakeMinefield);
            
            // Act
            _ = sut.Reveal(notMinedLocation);
            
            // Assert
            using (new AssertionScope())
            {
                sut.IsWon.Should().BeFalse();
                sut.IsLost.Should().BeFalse();
            }
        }
        
        [Fact]
        public void Reveal_Should_WinGameIfAllNotMinedLocationsAreNowRevealed()
        {
            // Arrange
            var minedLocation = Location.Parse("A1");
            var notMinedLocations = new[]
            {
                Location.Parse("A2"),
                Location.Parse("B1"),
                Location.Parse("B2")
            };
            var enumerator = Enumerable.Range(0, 2)
                .SelectMany(_ => Enumerable.Range(0, 2), (column, row) => new Location((ColumnName)(column + 1), row + 1))
                .GetEnumerator();
            
            var fakeMinefield = A.Fake<IMinefield>();
            A.CallTo(() => fakeMinefield.NumberOfColumns).Returns(2);
            A.CallTo(() => fakeMinefield.NumberOfRows).Returns(2);
            A.CallTo(() => fakeMinefield.IsMined(A<Location>._)).Returns(false);
            A.CallTo(() => fakeMinefield.IsMined(minedLocation)).Returns(true);
            A.CallTo(() => fakeMinefield.GetEnumerator()).Returns(enumerator);
        
            var sut = new Game(fakeMinefield);
            
            // Act
            foreach (var cell in notMinedLocations)
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