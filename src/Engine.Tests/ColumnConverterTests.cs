using FluentAssertions;
using Xunit;

namespace Dgt.Minesweeper.Engine
{
    public class ColumnConverterTests
    {
        public static TheoryData<int, string> ConversionTestData => new()
        {
            { 1, "A" },
            { 5, "E" },
            { 26, "Z" }
        };

        [Theory]
        [MemberData(nameof(ConversionTestData))]
        public void ToColumnString_ShouldConvertIntegerToColumnString(int index, string expected) =>
            index.ToColumnString().Should().Be(expected);

        // We typically have the expected parameter as the last one in the list. However, swapping them around
        // means we can use the same set of test data for both methods
        [Theory]
        [MemberData(nameof(ConversionTestData))]
        public void FromColumnString_Should_ConvertColumnStringToInteger(int expectedIndex, string column) =>
            column.FromColumnString().Should().Be(expectedIndex);
    }
}