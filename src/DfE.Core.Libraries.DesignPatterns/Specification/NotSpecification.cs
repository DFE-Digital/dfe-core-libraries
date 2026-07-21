using System.Linq.Expressions;
using DfE.Core.Libraries.DesignPatterns.Specification.Expressions;

namespace DfE.Core.Libraries.DesignPatterns.Specification;

internal sealed class NotSpecification<T> : ISpecification<T>
{
    public NotSpecification(ISpecification<T> inner)
    {
        Inner = inner ?? throw new ArgumentNullException(nameof(inner));
    }

    public ISpecification<T> Inner { get; }

    public bool IsSatisfiedBy(T input) => !Inner.IsSatisfiedBy(input);

    public Expression<Func<T, bool>> ToExpression()
    {
        Expression<Func<T, bool>> expr = Inner.ToExpression();
        ParameterExpression param = Expression.Parameter(typeof(T), "x");
        Expression innerBody = new ReplaceParameterVisitor(expr.Parameters[0], param).Visit(expr.Body)!;
        Expression body = Expression.Not(innerBody);
        return Expression.Lambda<Func<T, bool>>(body, param);
    }
}
