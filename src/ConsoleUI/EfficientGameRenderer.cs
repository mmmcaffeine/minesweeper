using System;
using System.Collections.Generic;
using Dgt.Minesweeper.Engine;

namespace Dgt.Minesweeper.ConsoleUI
{
    public class EfficientGameRenderer : IGameRenderer, IRowRenderer
    {
        private const string MustBePositiveNonZero = "Value must be a positive, non-zero integer.";

        public IEnumerable<string> Render()
        {
            throw new NotImplementedException();
        }

        public string RenderTopBorder(int numberOfRows, int numberOfColumns) =>
            RenderBoxArt(numberOfRows, numberOfColumns, '╔', '╦', '╗');

        private static string RenderBoxArt(int numberOfRows, int numberOfColumns, char left, char middle, char right)
        {
            if (numberOfRows <= 0) throw new ArgumentOutOfRangeException(nameof(numberOfRows), numberOfRows, MustBePositiveNonZero);
            if (numberOfColumns <= 0) throw new ArgumentOutOfRangeException(nameof(numberOfColumns), numberOfColumns, MustBePositiveNonZero);

            var prefixLength = GetNumberOfDigits(numberOfRows);
            var totalLength = prefixLength + 1 + numberOfColumns * 2 + 1;

            return string.Create(totalLength, prefixLength, (span, i) =>
            {
                span[..(prefixLength + 1)].Fill(' ');
                span[prefixLength + 1] = left;

                for (var x = span.Length - 3; x >= i + 3; x -= 2)
                {
                    span[x] = middle;
                    span[x - 1] = '═';
                }

                span[^2] = '═';
                span[^1] = right;
            });
        }

        public string RenderRow(int numberOfRows, int rowIndex, ICellRenderer cellRenderer, IEnumerable<Cell> cells)
        {
            if (rowIndex <= 0) throw new ArgumentOutOfRangeException(nameof(rowIndex), rowIndex, MustBePositiveNonZero);
            if (numberOfRows <= 0) throw new ArgumentOutOfRangeException(nameof(numberOfRows), numberOfRows, MustBePositiveNonZero);
            if (rowIndex > numberOfRows)
            {
                throw new InvalidOperationException("The row index must be less than the number of rows.")
                {
                    Data = { { "rowIndex", rowIndex }, { "numberOfRows", numberOfRows } }
                };
            }
            if (cellRenderer is null) throw new ArgumentNullException(nameof(cellRenderer));
            if (cells is null) throw new ArgumentNullException(nameof(cells));

            throw new NotImplementedException();
        }

        public string RenderRowSeparator(int numberOfRows, int numberOfColumns) =>
            RenderBoxArt(numberOfRows, numberOfColumns, '╠', '╬', '╣');

        public string RenderBottomBorder(int numberOfRows, int numberOfColumns) =>
            RenderBoxArt(numberOfRows, numberOfColumns, '╚', '╩', '╝');

        public IEnumerable<string> RenderColumnNames(int numberOfRows, IEnumerable<ColumnName> columnNames)
        {
            if (numberOfRows <= 0) throw new ArgumentOutOfRangeException(nameof(numberOfRows), numberOfRows, MustBePositiveNonZero);
            if (columnNames is null) throw new ArgumentNullException(nameof(columnNames));

            throw new NotImplementedException();
        }

        private static int GetNumberOfDigits(int value)
        {
            // This will not work for negative numbers, but we don't care; we've already checked the value
            // is a positive, non-zero value. We could just use the default arm in all cases, but using the
            // lookup is much faster for the range of values we would expect to receive
            return value switch
            {
                < 10 => 1,
                < 100 => 2,
                < 1_000 => 3,
                < 10_000 => 4,
                < 100_000 => 5,
                < 1_000_000 => 6,
                _ => value.ToString().Length
            };
        }
    }
}