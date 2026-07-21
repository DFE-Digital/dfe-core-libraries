using System.Linq.Expressions;
using DfE.Core.Libraries.DesignPatterns.Specification.Expressions;

namespace DfE.Core.Libraries.DesignPatterns.Specification;

internal sealed class XOrSpecification<TInput> : ISpecification<TInput>
{
    public XOrSpecification(ISpecification<TInput> left, ISpecification<TInput> right)
    {
        Left = left ?? throw new ArgumentNullException(nameof(left));
        Right = right ?? throw new ArgumentNullException(nameof(right));
    }

    public ISpecification<TInput> Left { get; }
    public ISpecification<TInput> Right { get; }

    public bool IsSatisfiedBy(TInput input)
        => Left.IsSatisfiedBy(input) ^ Right.IsSatisfiedBy(input);

    public Expression<Func<TInput, bool>> ToExpression()
    {
        Expression<Func<TInput, bool>> left = Left.ToExpression();
        Expression<Func<TInput, bool>> right = Right.ToExpression();

        // Unify parameter once
        ParameterExpression param = Expression.Parameter(typeof(TInput), "x");
        Expression leftBody = new ReplaceParameterVisitor(left.Parameters[0], param).Visit(left.Body)!;
        Expression rightBody = new ReplaceParameterVisitor(right.Parameters[0], param).Visit(right.Body)!;

        // Build (left && !right) || (!left && right)
        Expression notRight = Expression.Not(rightBody);
        Expression notLeft = Expression.Not(leftBody);

        Expression leftAndNotRight = Expression.AndAlso(leftBody, notRight);
        Expression notLeftAndRight = Expression.AndAlso(notLeft, rightBody);

        Expression body = Expression.OrElse(leftAndNotRight, notLeftAndRight);
        return Expression.Lambda<Func<TInput, bool>>(body, param);
    }
}
