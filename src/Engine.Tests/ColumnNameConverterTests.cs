using System;
using FluentAssertions;
using Xunit;

namespace Dgt.Minesweeper.Engine
{
    public class ColumnNameConverterTests
    {
        public static TheoryData<int, string> ConversionTestData => new()
        {
            { 1, "A" },
            { 5, "E" },
            { 26, "Z" },
            { 27, "AA" },
            { 28, "AB" },
            { 52, "AZ" },
            { 53, "BA" },
            { 79, "CA" },
            { 80, "CB" },
            { 677, "ZA"},
            { 702, "ZZ" },
            { 703, "AAA" },
            { 728, "AAZ" },
            { 729, "ABA" },
            { 730, "ABB" },
            { 744, "ABP" },
            { 754, "ABZ" },
            { 755, "ACA" },
            { 1500, "BER" }
        };

        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(-1)]
        [InlineData(0)]
        public void ToColumnName_Should_ThrowWhenIndexIsZeroOrNegative(int columnIndex)
        {
            // Arrange, Act
            Action act = () => _ = columnIndex.ToColumnName();
            
            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>()
                .WithMessage("Value must be a positive, non-zero integer.*")
                .WithParameterName("columnIndex")
                .And.ActualValue.Should().Be(columnIndex);
        }

        [Theory]
        [MemberData(nameof(ConversionTestData))]
        public void ToColumnName_ShouldConvertIntegerToColumnName(int columnIndex, string expectedColumnName) =>
            columnIndex.ToColumnName().Should().Be(expectedColumnName);

        // We typically have the expected parameter as the last one in the list. However, swapping them around
        // means we can use the same set of test data for both methods
        [Theory]
        [MemberData(nameof(ConversionTestData))]
        public void ToColumnIndex_Should_ConvertColumnNameToInteger(int expectedColumnIndex, string columnName) =>
            columnName.ToColumnIndex().Should().Be(expectedColumnIndex);
    }
}