using System.Linq.Expressions;

namespace DfE.Core.Libraries.DesignPatterns.Specification;

internal sealed class GreaterThanSpecification<TProjection, TProperty> : ComparisonSpecification<TProjection, TProperty>
    where TProperty : struct, IComparable<TProperty>
{
    public GreaterThanSpecification(
        Expression<Func<TProjection, TProperty?>> property,
        TProperty comparator)
        : base(property, comparator)
    {
    }

    protected override BinaryExpression CreateComparison(
        Expression property,
        Expression comparator)
    {
        return Expression.GreaterThan(property, comparator);
    }
}
