using System.Linq.Expressions;

namespace DfE.Core.Libraries.DesignPatterns.Specification.Expressions;

internal static class ExpressionRebinder
{
    public static Expression<Func<T, bool>> Rebind<T>(
        Expression<Func<T, bool>> left,
        Expression<Func<T, bool>> right,
        Func<Expression, Expression, BinaryExpression> merge)
    {
        // Create a new, shared parameter for both bodies
        ParameterExpression parameter = Expression.Parameter(typeof(T), "x");

        // Replace each expression's parameter with the shared one
        Expression leftBody = new ReplaceParameterVisitor(left.Parameters[0], parameter).Visit(left.Body)!;
        Expression rightBody = new ReplaceParameterVisitor(right.Parameters[0], parameter).Visit(right.Body)!;

        // Merge them (AndAlso, OrElse, custom, etc.)
        BinaryExpression body = merge(leftBody, rightBody);

        // Return a new lambda with the unified parameter
        return Expression.Lambda<Func<T, bool>>(body, parameter);
    }
}
