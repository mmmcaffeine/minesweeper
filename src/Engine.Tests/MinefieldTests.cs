using System;
using System.Collections.Generic;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;

namespace Dgt.Minesweeper.Engine
{
    public class MinefieldTests
    {
        public static TheoryData<int> InvalidNumberOfTestData = new() { int.MinValue, -1, 0 };

        [Theory]
        [MemberData(nameof(InvalidNumberOfTestData))]
        public void Ctor_Should_ThrowWhenNumberOfRowsAndColumnsIsNotPositiveNonZero(int numberOfRowsAndColumns)
        {
            // Arrange, Act
            Action act = () => _ = new Minefield(numberOfRowsAndColumns, Array.Empty<Location>());
            
            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>()
                .WithMessage("Value must be a positive, non-zero integer.*")
                .WithParameterName("numberOfRowsAndColumns")
                .And.ActualValue.Should().Be(numberOfRowsAndColumns);
        }

        [Theory]
        [MemberData(nameof(InvalidNumberOfTestData))]
        public void Ctor_Should_ThrowWhenNumberOfColumnsIsNotPositiveNonZero(int numberOfColumns)
        {
            // Arrange, Act
            Action act = () => _ = new Minefield(numberOfColumns, 2, Array.Empty<Location>());
            
            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>()
                .WithMessage("Value must be a positive, non-zero integer.*")
                .WithParameterName("numberOfColumns")
                .And.ActualValue.Should().Be(numberOfColumns);
        }

        [Theory]
        [MemberData(nameof(InvalidNumberOfTestData))]
        public void Ctor_Should_ThrowWhenNumberOfRowsIsNotPositiveNonZero(int numberOfRows)
        {
            // Arrange, Act
            Action act = () => _ = new Minefield(2, numberOfRows, Array.Empty<Location>());
            
            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>()
                .WithMessage("Value must be a positive, non-zero integer.*")
                .WithParameterName("numberOfRows")
                .And.ActualValue.Should().Be(numberOfRows);
        }

        [Fact]
        public void Ctor_Should_ThrowWhenMinedLocationsIsNull()
        {
            // Arrange, Act
            var acts = new List<Action>
            {
                () => _ = new Minefield(2, 2, (IEnumerable<Location>)null!),
                () => _ = new Minefield(2, 2, (IEnumerable<string>)null!),
                () => _ = new Minefield(2, (IEnumerable<Location>)null!),
                () => _ = new Minefield(2, (IEnumerable<string>)null!)
            };

            // Assert
            using (new AssertionScope())
            {
                acts.ForEach(x => x.Should().Throw<ArgumentNullException>().WithParameterName("minedLocations"));
            }
        }
        
        [Theory]
        [InlineData("B3", 2)]
        [InlineData("A1", 0)]
        [InlineData("D2", 1)]
        [InlineData("A3", 0)]
        public void GetHint_Should_ReturnNumberOfAdjacentMines(string location, int expectedHint)
        {
            // Arrange
            var minefield = new Minefield(4, 3, new[] { "A3", "C2" });

            // Act
            var hint = minefield.GetHint(Location.Parse(location));
            
            // Assert
            hint.Should().Be(expectedHint);
        }

        [Theory]
        [InlineData("A3")]
        [InlineData("C2")]
        public void IsMined_Should_ReturnTrueWhenLocationIsMined(string location)
        {
            // Arrange
            var sut = new Minefield(4, 3, new[] { "A3", "C2" });
            
            // Act
            var isMined = sut.IsMined(Location.Parse(location));
            
            // Assert
            isMined.Should().BeTrue();
        }

        [Theory]
        [InlineData("A1")]
        [InlineData("B2")]
        [InlineData("C3")]
        [InlineData("D1")]
        public void IsMined_Should_ReturnFalseWhenLocationIsNotMined(string location)
        {
            // Arrange
            var sut = new Minefield(4, 3, new[] { "A3", "C2" });
            
            // Act
            var isMined = sut.IsMined(Location.Parse(location));
            
            // Assert
            isMined.Should().BeFalse();
        }

        // TODO We could make our exception better by:
        // * Putting the actual location on the exception (maybe in Data if we don't want to write our own exception type)
        // * Indicating whether it is column, row, or both that are out of range
        [Fact]
        public void IsMined_Should_ThrowWhenLocationIsNotInMinefield()
        {
            // Arrange
            var minefield = new Minefield(4, 3, new[] { "A3", "C2" });
            var locationToTest = Location.Parse("E1");
            
            // Act, Assert
            minefield.Invoking(x => x.IsMined(locationToTest)).Should().Throw<ArgumentException>()
                .WithMessage("The location does not exist in the minefield.*")
                .WithParameterName("location");
        }
    }
}