using System.Collections.Generic;
using System.Linq;

namespace Dgt.Minesweeper.Engine
{
    public class Minefield
    {
        private readonly HashSet<Square> _minedSquares;

        public Minefield(int rows, int columns, IEnumerable<Square> minedSquares)
        {
            Rows = rows;
            Columns = columns;
            _minedSquares = new HashSet<Square>(minedSquares);
        }

        public int Rows { get; }
        public int Columns { get; }
        
        public bool IsMined(int row, int column)
        {
            return _minedSquares.Contains(new Square(row, column));
        }
        
        public int GetHint(int row, int column)
        {
            List<Square> adjacentSquares = new()
            {
                new Square(row + 1, column - 1),
                new Square(row + 1, column),
                new Square(row + 1, column + 1),
                new Square(row, column - 1),
                new Square(row, column + 1),
                new Square(row - 1, column + 1),
                new Square(row - 1, column),
                new Square(row - 1, column - 1)
            };

            return adjacentSquares.Count(square =>
                SquareExistsInMinefield(square.Row, square.Column) && IsMined(square.Row, square.Column));
        }
        
        private bool SquareExistsInMinefield(int row, int column) => row <= Rows && column <= Columns;
    }
}