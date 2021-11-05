using System.Linq;

namespace Dgt.Minesweeper.Engine
{
    public class CountMinedLocationsThatAreAdjacentGetHintStrategy : IGetHintStrategy
    {
        public int GetHint(Location location, IMinefield minefield) =>
            minefield.GetMinedLocations().Count(loc => loc.IsAdjacentTo(location));
    }
}