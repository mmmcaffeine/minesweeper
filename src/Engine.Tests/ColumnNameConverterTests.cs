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

        [Fact]
        public void ToColumnIndex_Should_ThrowWhenColumnNameIsNull()
        {
            // Arrange, Act
            Action act = () => _ = ((string)null!).ToColumnIndex();
            
            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithMessage("Value cannot be null.*")
                .WithMessage("*Value must be a string consisting only of letters e.g. 'ABC'.*")
                .WithParameterName("columnName");
        }

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData("\t")]
        [InlineData("\r")]
        [InlineData("\n")]
        public void ToColumnIndex_Should_ThrowWhenColumnNameIsEmptyOrWhiteSpace(string columnName)
        {
            // Arrange, Act
            Action act = () => _ = columnName.ToColumnIndex();
            
            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("Value cannot be whitespace or an empty string.*")
                .WithMessage("*Value must be a string consisting only of letters e.g. 'ABC'.*")
                .WithParameterName("columnName")
                .Where(ex => ex.Data.Contains("columnName") && ex.Data["columnName"]!.Equals(columnName));
        }

        [Theory]
        [InlineData("AA7")]
        [InlineData("6BB")]
        [InlineData("CC9")]
        [InlineData("D-D")]
        [InlineData("-E")]
        [InlineData("F+")]
        public void ToColumnIndex_Should_ThrowWhenColumnNameContainsNonLetters(string columnName)
        {
            // Arrange, Act
            Action act = () => _ = columnName.ToColumnIndex();
            
            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("Input string was not in a correct format.*")
                .WithMessage("*Value must be a string consisting only of letters e.g. 'ABC'.*")
                .WithParameterName("columnName")
                .Where(ex => ex.Data.Contains("columnName") && ex.Data["columnName"]!.Equals(columnName));
        }

        // We typically have the expected parameter as the last one in the list. However, swapping them around
        // means we can use the same set of test data for both methods
        [Theory]
        [MemberData(nameof(ConversionTestData))]
        public void ToColumnIndex_Should_ConvertColumnNameToInteger(int expectedColumnIndex, string columnName) =>
            columnName.ToColumnIndex().Should().Be(expectedColumnIndex);
    }
}