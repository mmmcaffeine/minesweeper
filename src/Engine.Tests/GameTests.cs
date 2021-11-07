using System;
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
            var minefield = new Minefield(4, new[] { minedLocation });

            // Act
            var sut = new Game(minefield);
            
            // Assert
            sut.GetCell(minedLocation).IsMined.Should().BeTrue();
        }

        [Fact]
        public void Ctor_Should_InitialiseCellsAtLocationsThatAreNotMinedToNotMined()
        {
            // Arrange
            var minefield = new Minefield(2, Array.Empty<Location>());
        
            // Act
            var sut = new Game(minefield);
            
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
        public void Ctor_Should_InitialiseAllCellsToNotRevealed()
        {
            // Arrange
            var minefield = new Minefield(2, Array.Empty<Location>());
            
            // Act
            var sut = new Game(minefield);
            
            // Assert
            using (new AssertionScope())
            {
                sut.GetCell(Location.Parse("A1")).IsRevealed.Should().BeFalse();
                sut.GetCell(Location.Parse("A2")).IsRevealed.Should().BeFalse();
                sut.GetCell(Location.Parse("B1")).IsRevealed.Should().BeFalse();
                sut.GetCell(Location.Parse("B2")).IsRevealed.Should().BeFalse();
            }
        }
        
        [Fact]
        public void Ctor_Should_InitialiseAllCellsToNotFlagged()
        {
            // Arrange
            var minefield = new Minefield(2, Array.Empty<Location>());
            
            // Act
            var sut = new Game(minefield);
            
            // Assert
            using (new AssertionScope())
            {
                sut.GetCell(Location.Parse("A1")).IsFlagged.Should().BeFalse();
                sut.GetCell(Location.Parse("A2")).IsFlagged.Should().BeFalse();
                sut.GetCell(Location.Parse("B1")).IsFlagged.Should().BeFalse();
                sut.GetCell(Location.Parse("B2")).IsFlagged.Should().BeFalse();
            }
        }

        [Fact]
        public void Ctor_Should_InitialiseAllCellsWithHint()
        {
            // Arrange
            var minefield = new Minefield(3, new[] { Location.Parse("A1"), Location.Parse("A2") });
            
            // Act
            var sut = new Game(minefield);
            
            // Assert
            
            // Our game should look like:
            // 1 1 0
            // * 2 0
            // * 2 0
            using (new AssertionScope())
            {
                sut.GetCell(Location.Parse("A1")).Hint.Should().Be(1);
                sut.GetCell(Location.Parse("A2")).Hint.Should().Be(1);
                sut.GetCell(Location.Parse("A3")).Hint.Should().Be(1);
                sut.GetCell(Location.Parse("B1")).Hint.Should().Be(2);
                sut.GetCell(Location.Parse("B2")).Hint.Should().Be(2);
                sut.GetCell(Location.Parse("B3")).Hint.Should().Be(1);
                sut.GetCell(Location.Parse("C1")).Hint.Should().Be(0);
                sut.GetCell(Location.Parse("C2")).Hint.Should().Be(0);
                sut.GetCell(Location.Parse("C3")).Hint.Should().Be(0);
            }
        }
        
        [Fact]
        public void Ctor_Should_InitialiseGameToStateThatIsNotWonAndNotLost()
        {
            // Arrange
            var minefield = new Minefield(4, new[] { Location.Parse("B2") });
            
            // Act
            var sut = new Game(minefield);
            
            // Assert
            using (new AssertionScope())
            {
                sut.NumberOfCellsToReveal.Should().Be(15, "there are 16 cells and one is mined");
                sut.IsWon.Should().BeFalse();
                sut.IsLost.Should().BeFalse();
            }
        }

        [Fact]
        public void GetCell_Should_ThrowWhenLocationIsNull()
        {
            // Arrange
            var minefield = new Minefield(2, Array.Empty<Location>());
            var sut = new Game(minefield);
            
            // Act
            Action act = () => _ = sut.GetCell(null!);

            // Assert
            act.Should().Throw<ArgumentNullException>().WithParameterName("location");
        }

        [Fact]
        public void GetCell_Should_ThrowWhenLocationIsNotInMinefield()
        {
            // Arrange
            var locationToTest = Location.Parse("C3");
            var minefield = new Minefield(2, Array.Empty<Location>());
            var sut = new Game(minefield);
            
            // Act
            Action act = () => _ = sut.GetCell(locationToTest);
            
            // Assert
            act.Should().Throw<InvalidLocationException>()
                .Where(ex => ex.Location == locationToTest)
                .Where(ex => ex.MaximumColumnName == new ColumnName("B"))
                .Where(ex => ex.MaximumRowIndex == 2);
        }
        
        [Fact]
        public void Reveal_Should_ThrowWhenLocationIsNull()
        {
            // Arrange
            var minefield = new Minefield(2, Array.Empty<Location>());
            var sut = new Game(minefield);
            
            // Act
            Action act = () => sut.Reveal(null!);

            // Assert
            act.Should().Throw<ArgumentNullException>().WithParameterName("location");
        }

        [Fact]
        public void Reveal_Should_ThrowWhenLocationIsNotInMinefield()
        {
            // Arrange
            var locationToTest = Location.Parse("C3");
            var minefield = new Minefield(2, Array.Empty<Location>());
            var sut = new Game(minefield);
            
            // Act
            Action act = () => sut.Reveal(locationToTest);
            
            // Assert
            act.Should().Throw<InvalidLocationException>()
                .Where(ex => ex.Location == locationToTest)
                .Where(ex => ex.MaximumColumnName == new ColumnName("B"))
                .Where(ex => ex.MaximumRowIndex == 2);
        }
        
        [Fact]
        public void Reveal_Should_PutMinedCellIntoRevealedAndExplodedState()
        {
            // Arrange
            var minedLocation = Location.Parse("B2");
            var minefield = new Minefield(4, new[] { minedLocation });
            var sut = new Game(minefield);
            
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
            var notMinedLocation = Location.Parse("A1");
            var minefield = new Minefield(4, new[] { Location.Parse("B2") });
            var sut = new Game(minefield);
            
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
            var minefield = new Minefield(4, new[] { minedLocation });
            var sut = new Game(minefield);
            
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
            var notMinedLocation = Location.Parse("A1");
            var minefield = new Minefield(4, new[] { Location.Parse("B2") });
            var sut = new Game(minefield);
            
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
            var notMinedLocations = new[]
            {
                Location.Parse("A2"),
                Location.Parse("B1"),
                Location.Parse("B2")
            };
            var minefield = new Minefield(2, new[] { Location.Parse("A1") });
            var sut = new Game(minefield);
            
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

        [Fact]
        public void ToggleFlag_Should_ThrowWhenLocationIsNull()
        {
            // Arrange
            var minefield = new Minefield(2, Array.Empty<Location>());
            var sut = new Game(minefield);
            
            // Act
            Action act = () => sut.ToggleFlag(null!);

            // Assert
            act.Should().Throw<ArgumentNullException>().WithParameterName("location");
        }
        
        [Fact]
        public void ToggleFlag_Should_ThrowWhenLocationIsNotInMinefield()
        {
            // Arrange
            var locationToTest = Location.Parse("C3");
            var minefield = new Minefield(2, Array.Empty<Location>());
            var sut = new Game(minefield);
            
            // Act
            Action act = () => sut.ToggleFlag(locationToTest);
            
            // Assert
            act.Should().Throw<InvalidLocationException>()
                .Where(ex => ex.Location == locationToTest)
                .Where(ex => ex.MaximumColumnName == new ColumnName("B"))
                .Where(ex => ex.MaximumRowIndex == 2);
        }

        [Fact]
        public void ToggleFlag_Should_PutMinedAndNotFlaggedCellIntoFlaggedState()
        {
            // Arrange
            var minedLocation = Location.Parse("A1");
            var minefield = new Minefield(2, new[] { minedLocation });
            var sut = new Game(minefield);
            
            // Act
            var returnedCell = sut.ToggleFlag(minedLocation);
            var retrievedCell = sut.GetCell(minedLocation);
            
            // Assert
            using (new AssertionScope())
            {
                returnedCell.IsFlagged.Should().BeTrue();
                retrievedCell.IsFlagged.Should().BeTrue();
            }
        }
        
        [Fact]
        public void ToggleFlag_Should_PutNotMinedAndNotFlaggedCellIntoFlaggedState()
        {
            // Arrange
            var notMinedLocation = Location.Parse("B2");
            var minefield = new Minefield(2, Array.Empty<Location>());
            var sut = new Game(minefield);
            
            // Act
            var returnedCell = sut.ToggleFlag(notMinedLocation);
            var retrievedCell = sut.GetCell(notMinedLocation);
            
            // Assert
            using (new AssertionScope())
            {
                returnedCell.IsFlagged.Should().BeTrue();
                retrievedCell.IsFlagged.Should().BeTrue();
            }
        }
        
        [Fact]
        public void ToggleFlag_Should_PutMinedAndFlaggedCellIntoNotFlaggedState()
        {
            // Arrange
            var minedLocation = Location.Parse("A1");
            var minefield = new Minefield(2, new[] { minedLocation });
            var sut = new Game(minefield);
            
            _ = sut.ToggleFlag(minedLocation);
            
            // Act
            var returnedCell = sut.ToggleFlag(minedLocation);
            var retrievedCell = sut.GetCell(minedLocation);
            
            // Assert
            using (new AssertionScope())
            {
                returnedCell.IsFlagged.Should().BeFalse();
                retrievedCell.IsFlagged.Should().BeFalse();
            }
        }
        
        [Fact]
        public void ToggleFlag_Should_PutNotMinedAndFlaggedCellIntoNotFlaggedState()
        {
            // Arrange
            var notMinedLocation = Location.Parse("B2");
            var minefield = new Minefield(2, Array.Empty<Location>());
            var sut = new Game(minefield);

            _ = sut.ToggleFlag(notMinedLocation);
            
            // Act
            var returnedCell = sut.ToggleFlag(notMinedLocation);
            var retrievedCell = sut.GetCell(notMinedLocation);
            
            // Assert
            using (new AssertionScope())
            {
                returnedCell.IsFlagged.Should().BeFalse();
                retrievedCell.IsFlagged.Should().BeFalse();
            }
        }
    }
}