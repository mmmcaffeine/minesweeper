using System;
using System.Collections.Generic;
using System.Linq;
using Dgt.Minesweeper.Engine;

namespace Dgt.Minesweeper.ConsoleUI
{
    public class CharArrayGameRenderer : IGameRenderer, IRowRenderer
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

            var rowHeaderLength = numberOfRows.ToString().Length;
            var boxArtLength = 1 + (numberOfColumns * 2) + 1;
            var totalLength = rowHeaderLength + boxArtLength;
            var chars = new char[totalLength];

            for (var i = 0; i <= rowHeaderLength + 1; i++)
            {
                chars[i] = ' ';
            }

            chars[rowHeaderLength + 1] = left;

            for (var i = chars.Length - 3; i >= rowHeaderLength + 3; i -= 2)
            {
                chars[i] = middle;
                chars[i - 1] = '═';
            }

            chars[^2] = '═';
            chars[^1] = right;

            return new string(chars);
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

            var renderedCells = cells.Select(cellRenderer.RenderCell).ToArray();
            var rowHeaderLength = GetNumberOfDigits(numberOfRows);
            var totalLength = rowHeaderLength + 1 + renderedCells.Length * 2 + 1;
            var chars = new char[totalLength];
            var rowIndexChars = rowIndex.ToString().ToCharArray();

            for (var i = 0; i < rowHeaderLength; i++)
            {
                chars[i] = rowHeaderLength - i <= rowIndexChars.Length
                    ? rowIndexChars[i - rowHeaderLength + rowIndexChars.Length]
                    : ' ';
            }

            chars[rowHeaderLength] = ' ';

            for (var i = 0; i < renderedCells.Length; i++)
            {
                chars[rowHeaderLength + 1 + (i * 2)] = '║';
                chars[rowHeaderLength + 2 + (i * 2)] = renderedCells[i];
            }

            chars[^1] = '║';

            return new string(chars);
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

        public string RenderRowSeparator(int numberOfRows, int numberOfColumns) =>
            RenderBoxArt(numberOfRows, numberOfColumns, '╠', '╬', '╣');

        public string RenderBottomBorder(int numberOfRows, int numberOfColumns) =>
            RenderBoxArt(numberOfRows, numberOfColumns, '╚', '╩', '╝');

        public IEnumerable<string> RenderColumnNames(int numberOfRows, IEnumerable<ColumnName> columnNames)
        {
            if (numberOfRows <= 0) throw new ArgumentOutOfRangeException(nameof(numberOfRows), numberOfRows, MustBePositiveNonZero);
            if (columnNames is null) throw new ArgumentNullException(nameof(columnNames));

            var columnNameArrays = columnNames.Select(cn => cn.Value.ToCharArray()).ToArray();
            var rowHeaderLength = GetNumberOfDigits(numberOfRows);
            var totalLength = rowHeaderLength + 1 + columnNameArrays.Length * 2;
            var maximumColumnNameLength = GetMaximumLength(columnNameArrays);

            for (var linesIndex = 0; linesIndex < maximumColumnNameLength; linesIndex++)
            {
                var lineChars = new char[totalLength];

                for (var i = 0; i <= rowHeaderLength; i++)
                {
                    lineChars[i] = ' ';
                }

                for (var columnNameIndex = 0; columnNameIndex < columnNameArrays.Length; columnNameIndex++)
                {
                    lineChars[rowHeaderLength + (columnNameIndex * 2) + 1] = ' ';
                    lineChars[rowHeaderLength + (columnNameIndex * 2) + 2] = columnNameArrays[columnNameIndex].Length > linesIndex
                        ? columnNameArrays[columnNameIndex][linesIndex]
                        : ' ';
                }

                yield return new string(lineChars);
            }
        }

        private static int GetMaximumLength(char[][] arrays)
        {
            var maxLength = 0;

            foreach (var array in arrays)
            {
                if (array.Length > maxLength)
                {
                    maxLength = array.Length;
                }
            }

            return maxLength;
        }
    }
}