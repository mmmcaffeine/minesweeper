using System;
using FluentAssertions;
using Xunit;

namespace Dgt.Minesweeper.Engine
{
    // ReSharper disable once InconsistentNaming
    // Resharper isn't keen on the name of this test fixture, and I'm not either. It sounds like it should be an
    // interface. However, the tests in this type are for default implementations on the IMinefield interface,
    // hence needing something that is separate from MinefieldTests
    public class IMinefieldTests
    {
        private class TestableMinefield : IMinefield
        {
            public TestableMinefield(int numberOfColumns, int numberOfRows)
            {
                NumberOfColumns = numberOfColumns;
                NumberOfRows = numberOfRows;
            }
            
            public int NumberOfColumns { get; }
            public int NumberOfRows { get; }
            public int CountOfMines => 0;
            public bool IsMined(Location location) => throw new NotSupportedException();
            public int GetHint(Location location) =>  throw new NotSupportedException();
        }

        [Fact]
        public void Contains_Should_ThrowWhenLocationIsNull()
        {
            // Arrange
            IMinefield sut = new TestableMinefield(4, 4);
            
            // Act
            Action act = () => _ = sut.Contains(null!);
            
            // Assert
            act.Should().Throw<ArgumentNullException>().WithParameterName("location");
        }

        [Fact]
        public void Contains_Should_ReturnFalseWhenColumnIsOutOfBounds()
        {
            // Arrange
            IMinefield sut = new TestableMinefield(1, 1);
            var location = Location.Parse("B1");
            
            // Act, Assert
            sut.Contains(location).Should().BeFalse();
        }

        [Fact]
        public void Contains_Should_ReturnFalseWhenRowIsOutOfBounds()
        {
            // Arrange
            IMinefield sut = new TestableMinefield(1, 1);
            var location = Location.Parse("A2");
            
            // Act, Assert
            sut.Contains(location).Should().BeFalse();
        }

        [Fact]
        public void Contains_Should_ReturnTrueWhenLocationIsInMinefield()
        {
            // Arrange
            IMinefield sut = new TestableMinefield(1, 1);
            var location = Location.Parse("A1");
            
            // Act, Assert
            sut.Contains(location).Should().BeTrue();
        }
    }
}