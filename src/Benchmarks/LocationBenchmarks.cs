using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using BenchmarkDotNet.Attributes;
using Dgt.Minesweeper.Engine;

namespace Dgt.Minesweeper.Benchmarks
{
    [MemoryDiagnoser]
    public class LocationBenchmarks
    {
        private Regex _parseRegex = default!;
        private Regex _columnRegex = default!;
        
        [GlobalSetup]
        public void GlobalSetup()
        {
            _parseRegex = new Regex(@"^\s*(?<column>[A-Z]+)\s*(?<row>\d+)\s*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            _columnRegex = new Regex(@"^[A_Z]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }

        public static IEnumerable<string> ValuesForLocation
        {
            get
            {
                yield return "A1";
                yield return "ZZZZ1000";
                yield return "1A";
                yield return "Nope";
                yield return string.Empty;
            }
        }

        [Benchmark(Baseline = true)]
        [ArgumentsSource(nameof(ValuesForLocation))]
#pragma warning disable CA1822
        public bool TryParse_DelegatingToParse(string location)
#pragma warning restore CA1822
        {
            return DoTryParseDelegatingToParse(location, out _);
        }

        private static bool DoTryParseDelegatingToParse(string s, [NotNullWhen(true)]out Location? result)
        {
            try
            {
                result = Location.Parse(s);
            }
            catch
            {
                result = null;
            }

            return result != null;
        }

        [Benchmark]
        [ArgumentsSource(nameof(ValuesForLocation))]
        public bool TryParse_TestingForPatternMatch(string location)
        {
            return DoTryParseTestingForPatternMatch(location, out _);
        }

        private bool DoTryParseTestingForPatternMatch(string s, out Location? result)
        {
            var match = _parseRegex.Match(s);

            result = match.Success
                ? new Location(match.Groups["column"].Value, int.Parse(match.Groups["row"].Value))
                : null;

            return result != null;
        }

        public static IEnumerable<string> ValuesForColumn
        {
            get
            {
                yield return "A";
                yield return "ZZZZZZ";
                yield return "1A";
                yield return "Nope";
                yield return string.Empty;
            }
        }

        [Benchmark(Baseline = true)]
        [ArgumentsSource(nameof(ValuesForColumn))]
#pragma warning disable CA1822
        public bool ValidateColumn_EnumeratingChars(string column)
#pragma warning restore CA1822
        {
            // ReSharper disable once SimplifyLinqExpressionUseAll
            // We wish to replicate the implementation in Location which wraps this test in an if block to decide
            // whether to throw
            return !column.Any(c => !char.IsLetter(c));
        }

        [Benchmark]
        [ArgumentsSource(nameof(ValuesForColumn))]
        public bool ValidateColumn_TestingPattern(string column)
        {
            return _columnRegex.IsMatch(column);
        }
    }
}