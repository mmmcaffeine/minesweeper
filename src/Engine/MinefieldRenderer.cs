using System.Collections.Generic;
using System.Text;

namespace Dgt.Minesweeper.Engine
{
    public class MinefieldRenderer
    {
        public IEnumerable<string> Render(IMinefield minefield)
        {
            var lines = new List<string>(minefield.NumberOfRows);
            
            for (var rowIndex = 0; rowIndex < minefield.NumberOfRows; rowIndex++)
            {
                lines.Add(RenderRow(minefield, rowIndex));
            }

            return lines;
        }

        private static string RenderRow(IMinefield minefield, int rowIndex)
        {
            var builder = new StringBuilder(minefield.NumberOfColumns);

            for (var columnIndex = 0; columnIndex < minefield.NumberOfColumns; columnIndex++)
            {
                builder.Append(RenderCell(minefield, columnIndex, rowIndex));
            }

            return builder.ToString();
        }

        private static string RenderCell(IMinefield minefield, int columnIndex, int rowIndex)
        {
            var cell = new Cell(columnIndex, rowIndex);
            return minefield.IsMined(cell)
                ? "*"
                : minefield.GetHint(cell).ToString();
        }
    }
}