using System;
using FluentAssertions;
using Xunit;

namespace Dgt.Minesweeper.Engine
{
    public class LocationTests
    {
        public static TheoryData<string, ColumnName, int> ValidLocationTestData => new()
        {
            { "A1", new ColumnName("A"), 1 },
            { "a1", new ColumnName("A"), 1 },
            { "H8", new ColumnName("H"), 8 },
            { "AA99", new ColumnName("AA"), 99 },
            { "ZZZ1000", new ColumnName("ZZZ"), 1000 }
        };

        public static TheoryData<string> InvalidLocationTestData => new() { "A", "1", "1A", "Nope!" };

        public static TheoryData<string> MissingValuesTestData => new() { null!, string.Empty, "\t", "\r\n", "   " };

        public static TheoryData<int> InvalidIndexTestData => new() { int.MinValue, -1, 0 };
        
        [Fact]
        public void Ctor_Should_ThrowWhenColumnNameStringIsNull()
        {
            // Arrange, Act
            Action act = () => _ = new Location((string)null!, 1);
            
            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithMessage("Value cannot be null.*")
                .WithMessage("*Value must be one or more letters, with no other characters e.g. 'AA'.*")
                .WithParameterName("columnName");
        }

        [Fact]
        public void Ctor_Should_ThrowWhenColumnNameRecordIsNull()
        {
            // Arrange, Act
            Action act = () => _ = new Location(null!, 1);
            
            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithMessage("Value cannot be null.*")
                .WithParameterName("columnName");
        }

        [Theory]
        [MemberData(nameof(InvalidIndexTestData))]
        public void Ctor_Should_ThrowWhenColumnNameIsNotPositiveNonZero(int columnIndex)
        {
            // Arrange, Act
            Action act = () => _ = new Location(columnIndex, 1);
            
            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>()
                .WithMessage("Value must be a positive, non-zero integer.*")
                .WithParameterName("columnIndex")
                .And.ActualValue.Should().Be(columnIndex);
        }

        [Theory]
        [MemberData(nameof(InvalidIndexTestData))]
        public void Ctor_Should_ThrowWhenRowIndexIsNotPositiveNonZero(int rowIndex)
        {
            // Arrange, Act
            Action act = () => _ = new Location("AA", rowIndex);
            
            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>()
                .WithMessage("Value must be a positive, non-zero integer.*")
                .WithParameterName("rowIndex")
                .And.ActualValue.Should().Be(rowIndex);
        }

        [Theory]
        [MemberData(nameof(ValidLocationTestData))]
        public void Parse_Should_ParseColumnNameAndRowIndexWhenInputIsProperlyFormatted
            (string input, ColumnName expectedColumnName, int expectedRowIndex)
        {
            // Arrange, Act
            var (columnName, rowIndex) = Location.Parse(input);
            
            // Assert
            columnName.Should().Be(expectedColumnName);
            rowIndex.Should().Be(expectedRowIndex);
        }

        [Theory]
        [InlineData("    A1", "A", 1)]
        [InlineData("A1     ", "A", 1)]
        [InlineData("A       1", "A", 1)]
        [InlineData("   A  1   ", "A", 1)]
        public void Parse_Should_IgnoreWhitespaceWhenParsingInput
            (string input, string expectedColumnName, int expectedRowIndex)
        {
            // Arrange, Act
            var (columnName, rowIndex) = Location.Parse(input);
            
            // Assert
            columnName.Should().Be((ColumnName)expectedColumnName);
            rowIndex.Should().Be(expectedRowIndex);
        }

        [Theory]
        [MemberData(nameof(InvalidLocationTestData))]
        [MemberData(nameof(MissingValuesTestData))]
        public void Parse_Should_ThrowWhenInputIsNotProperlyFormatted(string input)
        {
            // Arrange, Act
            Action act = () => Location.Parse(input);

            // Assert
            act.Should().Throw<FormatException>()
                .WithMessage("*Input string was not in a correct format.*")
                .WithMessage("*Locations must be of the form 'A1'.*")
                .WithMessage($"*Actual value was '{input}'.*")
                .Where(ex => ex.Data.Contains("input") && Equals(ex.Data["input"], input));
        }

