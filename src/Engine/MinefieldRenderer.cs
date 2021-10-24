using System.Collections.Generic;
using System.Text;

namespace Dgt.Minesweeper.Engine
{
    public class MinefieldRenderer
    {
        public IEnumerable<string> Render(Minefield minefield)
        {
            var lines = new List<string>(minefield.NumberOfRows);
            
            for (var rowIndex = 0; rowIndex < minefield.NumberOfRows; rowIndex++)
            {
                lines.Add(RenderRow(minefield, rowIndex));
            }

            return lines;
        }

        private static string RenderRow(Minefield minefield, int rowIndex)
        {
            var builder = new StringBuilder(minefield.NumberOfColumns);

            for (var columnIndex = 0; columnIndex < minefield.NumberOfColumns; columnIndex++)
            {
                var cell = new Cell(columnIndex, rowIndex);
                var value = minefield.IsMined(cell)
                    ? "*"
                    : minefield.GetHint(cell).ToString();
                
                builder.Append(value);
            }

            return builder.ToString();
        }
    }
}