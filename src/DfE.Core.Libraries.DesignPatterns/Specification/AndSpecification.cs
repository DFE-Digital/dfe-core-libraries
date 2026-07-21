using System.Linq.Expressions;
using DfE.Core.Libraries.DesignPatterns.Specification.Expressions;

namespace DfE.Core.Libraries.DesignPatterns.Specification;

internal sealed class AndSpecification<T> : ISpecification<T>
{
    public AndSpecification(ISpecification<T> left, ISpecification<T> right)
    {
        Left = left ?? throw new ArgumentNullException(nameof(left));
        Right = right ?? throw new ArgumentNullException(nameof(right));
    }

    public bool IsSatisfiedBy(T input) => Left.IsSatisfiedBy(input) && Right.IsSatisfiedBy(input);

    public ISpecification<T> Left { get; }
    public ISpecification<T> Right { get; }

    public Expression<Func<T, bool>> ToExpression() => ExpressionRebinder.Rebind(Left.ToExpression(), Right.ToExpression(), Expression.AndAlso);
}
