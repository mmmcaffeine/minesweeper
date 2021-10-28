using System.Collections.Generic;
using System.Text;

namespace Dgt.Minesweeper.Engine
{
    public class MinefieldRenderer
    {
        public IEnumerable<string> Render(IMinefield minefield)
        {
            var lines = new Stack<string>(minefield.NumberOfRows);
            
            for (var rowIndex = 0; rowIndex < minefield.NumberOfRows; rowIndex++)
            {
                lines.Push(RenderRow(minefield, rowIndex));
            }

            return lines;
        }

        private static string RenderRow(IMinefield minefield, int rowIndex)
        {
            var builder = new StringBuilder(minefield.NumberOfColumns);

            for (var columnIndex = 0; columnIndex < minefield.NumberOfColumns; columnIndex++)
            {
                builder.Append(RenderLocation(minefield, columnIndex, rowIndex));
            }

            return builder.ToString();
        }

        private static string RenderLocation(IMinefield minefield, int columnIndex, int rowIndex)
        {
            var location = new Location((ColumnName)(columnIndex + 1), rowIndex + 1);
            
            return minefield.IsMined(location)
                ? "*"
                : minefield.GetHint(location).ToString();
        }
    }
}