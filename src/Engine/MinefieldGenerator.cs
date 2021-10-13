using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Dgt.Minesweeper.Engine
{
    public class MinefieldGenerator
    {
        private const string HeaderPattern = @"^(?<rows>\d+)\s+(?<columns>\d+)$";
        private const string MinePattern = @"\*";
        
        private static readonly Regex HeaderRegex = new(HeaderPattern);
        private static readonly Regex MineRegex = new(MinePattern);
        
        // TODO Parameter validation...
        // * We must have at least two elements (a header and row that makes up the field)
        // * All elements other than the first should be of the length indicated in the header
        // * The number elements other than the first should match the length indicated in the header
        // * Elements that make up the field can only be '.' or '*'
        public Minefield GenerateMinefield(IEnumerable<string> lines)
        {
            var listOfLines = lines.ToList();
            var headerMatch = HeaderRegex.Match(listOfLines.First());
            var columns = int.Parse(headerMatch.Groups["columns"].Value);
            var rows = int.Parse(headerMatch.Groups["rows"].Value);
            var minedCells = listOfLines
                .Skip(1)
                .Select((line, index) => GetMinedCells(index, line))
                .SelectMany(cell => cell);

            return GenerateMinefield(columns, rows, minedCells.ToArray());
        }

        private static IEnumerable<Cell> GetMinedCells(int row, string line)
        {
            foreach (Match match in MineRegex.Matches(line))
            {
                yield return new Cell(match.Index, row);
            }
        }

        public Minefield GenerateMinefield(int columns, int rows, params Cell[] minedCells)
        {
            return new Minefield(columns, rows, minedCells);
        }
    }
}