using System;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;

namespace Dgt.Minesweeper.Engine
{
    public class GameTests
    {
        [Fact]
        public void Ctor_Should_ThrowWhenMinefieldIsNull()
        {
            // Arrange, Act
            Action act = () => _ = new Game(null!);
            
            // Assert
            act.Should().Throw<ArgumentNullException>().WithParameterName("minefield");
        }

        [Fact]
        public void Ctor_Should_InitialiseNumberOfRowsAndColumnsToSameDimensionOnMinefield()
        {
            // Arrange
            var minefield = new Minefield(10, 5, Array.Empty<Location>());
            
            // Act
            var sut = new Game(minefield);

            // Assert
            sut.NumberOfColumns.Should().Be(10);
            sut.NumberOfRows.Should().Be(5);
        }
        
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
                sut.IsOver.Should().BeFalse();
            }
        }

        [Fact]
        public void GetCell_Should_ThrowWhenLocationIsNull()
        {
            // Arrange
            var minefield = new Minefield(2, Array.Empty<Location>());
            var sut = new Game(minefield);
            
            // Act
            // ReSharper disable once RedundantCast
            // To guarantee we get the correct overload
            Action act = () => _ = sut.GetCell((Location)null!);

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
            // ReSharper disable once RedundantCast
            // To guarantee we get the correct overload
            Action act = () => sut.Reveal((Location)null!);

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
            var minefield = new Minefield(4, new[] { Location.Parse("B2") });
            var sut = new Game(minefield);
            
            // Act
            var returnedCell = sut.Reveal("A1");
            var retrievedCell = sut.GetCell("A1");
            
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
                sut.IsLost.Should().BeTrue("a mined cell has been revealed");
                sut.IsOver.Should().BeTrue("the game has been lost");
            }
        }
        
        [Fact]
        public void Reveal_Should_NotLoseGameIfRevealedCellIsNotMined()
        {
            // Arrange
            var minefield = new Minefield(4, new[] { Location.Parse("B2") });
            var sut = new Game(minefield);
            
            // Act
            _ = sut.Reveal("A1");
            
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
            var notMinedLocations = new[] { "A2", "B1", "B2" };
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
                sut.IsOver.Should().BeTrue("the game has been won");
            }
        }
        
        [Fact]
        public void Reveal_Should_ThrowIfCellHasAlreadyBeenRevealed()
        {
            // Arrange
            var minefield = new Minefield(2, Array.Empty<Location>());
            var sut = new Game(minefield);

            _ = sut.Reveal("A1");
            
            // Act
            Action act = () => _ = sut.Reveal("A1");
            
            // Assert
            act.Should().Throw<InvalidMoveException>()
                .WithMessage("*The Cell at Location \"A1\" has been revealed.*")
                .WithMessage("*A revealed Cell cannot be revealed again.")
                .Where(ex => ex.Cell == sut.GetCell("A1"));
        }
        
        [Fact]
        public void Reveal_Should_ThrowIfCellHasBeenFlagged()
        {
            // Arrange
            var minefield = new Minefield(2, Array.Empty<Location>());
            var sut = new Game(minefield);

            _ = sut.ToggleFlag("A1");
            
            // Act
            Action act = () => _ = sut.Reveal("A1");
            
            // Assert
            act.Should().Throw<InvalidMoveException>()
                .WithMessage("*The Cell at Location \"A1\" has been revealed.*")
                .WithMessage("*A flagged Cell cannot be revealed.")
                .Where(ex => ex.Cell == sut.GetCell("A1"));
        }

        [Fact]
        public void Reveal_Should_ThrowIfGameHasAlreadyBeenLost()
        {
            // Arrange
            var minedLocation = Location.Parse("A1");
            var minefield = new Minefield(2, new[] { minedLocation });
            var sut = new Game(minefield);

            _ = sut.Reveal(minedLocation);
            
            // Act
            Action act = () => _ = sut.Reveal("A2");
            
            // Assert
            act.Should().Throw<GameOverException>().Where(ex => ex.IsWon == false);
        }
        
        [Fact]
        public void Reveal_Should_ThrowIfGameHasAlreadyBeenWon()
        {
            // Arrange
            var minedLocation = Location.Parse("A1");
            var minefield = new Minefield(2, new[] { minedLocation });
            var sut = new Game(minefield);

            _ = sut.Reveal("A2");
            _ = sut.Reveal("B1");
            _ = sut.Reveal("B2");
            
            // Act
            Action act = () => _ = sut.Reveal(minedLocation);
            
            // Assert
            act.Should().Throw<GameOverException>().Where(ex => ex.IsWon == true);
        }

        [Fact]
        public void ToggleFlag_Should_ThrowWhenLocationIsNull()
        {
            // Arrange
            var minefield = new Minefield(2, Array.Empty<Location>());
            var sut = new Game(minefield);
            
            // Act
            // ReSharper disable once RedundantCast
            // To guarantee we get the correct overload
            Action act = () => sut.ToggleFlag((Location)null!);

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
            var minefield = new Minefield(2, Array.Empty<Location>());
            var sut = new Game(minefield);
            
            // Act
            var returnedCell = sut.ToggleFlag("B2");
            var retrievedCell = sut.GetCell("B2");
            
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
            var minefield = new Minefield(2, Array.Empty<Location>());
            var sut = new Game(minefield);

            _ = sut.ToggleFlag("B2");
            
            // Act
            var returnedCell = sut.ToggleFlag("B2");
            var retrievedCell = sut.GetCell("B2");
            
            // Assert
            using (new AssertionScope())
            {
                returnedCell.IsFlagged.Should().BeFalse();
                retrievedCell.IsFlagged.Should().BeFalse();
            }
        }

        [Fact]
        public void ToggleFlag_Should_ThrowIfCellHasBeenRevealed()
        {
            // Arrange
            var minefield = new Minefield(2, Array.Empty<Location>());
            var sut = new Game(minefield);

            _ = sut.Reveal("A1");
            
            // Act
            Action act = () => _ = sut.ToggleFlag("A1");
            
            // Assert
            act.Should().Throw<InvalidMoveException>()
                .WithMessage("*The Cell at Location \"A1\" has been revealed.*")
                .WithMessage("*A revealed Cell cannot be flagged.")
                .Where(ex => ex.Cell == sut.GetCell("A1"));
        }
        
        [Fact]
        public void ToggleFlag_Should_ThrowIfGameHasAlreadyBeenLost()
        {
            // Arrange
            var minedLocation = Location.Parse("A1");
            var minefield = new Minefield(2, new[] { minedLocation });
            var sut = new Game(minefield);

            _ = sut.Reveal(minedLocation);
            
            // Act
            Action act = () => _ = sut.ToggleFlag("A2");
            
            // Assert
            act.Should().Throw<GameOverException>().Where(ex => ex.IsWon == false);
        }
        
        [Fact]
        public void ToggleFlag_Should_ThrowIfGameHasAlreadyBeenWon()
        {
            // Arrange
            var minedLocation = Location.Parse("A1");
            var minefield = new Minefield(2, new[] { minedLocation });
            var sut = new Game(minefield);

            _ = sut.Reveal("A2");
            _ = sut.Reveal("B1");
            _ = sut.Reveal("B2");
            
            // Act
            Action act = () => _ = sut.ToggleFlag(minedLocation);
            
            // Assert
            act.Should().Throw<GameOverException>().Where(ex => ex.IsWon == true);
        }
    }
}