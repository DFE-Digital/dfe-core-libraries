using System.Linq.Expressions;
using DfE.Core.Libraries.DesignPatterns.Specification;
using DfE.Core.Libraries.DesignPatterns.UnitTests.Specification.TestDoubles;

namespace DfE.Core.Libraries.DesignPatterns.UnitTests.Specification;

public sealed class AndSpecificationTests
{
    [Fact]
    public void Constructor_ShouldThrow_WhenLeftIsNull()
    {
        // Arrange
        ISpecification<int> right = SpecificationTestDoubles.Fake<int>(true);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(
            () => new AndSpecification<int>(null!, right)
        );
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenRightIsNull()
    {
        // Arrange
        ISpecification<int> left = SpecificationTestDoubles.Fake<int>(true);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(
            () => new AndSpecification<int>(left, null!)
        );
    }

    [Theory]
    [InlineData(true, true, true)]
    [InlineData(true, false, false)]
    [InlineData(false, true, false)]
    [InlineData(false, false, false)]
    public void IsSatisfiedBy_ShouldReturnExpected(bool leftVal, bool rightVal, bool expected)
    {
        // Arrange
        ISpecification<int> left = SpecificationTestDoubles.Fake<int>(leftVal);
        ISpecification<int> right = SpecificationTestDoubles.Fake<int>(rightVal);
        AndSpecification<int> spec = new(left, right);

        // Act
        bool result = spec.IsSatisfiedBy(42);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(true, true, true)]
    [InlineData(true, false, false)]
    [InlineData(false, true, false)]
    [InlineData(false, false, false)]
    public void ToExpression_ShouldMatchIsSatisfiedBy(bool leftVal, bool rightVal, bool expected)
    {
        // Arrange
        ISpecification<int> left = SpecificationTestDoubles.Fake<int>(leftVal);
        ISpecification<int> right = SpecificationTestDoubles.Fake<int>(rightVal);
        AndSpecification<int> spec = new(left, right);

        // Act
        Expression<Func<int, bool>> expr = spec.ToExpression();
        bool compiled = expr.Compile()(42);

        // Assert
        Assert.Equal(expected, compiled);
    }
}
