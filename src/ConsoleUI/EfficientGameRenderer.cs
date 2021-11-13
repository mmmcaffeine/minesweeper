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

        public string RenderTopBorder(int numberOfRows, int numberOfColumns)
        {
            if (numberOfRows <= 0) throw new ArgumentOutOfRangeException(nameof(numberOfRows), numberOfRows, MustBePositiveNonZero);
            if (numberOfColumns <= 0) throw new ArgumentOutOfRangeException(nameof(numberOfColumns), numberOfColumns, MustBePositiveNonZero);

            var prefixLength = numberOfRows.ToString().Length;
            var totalLength = prefixLength + 1 + numberOfColumns * 2 + 1;

            return string.Create(totalLength, prefixLength, (span, i) =>
            {
                // Would it be more efficient to _not_ pre-fill the span, but fill it within the loop?
                span.Fill('═');

                span[..(prefixLength + 1)].Fill(' ');
                span[prefixLength + 1] = '╔';

                for (var x = span.Length - 3; x >= i + 3; x -= 2)
                {
                    span[x] = '╦';
                }

                span[^1] = '╗';
            });
        }

        public string RenderRow(int numberOfRows, int rowIndex, ICellRenderer cellRenderer, IEnumerable<Cell> cells)
        {
            throw new NotImplementedException();
        }

        public string RenderRowSeparator(int numberOfRows, int numberOfColumns)
        {
            throw new NotImplementedException();
        }

        public string RenderBottomBorder(int numberOfRows, int numberOfColumns)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> RenderColumnNames(int numberOfRows, IEnumerable<ColumnName> columnNames)
        {
            throw new NotImplementedException();
        }
    }
}