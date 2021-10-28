using System;
using FluentAssertions;
using FluentAssertions.Execution;
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

        public static TheoryData<string> EmptyStringTestData => new() { "", "  ", "\t", "\r", "\n" };
        public static TheoryData<string> NonLetterTestData => new() { "AA7", "6BB", "CC9", "D-D", "-E", "F+" };
        public static TheoryData<string> LowerCaseValueTestData => new() { "a", "bc", "def" };
        public static TheoryData<string> ValidValueTestData => new() { "A", "BC", "DEF" };
        public static TheoryData<int> NotPositiveNonZeroIntegerTestData => new() { int.MinValue, -1, 0 };

        [Theory]
        [MemberData(nameof(NotPositiveNonZeroIntegerTestData))]
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
        [MemberData(nameof(EmptyStringTestData))]
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
        [MemberData(nameof(NonLetterTestData))]
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
        [MemberData(nameof(LowerCaseValueTestData))]
        public void ExplicitConversionFromString_Should_InitialiseValueToUpperCase(string value) =>
            ((ColumnName)value).Value.Should().BeUpperCased();

        [Theory]
        [MemberData(nameof(ValidValueTestData))]
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
        [MemberData(nameof(EmptyStringTestData))]
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
        [MemberData(nameof(NonLetterTestData))]
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
        [MemberData(nameof(LowerCaseValueTestData))]
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
        [MemberData(nameof(ValidValueTestData))]
        public void ImplicitConversionToString_Should_ConvertColumnNameToString(string value)
        {
            // Arrange
            var columnName = new ColumnName(value);
            
            // Act
            string actual = columnName;
            
            // Assert
            actual.Should().Be(value);
        }

        [Theory]
        [InlineData("A", "a")]
        [InlineData("BC", "BC")]
        [InlineData("DEF", "dEf")]
        [InlineData("GHI", "GhI")]
        public void EqualityToString_Should_BeTrueRegardlessOfCasing(string columnNameValue, string valueToCompare)
        {
            // Arrange
            var columnName = new ColumnName(columnNameValue);

            // Act, Assert
            using (new AssertionScope())
            {
                (columnName == valueToCompare).Should().BeTrue();
                (columnName != valueToCompare).Should().BeFalse();
                (valueToCompare == columnName).Should().BeTrue();
                (valueToCompare != columnName).Should().BeFalse();
            }
        }

        [Fact]
        public void EqualityToString_Should_BeFalseWhenColumnNameIsNullAndStringHasValue()
        {
            // Arrange
            ColumnName columnName = null!;
            
            // Act, Assert
            using (new AssertionScope())
            {
                (columnName == "ABC").Should().BeFalse();
                (columnName != "ABC").Should().BeTrue();
                ("ABC" == columnName).Should().BeFalse();
                ("ABC" != columnName).Should().BeTrue();
            }
        }

        [Theory]
        [MemberData(nameof(EmptyStringTestData))]
        [InlineData(null)]
        public void EqualityToString_Should_BeTrueWhenColumnIsNullAndStringDoesNotHaveValue(string value)
        {
            // Arrange
            ColumnName columnName = null!;
            
            // Act, Assert
            using (new AssertionScope())
            {
                (columnName == value).Should().BeTrue();
                (columnName != value).Should().BeFalse();
                (value == columnName).Should().BeTrue();
                (value != columnName).Should().BeFalse();
            }
        }

        [Theory]
        [MemberData(nameof(NotPositiveNonZeroIntegerTestData))]
        public void EqualityToInt_Should_BeTrueWhenColumnIsNullAndIntIsNotPositiveNonZero(int value)
        {
            // Arrange
            ColumnName columnName = null!;
            
            // Act, Assert
            using (new AssertionScope())
            {
                (columnName == value).Should().BeTrue();
                (columnName != value).Should().BeFalse();
                (value == columnName).Should().BeTrue();
                (value != columnName).Should().BeFalse();
            }
        }

        [Theory]
        [MemberData(nameof(EmptyStringTestData))]
        [InlineData(null)]
        public void TryParseString_Should_ReturnFalseAndNoColumnNameWhenStringCannotBeParsed(string input)
        {
            // Arrange, Act
            var parsed = ColumnName.TryParse(input, out var columnName);

            // Assert
            parsed.Should().BeFalse();
            columnName.Should().BeNull();
        }

        [Theory]
        [MemberData(nameof(ValidValueTestData))]
        public void TryParseString_ShouldReturnTrueAndColumnNameWhenStringIsProperlyFormatted(string input)
        {
            // Arrange, Act
            var parsed = ColumnName.TryParse(input, out var columnName);
            
            // Assert
            parsed.Should().BeTrue();
            columnName!.Value.Should().Be(input);
        }
        
        [Theory]
        [MemberData(nameof(NotPositiveNonZeroIntegerTestData))]
        public void TryParseInt_Should_ReturnFalseAndNoColumnNameWhenIntIsNotPositiveNonZero(int value)
        {
            // Arrange, Act
            var parsed = ColumnName.TryParse(value, out var columnName);

            // Assert
            parsed.Should().BeFalse();
            columnName.Should().BeNull();
        }
        
        [Theory]
        [InlineData(1, "A")]
        [InlineData(28, "AB")]
        [InlineData(1500, "BER")]
        public void TryParseInt_Should_ReturnTrueAndNoColumnNameWhenIntIsPositiveNonZero
            (int value, string expectedValue)
        {
            // Arrange, Act
            var parsed = ColumnName.TryParse(value, out var columnName);

            // Assert
            parsed.Should().BeTrue();
            columnName!.Value.Should().Be(expectedValue);
        }
    }
}