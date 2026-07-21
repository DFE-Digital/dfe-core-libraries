using System.Linq.Expressions;
using DfE.Core.Libraries.DesignPatterns.Specification;
using DfE.Core.Libraries.DesignPatterns.UnitTests.Specification.TestDoubles;

namespace DfE.Core.Libraries.DesignPatterns.UnitTests.Specification;

public sealed class NotSpecificationTests
{
    [Fact]
    public void Constructor_ShouldThrow_WhenInnerIsNull()
    {
        Assert.Throws<ArgumentNullException>(
            () => new NotSpecification<int>(null!)
        );
    }

    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public void IsSatisfiedBy_ShouldReturnExpected(bool innerVal, bool expected)
    {
        ISpecification<int> inner = SpecificationTestDoubles.Fake<int>(innerVal);
        NotSpecification<int> spec = new(inner);

        bool result = spec.IsSatisfiedBy(42);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public void ToExpression_ShouldMatchIsSatisfiedBy(bool innerVal, bool expected)
    {
        ISpecification<int> inner = SpecificationTestDoubles.Fake<int>(innerVal);
        NotSpecification<int> spec = new(inner);

        Expression<Func<int, bool>> expr = spec.ToExpression();
        bool compiled = expr.Compile()(42);

        Assert.Equal(expected, compiled);
    }
}