        [Theory]
        [MemberData(nameof(ValidLocationTestData))]
        public void TryParse_Should_ReturnTrueAndLocationWhenInputIsProperlyFormatted
            (string input, ColumnName expectedColumnName, int expectedRowIndex)
        {
            // Arrange, Act
            var parsed = Location.TryParse(input, out var location);
            
            // Assert
            parsed.Should().BeTrue();
            location.Should().NotBeNull();
            location!.ColumnName.Should().Be(expectedColumnName);
            location.RowIndex.Should().Be(expectedRowIndex);
        }

        [Theory]
        [MemberData(nameof(InvalidLocationTestData))]
        [MemberData(nameof(MissingValuesTestData))]
        public void TryParse_Should_ReturnFalseAndNoLocationWhenInputIsNotProperlyFormatted(string input)
        {
            // Arrange, Act
            var parsed = Location.TryParse(input, out var location);
            
            // Assert
            parsed.Should().BeFalse();
            location.Should().BeNull();
        }

        [Theory]
        [InlineData("A", 1, "A1")]
        [InlineData("ZZ", 1, "ZZ1")]
        [InlineData("A", 99, "A99")]
        public void ImplicitConversionToString_Should_FormatAsColumnNameAndRowIndex(string columnName, int rowIndex, string expected)
        {
            // Arrange
            var location = new Location((ColumnName)columnName, rowIndex);

            // Act
            string actual = location;
            
            // Assert
            actual.Should().Be(expected);
        }

        [Theory]
        [MemberData(nameof(ValidLocationTestData))]
        public void ExplicitConversionFromString_Should_ParseColumnNameAndRowIndexWhenInputIsProperlyFormatted
            (string input, ColumnName expectedColumnName, int expectedRowIndex)
        {
            // Arrange, Act
            var location = (Location)input;
            
            // Assert
            location.ColumnName.Should().Be(expectedColumnName);
            location.RowIndex.Should().Be(expectedRowIndex);
        }

        [Theory]
        [MemberData(nameof(InvalidLocationTestData))]
        [MemberData(nameof(MissingValuesTestData))]
        public void ExplicitConversionFromString_Should_ThrowWhenInputIsNotProperlyFormatted(string input)
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
        public void EqualityToTupleOfStringAndInt_Should_BeTrueWhenColumnNameAndRowIndexAreEqual()
        {
            (new Location("A", 1) == ("A", 1)).Should().BeTrue();
            (("A", 1) == new Location("A", 1)).Should().BeTrue();
            
            (new Location("A", 1) != ("A", 1)).Should().BeFalse();
            (("A", 1) != new Location("A", 1)).Should().BeFalse();
        }
        
        // We test the inequality operator here because we're forced to implement both operators together
        [Fact]
        public void EqualityToTupleOfStringAndInt_Should_CompareColumnNameAndInt()
        {
            (new Location("A", 1) == (new ColumnName("A"), 1)).Should().BeTrue();
            ((new ColumnName("A"), 1) == new Location("A", 1)).Should().BeTrue();
            
            (new Location("A", 1) != (new ColumnName("A"), 1)).Should().BeFalse();
            ((new ColumnName("A"), 1) != new Location("A", 1)).Should().BeFalse();
        }

        // We test the inequality operator here because we're forced to implement both operators together
        [Fact]
        public void EqualityToTupleOfStringAndInt_Should_BeFalseWhenColumnNameDiffers()
        {
            (new Location("A", 1) == ("B", 1)).Should().BeFalse();
            (("B", 1) == new Location("A", 1)).Should().BeFalse();
            
            (new Location("A", 1) != ("B", 1)).Should().BeTrue();
            (("B", 1) != new Location("A", 1)).Should().BeTrue();
        }
        
        // We test the inequality operator here because we're forced to implement both operators together
        [Fact]
        public void EqualityToTupleOfStringAndInt_Should_BeFalseWhenRowIndexDiffers()
        {
            (new Location("A", 1) == ("A", 2)).Should().BeFalse();
            (("A", 2) == new Location("A", 1)).Should().BeFalse();
            
            (new Location("A", 1) != ("A", 2)).Should().BeTrue();
            (("A", 2) != new Location("A", 1)).Should().BeTrue();
        }

