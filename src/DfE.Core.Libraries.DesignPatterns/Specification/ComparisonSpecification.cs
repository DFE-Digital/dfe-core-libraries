using System.Linq.Expressions;

namespace DfE.Core.Libraries.DesignPatterns.Specification;

// IComparable<TProperty> provides a semantic constraint that TProperty supports ordering through CompareTo
// Expression construction additionally requires the selected comparison operator (> or <) to be defined for TProperty, which cannot be constrained.
internal abstract class ComparisonSpecification<TProjection, TProperty> : ISpecification<TProjection> where TProperty : struct, IComparable<TProperty>
{
    private readonly Expression<Func<TProjection, TProperty?>> _property;
    private readonly TProperty _comparator;

    protected ComparisonSpecification(
        Expression<Func<TProjection, TProperty?>> property,
        TProperty comparator)
    {
        _property = property ?? throw new ArgumentNullException(nameof(property));
        _comparator = comparator;
    }

    public bool IsSatisfiedBy(TProjection input)
    {
        return ToExpression().Compile().Invoke(input);
    }

    public Expression<Func<TProjection, bool>> ToExpression()
    {
        ParameterExpression parameter = _property.Parameters[0];

        Expression hasValue = Expression.Property(
            _property.Body,
            nameof(Nullable<>.HasValue));

        Expression value = Expression.Property(
            _property.Body,
            nameof(Nullable<>.Value));

        Expression comparator = Expression.Constant(
            _comparator,
            typeof(TProperty));

        BinaryExpression comparison = CreateComparison(
            value,
            comparator);

        Expression body = Expression.AndAlso(
            hasValue,
            comparison);

        return Expression.Lambda<Func<TProjection, bool>>(
            body,
            parameter);
    }

    protected abstract BinaryExpression CreateComparison(
        Expression property,
        Expression comparator);
}
