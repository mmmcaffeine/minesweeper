using System;
using FakeItEasy;
using FluentAssertions;
using Xunit;

namespace Dgt.Minesweeper.ConsoleUI
{
    public class EfficientGameRendererTests
    {
        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(-1)]
        [InlineData(0)]
        public void RenderTopBorder_Should_ThrowIfNumberOfRowsIsNotPositiveNonZero(int numberOfRows)
        {
            // Arrange
            var fakeCellRenderer = A.Fake<ICellRenderer>();
            var sut = new NaiveRowRenderer(fakeCellRenderer);

            // Act
            Action act = () => sut.RenderTopBorder(numberOfRows, 5);

            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>()
                .WithMessage("Value must be a positive, non-zero integer.*")
                .WithParameterName("numberOfRows")
                .Where(ex => ex.ActualValue!.Equals(numberOfRows));
        }

        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(-1)]
        [InlineData(0)]
        public void RenderTopBorder_Should_ThrowIfNumberOfColumnsIsNotPositiveNonZero(int numberOfColumns)
        {
            // Arrange
            var fakeCellRenderer = A.Fake<ICellRenderer>();
            var sut = new NaiveRowRenderer(fakeCellRenderer);

            // Act
            Action act = () => sut.RenderTopBorder(5, numberOfColumns);

            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>()
                .WithMessage("Value must be a positive, non-zero integer.*")
                .WithParameterName("numberOfColumns")
                .Where(ex => ex.ActualValue!.Equals(numberOfColumns));
        }

        [Fact]
        public void RenderTopBorder_Should_RenderBoxArtWithSpacesForRowNumbers()
        {
            // Arrange
            var sut = new EfficientGameRenderer();

            // Act
            var topBorder = sut.RenderTopBorder(10, 3);

            // Assert
            topBorder.Should().Be("   ╔═╦═╦═╗");
        }
    }
}