        // We test the inequality operator here because we're forced to implement both operators together
        [Fact]
        public void EqualityToTupleOfStringAndInt_Should_BeFalseWhenLocationIsNull()
        {
            ((Location)null! == ("A", 1)).Should().BeFalse();
            (("A", 1) == (Location)null!).Should().BeFalse();

            ((Location)null! != ("A", 1)).Should().BeTrue();
            (("A", 1) != (Location)null!).Should().BeTrue();
        }

        [Fact]
        public void IsAdjacentTo_Should_BeFalseWhenLocationIsNull()
        {
            new Location("A", 1).IsAdjacentTo(null!).Should().BeFalse();
        }
        
        [Fact]
        public void IsAdjacentTo_Should_BeFalseWhenLocationsAreEqual()
        {
            new Location("A", 1).IsAdjacentTo(new Location("A", 1)).Should().BeFalse();
        }
        
        [Theory]
        // The column is either the same, immediately to the left, or immediately to the right, but the row is not
        [InlineData("A4")]
        [InlineData("B4")]
        [InlineData("C4")]
        // The row is either the same, immediately to the left, or immediately to the right, but the column is not
        [InlineData("D1")]
        [InlineData("D2")]
        [InlineData("D3")]
        public void IsAdjacentTo_Should_BeFalseWhenCellIsNotAdjacentColumnOrRow(string location)
        {
            var left = Location.Parse("B2");
            var right = Location.Parse(location);

            left.IsAdjacentTo(right).Should().BeFalse();
        }
        
        [Theory]
        [InlineData("A1", "B2")]
        [InlineData("A2", "B2")]
        [InlineData("A3", "B2")]
        [InlineData("B1", "B2")]
        [InlineData("B3", "B2")]
        [InlineData("C1", "B2")]
        [InlineData("C2", "B2")]
        [InlineData("C3", "B2")]
        [InlineData("AA9", "Z10")]
        [InlineData("BD15", "BE14")]
        public void IsAdjacentTo_Should_BeTrueWhenLocationIsInSameOrAdjacentColumnAndRow(string left, string right)
        {
            var leftLocation = Location.Parse(left);
            var rightLocation = Location.Parse(right);

            leftLocation.IsAdjacentTo(rightLocation).Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(ValidLocationTestData))]
        public void DeconstructIntoColumnNameAndInt_Should_DeconstructColumnNameAndRowIndex
            (string location, ColumnName expectedColumnName, int expectedRowIndex)
        {
            // Arrange
            var toDeconstruct = Location.Parse(location);
            
            // Act
            var (columnName, rowIndex) = toDeconstruct;
            
            // Assert
            columnName.Should().Be(expectedColumnName);
            rowIndex.Should().Be(expectedRowIndex);
        }

        [Fact]
        public void Subtract_ShouldReturnDefaultTupleWhenWhenEitherOperandIsNull()
        {
            // Arrange
            var notNullLocation = Location.Parse("A1");
            var nullLocation = (Location)null!;

            // Act, Assert
            (notNullLocation - nullLocation).Should().Be((0, 0));
            (nullLocation - notNullLocation).Should().Be((0, 0));
            (nullLocation - nullLocation).Should().Be((0, 0));
        }

        [Theory]
        [InlineData("A1", "A1", 0, 0)]
        [InlineData("B1", "A1", 1, 0)]
        [InlineData("A1", "B1", -1, 0)]
        [InlineData("A2", "A1", 0, 1)]
        [InlineData("A1", "A2", 0, -1)]
        [InlineData("E5", "C3", 2, 2)]
        public void Subtract_Should_ReturnColumnDifferenceAndRowDifference
            (string left, string right, int expectedColumnDifference, int expectedRowDifference)
        {
            // Arrange
            var leftLocation = Location.Parse(left);
            var rightLocation = Location.Parse(right);
            
            // Act
            var (columnDifference, rowDifference) = leftLocation - rightLocation;
            
            // Assert
            columnDifference.Should().Be(expectedColumnDifference);
            rowDifference.Should().Be(expectedRowDifference);
        }
    }
}