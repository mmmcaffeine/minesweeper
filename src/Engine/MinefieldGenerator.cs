using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Dgt.Minesweeper.Engine
{
    public class MinefieldGenerator
    {
        private const string HeaderPattern = @"^(?<numberOfRows>\d+)\s+(?<numberOfColumns>\d+)$";
        private const string MinePattern = @"\*";
        
        private static readonly Regex HeaderRegex = new(HeaderPattern);
        private static readonly Regex MineRegex = new(MinePattern);
        
        // TODO Parameter validation...
        // * We must have at least two elements (a header and row that makes up the field)
        // * All elements other than the first should be of the length indicated in the header
        // * The number elements other than the first should match the length indicated in the header
        // * Elements that make up the field can only be '.' or '*'
        public IMinefield GenerateMinefield(IEnumerable<string> lines)
        {
            var listOfLines = lines.ToList();
            var headerMatch = HeaderRegex.Match(listOfLines.First());
            var numberOfColumns = int.Parse(headerMatch.Groups["numberOfColumns"].Value);
            var numberOfRows = int.Parse(headerMatch.Groups["numberOfRows"].Value);
            var minedLocations = listOfLines
                .Skip(1)
                .Select((line, index) => GetMinedLocations(line, index, numberOfRows))
                .SelectMany(location => location);

            return GenerateMinefield(numberOfColumns, numberOfRows, minedLocations.ToArray());
        }

        private static IEnumerable<Location> GetMinedLocations(string line, int rowIndex, int countOfRows)
        {
            foreach (Match match in MineRegex.Matches(line))
            {
                var locationColumnIndex = match.Index + 1;
                var locationRowIndex = countOfRows - rowIndex;

                yield return new Location((ColumnName)locationColumnIndex, locationRowIndex);
            }
        }

        public IMinefield GenerateMinefield(int numberOfColumns, int numberOfRows, params Location[] minedLocations)
        {
            return new Minefield(numberOfColumns, numberOfRows, minedLocations);
        }
    }
}