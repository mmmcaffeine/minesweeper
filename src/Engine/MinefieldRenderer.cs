using System.Collections.Generic;
using System.Text;

namespace Dgt.Minesweeper.Engine
{
    public class MinefieldRenderer
    {
        public IEnumerable<string> Render(Minefield minefield)
        {
            var lines = new List<string>(minefield.Rows);
            
            for (var row = 0; row < minefield.Rows; row++)
            {
                lines.Add(RenderRow(minefield, row));
            }

            return lines;
        }

        private static string RenderRow(Minefield minefield, int row)
        {
            var builder = new StringBuilder(minefield.Columns);

            for (var column = 0; column < minefield.Columns; column++)
            {
                var cell = new Cell(column, row);
                var value = minefield.IsMined(cell)
                    ? "*"
                    : minefield.GetHint(cell).ToString();
                
                builder.Append(value);
            }

            return builder.ToString();
        }
    }
}