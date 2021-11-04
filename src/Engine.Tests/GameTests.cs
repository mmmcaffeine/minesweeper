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
        public void Ctor_Should_InitialiseCellsAtLocationsThatAreMinedToMined()
        {
            // Arrange
            var minedLocation = Location.Parse("B2");
            var enumerator = Enumerable.Range(1, 4)
                .SelectMany(_ => Enumerable.Range(1, 4), (ci, ri) => new Location(ci, ri))
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
            sut.GetCell(minedLocation).IsMined.Should().BeTrue();
        }

        [Fact]
        public void Ctor_Should_InitialiseCellsAtLocationsThatAreNotMinedToNotMined()
        {
            // Arrange
            var enumerator = Enumerable.Range(1, 2)
                .SelectMany(_ => Enumerable.Range(1, 2), (ci, ri) => new Location(ci, ri))
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
                sut.GetCell(Location.Parse("A1")).IsMined.Should().BeFalse();
                sut.GetCell(Location.Parse("A2")).IsMined.Should().BeFalse();
                sut.GetCell(Location.Parse("B1")).IsMined.Should().BeFalse();
                sut.GetCell(Location.Parse("B2")).IsMined.Should().BeFalse();
            }
        }
        
        [Fact]
        public void Ctor_Should_InitialiseGameToStateThatIsNotWonAndNotLost()
        {
            // Arrange
            var minedLocation = Location.Parse("B2");
            var enumerator = Enumerable.Range(1, 4)
                .SelectMany(_ => Enumerable.Range(1, 4), (ci, ri) => new Location(ci, ri))
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
                sut.NumberOfCellsToReveal.Should().Be(15, "there are 16 cells and one is mined");
                sut.IsWon.Should().BeFalse();
                sut.IsLost.Should().BeFalse();
            }
        }
        
        [Fact]
        public void Reveal_Should_PutMinedCellIntoRevealedAndExplodedState()
        {
            // Arrange
            var minedLocation = Location.Parse("B2");
            var enumerator = Enumerable.Range(1, 4)
                .SelectMany(_ => Enumerable.Range(1, 4), (ci, ri) => new Location(ci, ri))
                .GetEnumerator();
            
            var fakeMinefield = A.Fake<IMinefield>();
            A.CallTo(() => fakeMinefield.NumberOfColumns).Returns(4);
            A.CallTo(() => fakeMinefield.NumberOfRows).Returns(4);
            A.CallTo(() => fakeMinefield.IsMined(A<Location>._)).Returns(false);
            A.CallTo(() => fakeMinefield.IsMined(minedLocation)).Returns(true);
            A.CallTo(() => fakeMinefield.GetEnumerator()).Returns(enumerator);
        
            var sut = new Game(fakeMinefield);
            
            // Act
            var returnedCell = sut.Reveal(minedLocation);
            var retrievedCell = sut.GetCell(minedLocation);
            
            // Assert
            using (new AssertionScope())
            {
                returnedCell.IsRevealed.Should().BeTrue();
                returnedCell.IsExploded.Should().BeTrue();
                retrievedCell.IsRevealed.Should().BeTrue();
                retrievedCell.IsExploded.Should().BeTrue();
            }
        }
        
        [Fact]
        public void Reveal_Should_PutNotMinedCellIntoRevealedAndNotExplodedState()
        {
            // Arrange
            var minedLocation = Location.Parse("B2");
            var notMinedLocation = Location.Parse("A1");
            var enumerator = Enumerable.Range(1, 4)
                .SelectMany(_ => Enumerable.Range(1, 4), (ci, ri) => new Location(ci, ri))
                .GetEnumerator();
            
            var fakeMinefield = A.Fake<IMinefield>();
            A.CallTo(() => fakeMinefield.NumberOfColumns).Returns(4);
            A.CallTo(() => fakeMinefield.NumberOfRows).Returns(4);
            A.CallTo(() => fakeMinefield.IsMined(A<Location>._)).Returns(false);
            A.CallTo(() => fakeMinefield.IsMined(minedLocation)).Returns(true);
            A.CallTo(() => fakeMinefield.GetEnumerator()).Returns(enumerator);
        
            var sut = new Game(fakeMinefield);
            
            // Act
            var returnedCell = sut.Reveal(notMinedLocation);
            var retrievedCell = sut.GetCell(notMinedLocation);
            
            // Assert
            using (new AssertionScope())
            {
                returnedCell.IsRevealed.Should().BeTrue();
                returnedCell.IsExploded.Should().BeFalse();
                retrievedCell.IsRevealed.Should().BeTrue();
                retrievedCell.IsExploded.Should().BeFalse();
            }
        }
        
        [Fact]
        public void Reveal_Should_LoseGameIfRevealedCellIsMined()
        {
            // Arrange
            var minedLocation = Location.Parse("B2");
            var enumerator = Enumerable.Range(1, 4)
                .SelectMany(_ => Enumerable.Range(1, 4), (ci, ri) => new Location(ci, ri))
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
        public void Reveal_Should_NotLoseGameIfRevealedCellIsNotMined()
        {
            // Arrange
            var minedLocation = Location.Parse("B2");
            var notMinedLocation = Location.Parse("A1");
            var enumerator = Enumerable.Range(1, 4)
                .SelectMany(_ => Enumerable.Range(1, 4), (ci, ri) => new Location(ci, ri))
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
                sut.NumberOfCellsToReveal.Should().Be(14, "there are 16 cells, one is mined, and we have revealed one");
                sut.IsWon.Should().BeFalse("there are 14 cells to reveal");
                sut.IsLost.Should().BeFalse("no mined cells have been revealed");
            }
        }
        
        [Fact]
        public void Reveal_Should_WinGameIfAllCellsWithoutMinesAreNowRevealed()
        {
            // Arrange
            var minedLocation = Location.Parse("A1");
            var notMinedLocations = new[]
            {
                Location.Parse("A2"),
                Location.Parse("B1"),
                Location.Parse("B2")
            };
            var enumerator = Enumerable.Range(1, 2)
                .SelectMany(_ => Enumerable.Range(1, 2), (ci, ri) => new Location(ci, ri))
                .GetEnumerator();
            
            var fakeMinefield = A.Fake<IMinefield>();
            A.CallTo(() => fakeMinefield.NumberOfColumns).Returns(2);
            A.CallTo(() => fakeMinefield.NumberOfRows).Returns(2);
            A.CallTo(() => fakeMinefield.IsMined(A<Location>._)).Returns(false);
            A.CallTo(() => fakeMinefield.IsMined(minedLocation)).Returns(true);
            A.CallTo(() => fakeMinefield.GetEnumerator()).Returns(enumerator);
            
            var sut = new Game(fakeMinefield);
            
            // Act
            foreach (var location in notMinedLocations)
            {
                _ = sut.Reveal(location);
            }
            
            // Assert
            using (new AssertionScope())
            {
                sut.NumberOfCellsToReveal.Should().Be(0, "all cells without mines have been revealed");
                sut.IsWon.Should().BeTrue("all cells without mines have been revealed");
                sut.IsLost.Should().BeFalse("no mined cells have been revealed");
            }
        }
    }
}