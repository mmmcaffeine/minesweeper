﻿using System;
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

        [Theory]
        [InlineData(1, "A1", true)]
        [InlineData(1, "A2", false)]        // Row is out of bounds
        [InlineData(1, "B1", false)]        // Column is out of bounds
        [InlineData(10, "I10", true)]
        [InlineData(10, "M15", false)]      // Row and column are out of bounds
        public void Contains_Should_ReturnTrueWhenLocationIsInMinefieldOtherwiseFalse
            (int numberOfRowsAndColumns, string location, bool expected)
        {
            // Arrange
            IMinefield sut = new TestableMinefield(numberOfRowsAndColumns, numberOfRowsAndColumns);
            var locationToTest = Location.Parse(location);
            
            // Act, Assert
            sut.Contains(locationToTest).Should().Be(expected);
        }
    }
}