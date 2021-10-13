using FluentAssertions;
using Xunit;

namespace Dgt.Minesweeper.Engine
{
    public class DummyTests
    {
        [Fact]
        public void Add_Should_ReturnTheSumOfTheInputs()
        {
            Dummy.Add(3, 4).Should().Be(7);
        }
    }
}