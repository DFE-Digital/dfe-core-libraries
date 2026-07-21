using System.Linq.Expressions;
using DfE.Core.Libraries.DesignPatterns.Specification;
using DfE.Core.Libraries.DesignPatterns.UnitTests.Specification.TestDoubles;

namespace DfE.Core.Libraries.DesignPatterns.UnitTests.Specification;

public sealed class OrSpecificationTests
{
    [Fact]
    public void Constructor_ShouldThrow_WhenLeftIsNull()
    {
        ISpecification<int> right = SpecificationTestDoubles.Fake<int>(true);

        Assert.Throws<ArgumentNullException>(
            () => new OrSpecification<int>(null!, right)
        );
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenRightIsNull()
    {
        ISpecification<int> left = SpecificationTestDoubles.Fake<int>(true);

        Assert.Throws<ArgumentNullException>(
            () => new OrSpecification<int>(left, null!)
        );
    }

    [Theory]
    [InlineData(true, true, true)]
    [InlineData(true, false, true)]
    [InlineData(false, true, true)]
    [InlineData(false, false, false)]
    public void IsSatisfiedBy_ShouldReturnExpected(bool leftVal, bool rightVal, bool expected)
    {
        ISpecification<int> left = SpecificationTestDoubles.Fake<int>(leftVal);
        ISpecification<int> right = SpecificationTestDoubles.Fake<int>(rightVal);
        OrSpecification<int> spec = new(left, right);

        bool result = spec.IsSatisfiedBy(42);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(true, true, true)]
    [InlineData(true, false, true)]
    [InlineData(false, true, true)]
    [InlineData(false, false, false)]
    public void ToExpression_ShouldMatchIsSatisfiedBy(bool leftVal, bool rightVal, bool expected)
    {
        ISpecification<int> left = SpecificationTestDoubles.Fake<int>(leftVal);
        ISpecification<int> right = SpecificationTestDoubles.Fake<int>(rightVal);
        OrSpecification<int> spec = new(left, right);

        Expression<Func<int, bool>> expr = spec.ToExpression();
        bool compiled = expr.Compile()(42);

        Assert.Equal(expected, compiled);
    }
}
