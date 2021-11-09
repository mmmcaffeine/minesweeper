using System;
using System.Collections.Generic;
using Dgt.Minesweeper.Engine;

namespace Dgt.Minesweeper.ConsoleUI
{
    public interface IRowRenderer
    {
        ReadOnlySpan<char> RenderTopBorder(int numberOfRows, int numberOfColumns);
        ReadOnlySpan<char> RenderRow(int rowIndex, int numberOfRows, IEnumerable<Cell> cells);
        ReadOnlySpan<char> RenderRowSeparator(int numberOfRows, int numberOfColumns);
        ReadOnlySpan<char> RenderBottomBorder(int numberOfRows, int numberOfColumns);
        IEnumerable<string> RenderColumnNames(int numberOfRows, IEnumerable<ColumnName> columnNames);
    }
}