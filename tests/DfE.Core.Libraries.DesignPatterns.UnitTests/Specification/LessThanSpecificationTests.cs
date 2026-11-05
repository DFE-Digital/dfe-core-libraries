using System.Linq.Expressions;
using DfE.Core.Libraries.DesignPatterns.Specification;

namespace DfE.Core.Libraries.DesignPatterns.UnitTests.Specification;

public sealed class LessThanSpecificationTests
{
    [Fact]
    public void Constructor_WhenPropertyIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        Expression<Func<TestProjection<int>, int?>> property = null!;

        // Act
        Func<LessThanSpecification<TestProjection<int>, int>> construct =
            () => new(property, 10);

        // Assert
        Assert.Throws<ArgumentNullException>(construct);
    }

    [Theory]
    [InlineData(11, false)]
    [InlineData(10, false)]
    [InlineData(9, true)]
    [InlineData(null, false)]
    public void IsSatisfiedBy_WhenPropertyIsInteger_ReturnsExpectedResult(
        int? value,
        bool expected)
    {
        // Arrange
        LessThanSpecification<TestProjection<int>, int> specification =
            new(projection => projection.Value, 10);

        TestProjection<int> input = new()
        {
            Value = value
        };

        // Act
        bool result = specification.IsSatisfiedBy(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(-1, true)]
    [InlineData(0, false)]
    [InlineData(1, false)]
    public void IsSatisfiedBy_WhenPropertyIsDateTime_ReturnsExpectedResult(
        int daysFromComparator,
        bool expected)
    {
        // Arrange
        DateTime comparator = new(2026, 11, 5);

        LessThanSpecification<TestProjection<DateTime>, DateTime> specification =
            new(projection => projection.Value, comparator);

        TestProjection<DateTime> input = new()
        {
            Value = comparator.AddDays(daysFromComparator)
        };

        // Act
        bool result = specification.IsSatisfiedBy(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void IsSatisfiedBy_WhenDateTimePropertyIsNull_ReturnsFalse()
    {
        // Arrange
        DateTime comparator = new(2026, 11, 5);

        LessThanSpecification<TestProjection<DateTime>, DateTime> specification =
            new(projection => projection.Value, comparator);

        TestProjection<DateTime> input = new()
        {
            Value = null
        };

        // Act
        bool result = specification.IsSatisfiedBy(input);

        // Assert
        Assert.False(result);
    }

    [Theory]
    [InlineData(-1, true)]
    [InlineData(0, false)]
    [InlineData(1, false)]
    public void IsSatisfiedBy_WhenPropertyIsDecimal_ReturnsExpectedResult(
        int difference,
        bool expected)
    {
        // Arrange
        decimal comparator = 10m;

        LessThanSpecification<TestProjection<decimal>, decimal> specification =
            new(projection => projection.Value, comparator);

        TestProjection<decimal> input = new()
        {
            Value = comparator + difference
        };

        // Act
        bool result = specification.IsSatisfiedBy(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void IsSatisfiedBy_WhenPropertyImplementsIComparableWithoutLessThanOperator_ThrowsInvalidOperationException()
    {
        // Arrange
        ComparableValue comparator = new(10);

        TestProjection<ComparableValue> input = new()
        {
            Value = new ComparableValue(9)
        };

        // Act
        Action act = () =>
        {
            LessThanSpecification<TestProjection<ComparableValue>, ComparableValue> specification =
                new(projection => projection.Value, comparator);

            specification.IsSatisfiedBy(input);
        };

        // Assert
        Assert.Throws<InvalidOperationException>(act);
    }

    // TProperty : struct to exercise CompareTo
    private sealed class TestProjection<TProperty>
        where TProperty : struct
    {
        public TProperty? Value { get; init; }
    }

    private readonly record struct ComparableValue(int Value)
        : IComparable<ComparableValue>
    {
        public int CompareTo(ComparableValue other)
        {
            return Value.CompareTo(other.Value);
        }
    }
}
