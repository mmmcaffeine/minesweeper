using System.Linq;
using FluentAssertions;
using Xunit;

namespace Dgt.Minesweeper.Engine
{
    public class MinefieldRendererTests
    {
        // TODO Currently our "unit" test is reliant on the implementation of both Minefield and MinefieldRenderer
        //      Minefield itself is reliant on Location and ColumnName
        //      Ideally, we would only be reliant on one of our types. Currently if our test fails we don't immediately
        //      know which type is at fault. We can resolve this by extracting an interface from Minefield, have
        //      MinefieldRenderer consume that rather than the concrete type and then use a fake
        [Fact]
        public void Render_Should_RenderMineOrHintForEveryLocation()
        {
            // Arrange
            var minefield = new Minefield(4, 3, new[] { Location.Parse("A3"), Location.Parse("C2") });
            var sut = new MinefieldRenderer();

            // Act
            var output = sut.Render(minefield).ToList();

            // Assert
            output.Count.Should().Be(3);

            output[0].Should().Be("*211");
            output[1].Should().Be("12*1");
            output[2].Should().Be("0111");
        }
    }
}