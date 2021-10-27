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
        public void ToColumnName_ShouldConvertIntegerToColumnString(int columnIndex, string expectedColumnName) =>
            columnIndex.ToColumnName().Should().Be(expectedColumnName);

        // We typically have the expected parameter as the last one in the list. However, swapping them around
        // means we can use the same set of test data for both methods
        [Theory]
        [MemberData(nameof(ConversionTestData))]
        public void ToColumnIndex_Should_ConvertColumnStringToInteger(int expectedColumnIndex, string columnName) =>
            columnName.ToColumnIndex().Should().Be(expectedColumnIndex);
    }
}