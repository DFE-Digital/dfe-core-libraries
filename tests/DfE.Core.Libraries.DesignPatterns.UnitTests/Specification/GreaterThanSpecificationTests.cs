using System.Linq.Expressions;
using DfE.Core.Libraries.DesignPatterns.Specification;

namespace DfE.Core.Libraries.DesignPatterns.UnitTests.Specification;

public sealed class GreaterThanSpecificationTests
{
    [Fact]
    public void Constructor_WhenPropertyIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        Expression<Func<TestProjection<int>, int?>> property = null!;

        // Act
        Func<GreaterThanSpecification<TestProjection<int>, int>> construct =
            () => new(property, 10);

        // Assert
        Assert.Throws<ArgumentNullException>(construct);
    }

    [Theory]
    [InlineData(11, true)]
    [InlineData(10, false)]
    [InlineData(9, false)]
    [InlineData(null, false)]
    public void IsSatisfiedBy_WhenPropertyIsInteger_ReturnsExpectedResult(
        int? value,
        bool expected)
    {
        // Arrange
        GreaterThanSpecification<TestProjection<int>, int> specification =
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
    [InlineData(-1, false)]
    [InlineData(0, false)]
    [InlineData(1, true)]
    public void IsSatisfiedBy_WhenPropertyIsDateTime_ReturnsExpectedResult(
        int daysFromComparator,
        bool expected)
    {
        // Arrange
        DateTime comparator = new(2026, 11, 5);

        GreaterThanSpecification<TestProjection<DateTime>, DateTime> specification =
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
