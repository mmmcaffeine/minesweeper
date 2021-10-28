using System;
using FluentAssertions;
using Xunit;

namespace Dgt.Minesweeper.Engine
{
    public class MinefieldTests
    {
        [Theory]
        [InlineData("B3", 2)]
        [InlineData("A1", 0)]
        [InlineData("D2", 1)]
        [InlineData("A3", 0)]
        public void GetHint_Should_ReturnNumberOfAdjacentMines(string location, int expectedHint)
        {
            // Arrange
            var minefield = new Minefield(4, 3, new[] { Location.Parse("A3"), Location.Parse("C2") });

            // Act
            var hint = minefield.GetHint(Location.Parse(location));
            
            // Assert
            hint.Should().Be(expectedHint);
        }

        // TODO We could make our exception better by:
        // * Putting the actual location on the exception (maybe in Data if we don't want to write our own exception type)
        // * Indicating whether it is column, row, or both that are out of range
        [Fact]
        public void IsMined_Should_ThrowWhenLocationIsNotInMinefield()
        {
            // Arrange
            var minefield = new Minefield(4, 3, new[] { Location.Parse("A3"), Location.Parse("C2") });
            var locationToTest = Location.Parse("E1");
            
            // Act, Assert
            minefield.Invoking(x => x.IsMined(locationToTest)).Should().Throw<ArgumentException>()
                .WithMessage("The location does not exist in the minefield.*")
                .WithParameterName("location");
        }
    }
}