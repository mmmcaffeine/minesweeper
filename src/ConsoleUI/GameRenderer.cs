using System;
using System.Collections.Generic;
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
            
            var length = numberOfRows.ToString().Length;
            var formatString = $"{{0,{length}}} ";

            return string.Format(formatString, rowIndex);
        }
    }
}