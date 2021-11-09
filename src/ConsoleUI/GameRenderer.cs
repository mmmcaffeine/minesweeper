using System;
using System.Collections.Generic;
using System.Linq;
using Dgt.Minesweeper.Engine;

namespace Dgt.Minesweeper.ConsoleUI
{
    public class GameRenderer
    {
        private readonly ICellRenderer _cellRenderer;

        public GameRenderer(ICellRenderer cellRenderer)
        {
            _cellRenderer = cellRenderer ?? throw new ArgumentNullException(nameof(cellRenderer));
        }

        // This is going to be slow and consume more memory than we need to. Considering this is likely to get
        // called a _lot_ we should benchmark this and try to improve the performance. We are also not making use
        // of the fact we have typed the return as ReadOnlySpan<char> instead of string
        public ReadOnlySpan<char> RenderRow(int rowIndex, int numberOfRows, IEnumerable<Cell> cells)
        {
            if(rowIndex <= 0) throw new ArgumentOutOfRangeException(nameof(rowIndex), rowIndex, "Value must be a positive, non-zero integer.");
            if(numberOfRows <= 0) throw new ArgumentOutOfRangeException(nameof(numberOfRows), numberOfRows, "Value must be a positive, non-zero integer.");
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

        public IEnumerable<string> RenderColumnNames(int numberOfRows, IEnumerable<ColumnName> columnNames)
        {
            if (numberOfRows <= 0) throw new ArgumentOutOfRangeException(nameof(numberOfRows), numberOfRows, "Value must be a positive, non-zero integer.");
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