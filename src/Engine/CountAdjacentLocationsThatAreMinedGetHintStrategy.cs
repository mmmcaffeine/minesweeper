using System.Linq;

namespace Dgt.Minesweeper.Engine
{
    public class CountAdjacentLocationsThatAreMinedGetHintStrategy : IGetHintStrategy
    {
        public int GetHint(Location location, IMinefield minefield) =>
            minefield.GetAdjacentLocations(location).Count(minefield.IsMined);
    }
}