using FluentAssertions;
using Xunit;

namespace Dgt.Minesweeper.Engine
{
    public class GameOverExceptionTests
    {
        [Fact]
        public void Ctor_Should_IncludeGameOverInformationInMessage()
        {
            // Arrange, Act
            var sut = new GameOverException(true);
            
            // Assert
            sut.Message.Should().StartWith("The Game is over and no further moves may be made.");
        }
        
        [Fact]
        public void Ctor_Should_IncludeGameIsWonInformationInMessage()
        {
            // Arrange, Act
            var sut = new GameOverException(true);
            
            // Assert
            sut.Message.Should().EndWith("The Game has already been won.");
        }
        
        [Fact]
        public void Ctor_Should_IncludeGameIsLostInformationInMessage()
        {
            // Arrange, Act
            var sut = new GameOverException(false);
            
            // Assert
            sut.Message.Should().EndWith("The Game has already been lost.");
        }
    }
}