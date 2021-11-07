using FluentAssertions;
using Xunit;

namespace Dgt.Minesweeper.Engine
{
    public class InvalidMoveExceptionTests
    {
        [Fact]
        public void Ctor_Should_InitialiseCellAndLocationWhenCellIsPassed()
        {
            // Arrange, Act
            var location = Location.Parse("A5");
            var cell = new Cell(location, false, 0);
            var sut = new InvalidMoveException(cell, "foo", "bar");
            
            // Assert
            sut.Cell.Should().Be(cell);
            sut.Location.Should().Be(location);
        }
        
        [Fact]
        public void Ctor_Should_IncludeDefaultMessage()
        {
            // Arrange, Act
            var location = Location.Parse("A5");
            var cell = new Cell(location, false, 0);
            var sut = new InvalidMoveException(cell, "foo", "bar");
            
            // Assert
            sut.Message.Should().StartWith("The move is not valid due to the current state of the Cell at the given Location.");
        }

        [Fact]
        public void Ctor_Should_IncludeLocationAndStateInMessageWhenLocationAndStateArePassed()
        {
            // Arrange, Act
            var location = Location.Parse("A5");
            var cell = new Cell(location, false, 0);
            var sut = new InvalidMoveException(cell, "revealed", "foo");
            
            // Assert
            sut.Message.Should().Contain("The Cell at Location \"A5\" has been revealed.");
        }

        [Fact]
        public void Ctor_Should_IncludeExplanationInMessage()
        {
            // Arrange, Act
            var location = Location.Parse("A5");
            var cell = new Cell(location, false, 0);
            var sut = new InvalidMoveException(cell, "foo", "You can't do the thing because the reason");
            
            // Assert
            sut.Message.Should().EndWith("You can't do the thing because the reason.");
        }
    }
}