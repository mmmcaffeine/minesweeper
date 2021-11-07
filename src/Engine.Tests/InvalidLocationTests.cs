using FluentAssertions;
using Xunit;

namespace Dgt.Minesweeper.Engine
{
    public class InvalidLocationTests
    {
        [Fact]
        public void Ctor_Should_InitialiseLocationWhenLocationIsPassed()
        {
            // Arrange
            var location = Location.Parse("C3");
            
            // Act
            var sut = new InvalidLocationException(location, 5);

            // Assert
            sut.Location.Should().Be(location);
        }

        [Fact]
        public void Ctor_Should_InitialiseMaximumColumnNameWhenNumberOfColumnsIsPassed()
        {
            // Arrange, Act
            var sut = new InvalidLocationException(Location.Parse("E5"), 3, 7);
            
            // Assert
            sut.MaximumColumnName.Should().Be(new ColumnName("C"));
        }

        [Fact]
        public void Ctor_Should_InitialiseMaximumRowIndexWhenNumberOfRowsIsPassed()
        {
            // Arrange, Act
            var sut = new InvalidLocationException(Location.Parse("E5"), 7, 3);
            
            // Assert
            sut.MaximumRowIndex.Should().Be(3);
        }

        [Fact]
        public void Ctor_Should_InitialiseMaximumColumnNameAndMaximumRowIndexWhenNumberOfColumnsAndRowsIsPassed()
        {
            // Arrange, Act
            var sut = new InvalidLocationException(Location.Parse("H8"), 5);
            
            // Assert
            sut.MaximumColumnName.Should().Be(new ColumnName("E"));
            sut.MaximumRowIndex.Should().Be(5);
        }

        [Fact]
        public void Ctor_Should_IncludeColumnInformationInMessageWhenColumnIsOutOfBounds()
        {
            // Arrange, Act
            var sut = new InvalidLocationException(Location.Parse("H8"), 5, 10);
            
            // Assert
            sut.Message.Should().Contain("The column is out of bounds.");
            sut.Message.Should().Contain("The column must be between \"A\" and \"E\", but found \"H\".");
        }
        
        [Fact]
        public void Ctor_Should_IncludeRowInformationInMessageWhenRowIsOutOfBounds()
        {
            // Arrange, Act
            var sut = new InvalidLocationException(Location.Parse("H8"), 10, 5);
            
            // Assert
            sut.Message.Should().Contain("The row is out of bounds.");
            sut.Message.Should().Contain("The row must be between 1 and 5, but found 8.");
        }

        [Fact]
        public void Ctor_Should_IncludeColumnAndRowInformationInMessageWhenColumnAndRowAreOutOfBounds()
        {
            // Arrange, Act
            var sut = new InvalidLocationException(Location.Parse("H8"), 5);
            
            // Assert
            sut.Message.Should().Contain("The column is out of bounds.");
            sut.Message.Should().Contain("The row is out of bounds.");
        }

        [Fact]
        public void Ctor_Should_IncludeDefaultMessage()
        {
            // Arrange
            var sut = new InvalidLocationException(Location.Parse("H8"), 5);
            
            // Assert
            sut.Message.Should().StartWith("The Location does not exist in the Minefield.");
        }
    }
}