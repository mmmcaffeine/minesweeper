using System;
using FluentAssertions;
using Xunit;

namespace Dgt.Minesweeper.Engine
{
    public class LocationTests
    {
        public static TheoryData<string, string, int> ValidLocationTestData => new()
        {
            { "A1", "A", 1 },
            { "a1", "A", 1 },
            { "H8", "H", 8 },
            { "AA99", "AA", 99 },
            { "ZZZ1000", "ZZZ", 1000 }
        };

        public static TheoryData<string> InvalidLocationTestData => new() { "A", "1", "1A", "Nope!" };

        [Fact]
        public void Ctor_Should_ThrowWhenColumnIsNull()
        {
            // Arrange, Act
            Action act = () => _ = new Location(null!, 1);
            
            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithMessage("Value cannot be null.*")
                .WithMessage("*Value must be one or more letters, with no other characters e.g. 'AA'.*")
                .WithParameterName("column");
        }

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData("\t")]
        [InlineData("\r")]
        [InlineData("\n")]
        public void Ctor_Should_ThrowWhenColumnIsEmptyOrWhitespace(string column)
        {
            // Arrange, Act
            Action act = () => _ = new Location(column, 1);
            
            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("Value cannot be whitespace or an empty string.*")
                .WithMessage("*Value must be one or more letters, with no other characters e.g. 'AA'.*")
                .WithParameterName("column")
                .Where(ex => ex.Data.Contains("column") && ex.Data["column"]!.Equals(column));
        }

        [Theory]
        [InlineData("99")]
        [InlineData("A1")]
        [InlineData("!H!")]
        public void Ctor_Should_ThrowWhenColumnIsNotOnlyCharacters(string column)
        {
            // Arrange, Act
            Action act = () => _ = new Location(column, 1);
            
            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("Input string was not in a correct format.*")
                .WithMessage("*Value must be one or more letters, with no other characters e.g. 'AA'.*")
                .WithParameterName("column")
                .Where(ex => ex.Data.Contains("column") && ex.Data["column"]!.Equals(column));
        }

        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(-1)]
        [InlineData(0)]
        public void Ctor_Should_ThrowWhenRowIsNotPositive(int row)
        {
            // Arrange, Act
            Action act = () => _ = new Location("AA", row);
            
            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>()
                .WithMessage("Value must be a positive, non-zero integer.*")
                .WithParameterName("row")
                .And.ActualValue.Should().Be(row);
        }

        [Theory]
        [MemberData(nameof(ValidLocationTestData))]
        public void Parse_Should_ParseColumnAndRowWhenInputIsProperlyFormatted
            (string input, string expectedColumn, int expectedRow)
        {
            // Arrange, Act
            var (column, row) = Location.Parse(input);
            
            // Assert
            column.Should().Be(expectedColumn);
            row.Should().Be(expectedRow);
        }

        [Theory]
        [InlineData("    A1", "A", 1)]
        [InlineData("A1     ", "A", 1)]
        [InlineData("A       1", "A", 1)]
        [InlineData("   A  1   ", "A", 1)]
        public void Parse_Should_IgnoreWhitespaceWhenParsingInput
            (string input, string expectedColumn, int expectedRow)
        {
            // Arrange, Act
            var (column, row) = Location.Parse(input);
            
            // Assert
            column.Should().Be(expectedColumn);
            row.Should().Be(expectedRow);
        }

        [Theory]
        [MemberData(nameof(InvalidLocationTestData))]
        public void Parse_Should_ThrowFormatExceptionWhenInputIsNotProperlyFormatted(string input)
        {
            // Arrange, Act
            Action act = () => Location.Parse(input);

            // Assert
            act.Should().Throw<FormatException>()
                .WithMessage("*Input string was not in a correct format.*")
                .WithMessage("*Locations must be of the form 'A1'.*")
                .WithMessage($"*Actual value was '{input}'.*")
                .Where(ex => ex.Data.Contains("input") && ex.Data["input"]!.Equals(input));
        }

        [Theory]
        [InlineData("A", 1, "A1")]
        [InlineData("ZZ", 1, "ZZ1")]
        [InlineData("A", 99, "A99")]
        public void ConvertToString_Should_FormatAsColumnAndRow(string column, int row, string expected)
        {
            // Arrange
            var location = new Location(column, row);

            // Act
            string actual = location;
            
            // Assert
            actual.Should().Be(expected);
        }

        [Theory]
        [MemberData(nameof(ValidLocationTestData))]
        public void ConvertFromString_Should_ParseColumnAndStringWhenInputIsProperlyFormatted
            (string input, string expectedColumn, int expectedRow)
        {
            // Arrange, Act
            var location = (Location)input;
            
            // Assert
            location.Column.Should().Be(expectedColumn);
            location.Row.Should().Be(expectedRow);
        }

        [Theory]
        [MemberData(nameof(InvalidLocationTestData))]
        public void ConvertFromString_Should_ThrowInvalidCastExceptionWhenInputIsNotProperlyFormatted(string input)
        {
            // Arrange, Act
            Action act = () => _ = (Location)input;
            
            // Assert
            act.Should().Throw<InvalidCastException>()
                .WithMessage("Specified cast is not valid.*")
                .WithInnerException<FormatException>();
        }

        // We test the inequality operator here because we're forced to implement both operators together
        [Fact]
        public void EqualityWithTuple_Should_BeTrueWhenColumnAndRowAreEqual()
        {
            (new Location("A", 1) == ("A", 1)).Should().BeTrue();
            (("A", 1) == new Location("A", 1)).Should().BeTrue();
            
            (new Location("A", 1) != ("A", 1)).Should().BeFalse();
            (("A", 1) != new Location("A", 1)).Should().BeFalse();
        }

        // We test the inequality operator here because we're forced to implement both operators together
        [Fact]
        public void EqualityWithTuple_Should_BeFalseWhenColumnDiffers()
        {
            (new Location("A", 1) == ("B", 1)).Should().BeFalse();
            (("B", 1) == new Location("A", 1)).Should().BeFalse();
            
            (new Location("A", 1) != ("B", 1)).Should().BeTrue();
            (("B", 1) != new Location("A", 1)).Should().BeTrue();
        }
        
        // We test the inequality operator here because we're forced to implement both operators together
        [Fact]
        public void EqualityWithTuple_Should_BeFalseWhenRowDiffers()
        {
            (new Location("A", 1) == ("A", 2)).Should().BeFalse();
            (("A", 2) == new Location("A", 1)).Should().BeFalse();
            
            (new Location("A", 1) != ("A", 2)).Should().BeTrue();
            (("A", 2) != new Location("A", 1)).Should().BeTrue();
        }

        // We test the inequality operator here because we're forced to implement both operators together
        [Fact]
        public void EqualityWithTuple_Should_BeFalseWhenLocationIsNull()
        {
            ((Location)null! == ("A", 1)).Should().BeFalse();
            (("A", 1) == (Location)null!).Should().BeFalse();

            ((Location)null! != ("A", 1)).Should().BeTrue();
            (("A", 1) != (Location)null!).Should().BeTrue();
        }
    }
}