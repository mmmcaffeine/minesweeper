using System;
using System.Collections.Generic;
using Dgt.Minesweeper.Engine;

namespace Dgt.Minesweeper.ConsoleUI
{
    public class NaiveGameRenderer : IGameRenderer
    {
        private readonly Game _game;
        private readonly IRowRenderer _rowRenderer;
        private readonly ICellRenderer _cellRenderer;

        public NaiveGameRenderer(Game game, IRowRenderer rowRenderer, ICellRenderer cellRenderer)
        {
            _game = game ?? throw new ArgumentNullException(nameof(game));
            _rowRenderer = rowRenderer ?? throw new ArgumentNullException(nameof(rowRenderer));
            _cellRenderer = cellRenderer ?? throw new ArgumentNullException(nameof(cellRenderer));
        }
        
        public IEnumerable<string> Render()
        {
            var rows = new List<string>();
            
            rows.Add(_rowRenderer.RenderTopBorder(_game.NumberOfRows, _game.NumberOfColumns));

            for (var i = _game.NumberOfRows; i > 0; i--)
            {
                rows.Add(_rowRenderer.RenderRow(_game.NumberOfRows, i, _cellRenderer, _game.GetRow(i)));

                if (i > 1)
                {
                    rows.Add(_rowRenderer.RenderRowSeparator(_game.NumberOfRows, _game.NumberOfColumns));
                }
            }
            
            rows.Add(_rowRenderer.RenderBottomBorder(_game.NumberOfRows, _game.NumberOfColumns));
            rows.Add(string.Empty);
            rows.AddRange(_rowRenderer.RenderColumnNames(_game.NumberOfRows, _game.ColumnNames));

            return rows;
        }
    }
}