using FluentAssertions;
using Xunit;

namespace Dgt.Minesweeper.Engine
{
    public class ColumnConverterTests
    {
        public static TheoryData<int, string> ToColumnStringTestData => new()
        {
            { 1, "A" },
            { 5, "E" },
            { 26, "Z" }
        };
        
        public static TheoryData<string, int> FromColumnStringTestData => new()
        {
            { "A", 1 },
            { "E", 5 },
            { "Z", 26 }
        };

        [Theory]
        [MemberData(nameof(ToColumnStringTestData))]
        public void ToColumnString_ShouldConvertIntegerToColumnString(int index, string expected) =>
            index.ToColumnString().Should().Be(expected);

        [Theory]
        [MemberData(nameof(FromColumnStringTestData))]
        public void FromColumnString_Should_ConvertColumnStringToInteger(string column, int expected) =>
            column.FromColumnString().Should().Be(expected);
    }
}