using System;
using FluentAssertions;
using Xunit;

namespace Dgt.Minesweeper.Engine
{
    public class ColumnNameTests
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
        public void ExplicitConversionFromInteger_Should_ThrowWhenIndexIsZeroOrNegative(int columnIndex)
        {
            // Arrange, Act
            Action act = () => _ = (ColumnName)columnIndex;
            
            // Assert
            act.Should().Throw<InvalidCastException>()
                .WithMessage("Specified cast is not valid.*")
                .WithMessage("*Value must be a positive, non-zero integer.*")
                .Where(ex => ex.Data.Contains("value") && ex.Data["value"]!.Equals(columnIndex));
        }

        [Theory]
        [MemberData(nameof(ConversionTestData))]
        public void ExplicitConversionFromInteger_Should_ConvertIntegerToColumnName(int columnIndex, string expectedValue) =>
            ((ColumnName)columnIndex).Value.Should().Be(expectedValue);
        
        [Fact]
        public void ExplicitConversionFromString_Should_ThrowWhenValueIsNull()
        {
            // Arrange, Act
            Action act = () => _ = (ColumnName)((string)null!);
            
            // Assert
            act.Should().Throw<InvalidCastException>()
                .WithMessage("Specified cast is not valid.*")
                .WithMessage("*Value must be a string consisting only of letters e.g. 'ABC'.*")
                .Where(ex => ex.Data.Contains("value") && ex.Data["value"] == null);
        }

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData("\t")]
        [InlineData("\r")]
        [InlineData("\n")]
        public void ExplicitConversionFromString_Should_ThrowWhenValueIsEmptyOrWhiteSpace(string value)
        {
            // Arrange, Act
            Action act = () => _ = (ColumnName)value;
            
            // Assert
            act.Should().Throw<InvalidCastException>()
                .WithMessage("Specified cast is not valid.*")
                .WithMessage("*Value must be a string consisting only of letters e.g. 'ABC'.*")
                .Where(ex => ex.Data.Contains("value") && ex.Data["value"]!.Equals(value));
        }

        [Theory]
        [InlineData("AA7")]
        [InlineData("6BB")]
        [InlineData("CC9")]
        [InlineData("D-D")]
        [InlineData("-E")]
        [InlineData("F+")]
        public void ExplicitConversionFromString_Should_ThrowWhenValueContainsNonLetters(string value)
        {
            // Arrange, Act
            Action act = () => _ = (ColumnName)value;
            
            // Assert
            act.Should().Throw<InvalidCastException>()
                .WithMessage("Specified cast is not valid.*")
                .WithMessage("*Value must be a string consisting only of letters e.g. 'ABC'.*")
                .Where(ex => ex.Data.Contains("value") && ex.Data["value"]!.Equals(value));
        }

        [Theory]
        [InlineData("a")]
        [InlineData("bc")]
        [InlineData("def")]
        public void ExplicitConversionFromString_Should_InitialiseValueToUpperCase(string value) =>
            ((ColumnName)value).Value.Should().BeUpperCased();

        [Theory]
        [InlineData("A")]
        [InlineData("BC")]
        [InlineData("DEF")]
        public void ExplicitConversionFromString_Should_ConvertStringToColumnName(string value)
            => ((ColumnName)value).Value.Should().Be(value);

        [Fact]
        public void Ctor_Should_ThrowWhenValueIsNull()
        {
            // Arrange, Act
            Action act = () => _ = new ColumnName(null!);
            
            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithMessage("Value cannot be null.*")
                .WithMessage("*Value must be a string consisting only of letters e.g. 'ABC'.*")
                .WithParameterName("value");
        }

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData("\t")]
        [InlineData("\r")]
        [InlineData("\n")]
        public void Ctor_Should_ThrowWhenValueIsEmptyOrWhiteSpace(string value)
        {
            // Arrange, Act
            Action act = () => _ = new ColumnName(value);
            
            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("Value cannot be whitespace or an empty string.*")
                .WithMessage("*Value must be a string consisting only of letters e.g. 'ABC'.*")
                .WithParameterName("value")
                .Where(ex => ex.Data.Contains("value") && ex.Data["value"]!.Equals(value));
        }

        [Theory]
        [InlineData("AA7")]
        [InlineData("6BB")]
        [InlineData("CC9")]
        [InlineData("D-D")]
        [InlineData("-E")]
        [InlineData("F+")]
        public void Ctor_Should_ThrowWhenValueContainsNonLetters(string value)
        {
            // Arrange, Act
            Action act = () => _ = new ColumnName(value);
            
            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("Input string was not in a correct format.*")
                .WithMessage("*Value must be a string consisting only of letters e.g. 'ABC'.*")
                .WithParameterName("value")
                .Where(ex => ex.Data.Contains("value") && ex.Data["value"]!.Equals(value));
        }

        [Theory]
        [InlineData("a")]
        [InlineData("bc")]
        [InlineData("def")]
        public void Ctor_Should_InitialiseValueToUpperCase(string value) =>
            new ColumnName(value).Value.Should().BeUpperCased();

        [Fact]
        public void ImplicitConversionToInteger_Should_NotThrowOnNull()
        {
            // Arrange
            ColumnName columnName = null!;
            
            // Act
            int actual = columnName;
            
            // Assert
            actual.Should().Be(0);
        }

        // We typically have the expected parameter as the last one in the list. However, swapping them around
        // means we can use the same set of test data for both converting a ColumnName to an int and vice versa
        [Theory]
        [MemberData(nameof(ConversionTestData))]
        public void ImplicitConversionToInteger_Should_ConvertColumnNameToInteger(int expectedColumnIndex, string value)
        {
            // Arrange
            var columnName = new ColumnName(value);

            // Act
            int columnIndex = columnName;

            // Assert
            columnIndex.Should().Be(expectedColumnIndex);
        }

        [Fact]
        public void ImplicitConversionToString_Should_NotThrowOnNull()
        {
            // Arrange
            ColumnName columnName = null!;
            
            // Act
            string actual = columnName;
            
            // Assert
            actual.Should().BeEmpty();
        }

        [Theory]
        [InlineData("A")]
        [InlineData("BC")]
        [InlineData("DEF")]
        public void ImplicitConversionToString_Should_ConvertColumnNameToString(string value)
        {
            // Arrange
            var columnName = new ColumnName(value);
            
            // Act
            string actual = columnName;
            
            // Assert
            actual.Should().Be(value);
        }
    }
}