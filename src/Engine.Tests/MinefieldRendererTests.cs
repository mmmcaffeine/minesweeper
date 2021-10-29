using System.Linq;
using FluentAssertions;
using Xunit;

namespace Dgt.Minesweeper.Engine
{
    public class MinefieldRendererTests
    {
        // TODO Currently our "unit" test is reliant on the implementation of both Minefield and MinefieldRenderer
        //      Minefield itself is reliant on Location and ColumnName. Currently if our test fails we don't
        //      immediately know whether it is our test subject that is at fault, or if it is Minefield or one of
        //      its dependencies. Our test subject now consumes IMinefield so we could use a fake. We'd need to fake:
        //
        //      * NumberOfColumns (easy)
        //      * NumberOfRows (easy)
        //      * IsMined (not so tough as we only have two locations)
        //      * GetHint (probably not all that hard if it defaults to returning 0)
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