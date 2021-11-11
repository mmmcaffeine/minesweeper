using System.Collections.Generic;

namespace Dgt.Minesweeper.ConsoleUI
{
    public interface IGameRenderer
    {
        // Typically you would design this interface to accept the IGame to render. However, we're taking a bit
        // of a liberty with knowledge of how we're going to want to implement this. We're going to want to
        // cache a lot of information about the Game (e.g. column headers) which wouldn't be practical if we
        // didn't know what Game we were going to be rendering
        IEnumerable<string> Render();
    }
}