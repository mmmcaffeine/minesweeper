using System.Collections.Generic;
using System.Linq;

namespace Dgt.Minesweeper.Engine
{
    public class Minefield
    {
        private readonly List<Square> _minedSquares;

        public Minefield(int rows, int columns, IEnumerable<Square> minedSquares)
        {
            Rows = rows;
            Columns = columns;
            _minedSquares = minedSquares.ToList();
        }

        public int Rows { get; }
        public int Columns { get; }
        
        public bool IsMined(int row, int column)
        {
            return _minedSquares.Any(square => square.Row == row && square.Column == column);
        }
    }
}