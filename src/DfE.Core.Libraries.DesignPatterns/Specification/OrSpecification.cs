using System.Linq.Expressions;
using DfE.Core.Libraries.DesignPatterns.Specification.Expressions;

namespace DfE.Core.Libraries.DesignPatterns.Specification;

internal sealed class OrSpecification<TInput> : ISpecification<TInput>
{
    public OrSpecification(ISpecification<TInput> left, ISpecification<TInput> right)
    {
        Left = left ?? throw new ArgumentNullException(nameof(left));
        Right = right ?? throw new ArgumentNullException(nameof(right));
    }

    public ISpecification<TInput> Left { get; }
    public ISpecification<TInput> Right { get; }

    public bool IsSatisfiedBy(TInput input)
        => Left.IsSatisfiedBy(input) || Right.IsSatisfiedBy(input);

    public Expression<Func<TInput, bool>> ToExpression()
        => ExpressionRebinder.Rebind(Left.ToExpression(), Right.ToExpression(), Expression.OrElse);
}
