using System.Collections.Generic;
using Dgt.Minesweeper.Engine;

namespace Dgt.Minesweeper.ConsoleUI
{
    public interface IRowRenderer
    {
        string RenderTopBorder(int numberOfRows, int numberOfColumns);
        string RenderRow(int numberOfRows, int rowIndex, ICellRenderer cellRenderer, IEnumerable<Cell> cells);
        string RenderRowSeparator(int numberOfRows, int numberOfColumns);
        string RenderBottomBorder(int numberOfRows, int numberOfColumns);
        IEnumerable<string> RenderColumnNames(int numberOfRows, IEnumerable<ColumnName> columnNames);
    }
}