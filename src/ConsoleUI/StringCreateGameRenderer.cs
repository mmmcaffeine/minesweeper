﻿using System;
using System.Collections.Generic;
using System.Linq;
using Dgt.Minesweeper.Engine;

namespace Dgt.Minesweeper.ConsoleUI
{
    public class StringCreateGameRenderer : IGameRenderer, IRowRenderer
    {
        private const string MustBePositiveNonZero = "Value must be a positive, non-zero integer.";

        private readonly Game _game;
        private readonly ICellRenderer _cellRenderer;

        public StringCreateGameRenderer(Game game, ICellRenderer cellRenderer)
        {
            _game = game ?? throw new ArgumentNullException(nameof(game));
            _cellRenderer = cellRenderer ?? throw new ArgumentNullException(nameof(cellRenderer));
        }

        public IEnumerable<string> Render()
        {
            var rowSeparator = RenderRowSeparator(_game.NumberOfRows, _game.NumberOfColumns);

            yield return RenderTopBorder(_game.NumberOfRows, _game.NumberOfColumns);

            for (var i = _game.NumberOfRows; i > 0 ; i--)
            {
                yield return RenderRow(_game.NumberOfRows, i, _cellRenderer, _game.GetRow(i));

                if (i > 1)
                {
                    yield return rowSeparator;
                }
            }

            yield return RenderBottomBorder(_game.NumberOfRows, _game.NumberOfColumns);
            yield return string.Empty;

            foreach (var line in RenderColumnNames(_game.NumberOfRows, _game.ColumnNames))
            {
                yield return line;
            }
        }

        public string RenderTopBorder(int numberOfRows, int numberOfColumns) =>
            RenderBoxArt(numberOfRows, numberOfColumns, '╔', '╦', '╗');

        private static string RenderBoxArt(int numberOfRows, int numberOfColumns, char left, char middle, char right)
        {
            if (numberOfRows <= 0) throw new ArgumentOutOfRangeException(nameof(numberOfRows), numberOfRows, MustBePositiveNonZero);
            if (numberOfColumns <= 0) throw new ArgumentOutOfRangeException(nameof(numberOfColumns), numberOfColumns, MustBePositiveNonZero);

            var prefixLength = GetNumberOfDigits(numberOfRows);
            var totalLength = prefixLength + 1 + numberOfColumns * 2 + 1;

            return string.Create(totalLength, prefixLength, (span, state) =>
            {
                span[..(prefixLength + 1)].Fill(' ');
                span[prefixLength + 1] = left;

                for (var i = span.Length - 3; i >= state + 3; i -= 2)
                {
                    span[i] = middle;
                    span[i - 1] = '═';
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

            var renderedCells = cells.Select(cellRenderer.RenderCell).ToArray();
            var rowHeaderLength = GetNumberOfDigits(numberOfRows);
            var totalLength = rowHeaderLength + 1 + renderedCells.Length * 2 + 1;

            // It is unclear if we need to pass everything we need in the SpanAction to avoid any closures and
            // thus unwanted heap allocations
            // See https://www.stevejgordon.co.uk/creating-strings-with-no-allocation-overhead-using-string-create-csharp
            return string.Create(totalLength, (rowHeaderLength, rowIndex, renderedCells), RenderRow);
        }

        private static void RenderRow(Span<char> span, (int RowHeaderLength, int RowIndex, char[] RenderedCells) state)
        {
            var (rowHeaderLength, rowIndex, renderedCells) = state;
            var rowIndexString = rowIndex.ToString();

            for (var i = 0; i < rowHeaderLength; i++)
            {
                span[i] = rowHeaderLength - i <= rowIndexString.Length
                    ? rowIndexString[i - rowHeaderLength + rowIndexString.Length]
                    : ' ';
            }

            span[rowHeaderLength] = ' ';

            for (var i = 0; i < renderedCells.Length; i++)
            {
                span[rowHeaderLength + 1 + (i * 2)] = '║';
                span[rowHeaderLength + 2 + (i * 2)] = renderedCells[i];
            }

            span[^1] = '║';
        }

        public string RenderRowSeparator(int numberOfRows, int numberOfColumns) =>
            RenderBoxArt(numberOfRows, numberOfColumns, '╠', '╬', '╣');

        public string RenderBottomBorder(int numberOfRows, int numberOfColumns) =>
            RenderBoxArt(numberOfRows, numberOfColumns, '╚', '╩', '╝');

        public IEnumerable<string> RenderColumnNames(int numberOfRows, IEnumerable<ColumnName> columnNames)
        {
            if (numberOfRows <= 0) throw new ArgumentOutOfRangeException(nameof(numberOfRows), numberOfRows, MustBePositiveNonZero);
            if (columnNames is null) throw new ArgumentNullException(nameof(columnNames));

            var columnNameArrays = columnNames.Select(cn => ((string)cn).ToCharArray()).ToArray();
            var rowHeaderLength = GetNumberOfDigits(numberOfRows);
            var totalLength = rowHeaderLength + 1 + columnNameArrays.Length * 2;
            var maximumColumnNameLength = columnNameArrays.Max(a => a.Length);

            for (var i = 0; i < maximumColumnNameLength; i++)
            {
                yield return string.Create(totalLength, (rowHeaderLength, columnNameArrays, i), (span, state) =>
                {
                    var (rhl, cna, lineIndex) = state;

                    for (var spanIndex = 0; spanIndex <= rowHeaderLength; spanIndex++)
                    {
                        span[spanIndex] = ' ';
                    }

                    for (var columnNameIndex = 0; columnNameIndex < cna.Length; columnNameIndex++)
                    {
                        span[rhl + (columnNameIndex * 2) + 1] = ' ';
                        span[rhl + (columnNameIndex * 2) + 2] = cna[columnNameIndex].Length > lineIndex
                            ? cna[columnNameIndex][lineIndex]
                            : ' ';
                    }
                });
            }
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