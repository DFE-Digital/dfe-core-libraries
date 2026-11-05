using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace DfE.Core.Libraries.DesignPatterns.Specification;

internal sealed class LessThanSpecification<TProjection, TProperty> : ComparisonSpecification<TProjection, TProperty>
    where TProperty : struct, IComparable<TProperty>
{
    public LessThanSpecification(
        Expression<Func<TProjection, TProperty?>> property,
        TProperty comparator)
        : base(property, comparator)
    {
    }

    protected override BinaryExpression CreateComparison(
        Expression property,
        Expression comparator)
    {
        return Expression.LessThan(property, comparator);
    }
}
