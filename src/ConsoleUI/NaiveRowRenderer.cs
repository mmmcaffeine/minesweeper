using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dgt.Minesweeper.Engine;

namespace Dgt.Minesweeper.ConsoleUI
{
    // This is going to be slow and consume more memory than we need to. Considering this is likely to get
    // called a _lot_ we should benchmark this and try to improve the performance. 
    public class NaiveRowRenderer : IRowRenderer
    {
        private const string MustBePositiveNonZero = "Value must be a positive, non-zero integer.";

        private readonly ICellRenderer _cellRenderer;

        public NaiveRowRenderer(ICellRenderer cellRenderer)
        {
            _cellRenderer = cellRenderer ?? throw new ArgumentNullException(nameof(cellRenderer));
        }

        public string RenderTopBorder(int numberOfRows, int numberOfColumns) =>
            RenderBoxArt(numberOfRows, numberOfColumns, '╔', '╦', '╗');

        public string RenderRow(int numberOfRows, int rowIndex, IEnumerable<Cell> cells)
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

            const char separator = '║';
            
            var length = numberOfRows.ToString().Length;
            var renderedCells = string.Join(separator, cells.Select(cell => _cellRenderer.RenderCell(cell)));
            var formatString = $"{{0,{length}}} {{1}}{{2}}{{1}}";
            
            return string.Format(formatString, rowIndex, separator, renderedCells, separator);
        }

        public string RenderBottomBorder(int numberOfRows, int numberOfColumns) =>
            RenderBoxArt(numberOfRows, numberOfColumns, '╚', '╩', '╝');

        public string RenderRowSeparator(int numberOfRows, int numberOfColumns) =>
            RenderBoxArt(numberOfRows, numberOfColumns, '╠', '╬', '╣');

        private static string RenderBoxArt(int numberOfRows, int numberOfColumns, char left, char middle, char right)
        {
            if (numberOfRows <= 0) throw new ArgumentOutOfRangeException(nameof(numberOfRows), numberOfRows, MustBePositiveNonZero);
            if (numberOfColumns <= 0) throw new ArgumentOutOfRangeException(nameof(numberOfColumns), numberOfColumns, MustBePositiveNonZero);
            
            var boxArtBuilder = new StringBuilder();
            
            boxArtBuilder.Append(' ', numberOfRows.ToString().Length);
            boxArtBuilder.Append(' ');
            boxArtBuilder.Append(left);
            boxArtBuilder.Append('═');
            
            for (var i = 0; i < numberOfColumns - 1; i++)
            {
                boxArtBuilder.Append(middle);
                boxArtBuilder.Append('═');
            }
            
            boxArtBuilder.Append(right);

            return boxArtBuilder.ToString();
        }

        public IEnumerable<string> RenderColumnNames(int numberOfRows, IEnumerable<ColumnName> columnNames)
        {
            if (numberOfRows <= 0) throw new ArgumentOutOfRangeException(nameof(numberOfRows), numberOfRows, MustBePositiveNonZero);
            if (columnNames is null) throw new ArgumentNullException(nameof(columnNames));
            
            var rowIndicesPrefix = new string(' ', numberOfRows.ToString().Length);
            var columnNameStrings = columnNames.Select(cn => (string)cn).ToList();
            
            for (var i = 0; i < columnNameStrings.Max(s => s.Length); i++)
            {
                // Don't use i in the lambda expression. To do so would capture a variable that is being modified
                // in the outer scope!
                var currentIndex = i; 
                var currentCharacters = columnNameStrings.Select(s => currentIndex < s.Length ? s[currentIndex] : ' ');
                var joinedCharacters = string.Join(' ', currentCharacters);
            
                // We need the second space after the row indices to account for the left border of the box art
                yield return $"{rowIndicesPrefix}  {joinedCharacters}";
            }
        }
    }
}