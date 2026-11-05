using System.Linq.Expressions;

namespace DfE.Core.Libraries.DesignPatterns.Specification.Extensions;

/// <summary>
/// Provides operations for creating and composing specifications.
/// </summary>
/// <remarks>
/// Logical operations compose existing specifications while preserving the
/// specification abstraction until <see cref="ISpecification{T}.ToExpression"/>
/// is required.
///
/// Comparison operations create a specification from a strongly typed property
/// expression and a comparator. The property's type is used when constructing
/// the comparison, but is intentionally not exposed by the resulting
/// <see cref="ISpecification{T}"/>, which represents a predicate over the
/// projection as a whole.
/// </remarks>
public static class SpecificationExtensions
{
    /// <summary>
    /// Creates a specification that is satisfied when both specifications are satisfied.
    /// </summary>
    public static ISpecification<T> And<T>(
        this ISpecification<T> left,
        ISpecification<T> right)
        => new AndSpecification<T>(left, right);

    /// <summary>
    /// Creates a specification that is satisfied when either specification is satisfied.
    /// </summary>
    public static ISpecification<T> Or<T>(
        this ISpecification<T> left,
        ISpecification<T> right)
        => new OrSpecification<T>(left, right);

    /// <summary>
    /// Creates a specification that is satisfied when exactly one specification is satisfied.
    /// </summary>
    public static ISpecification<T> Xor<T>(
        this ISpecification<T> left,
        ISpecification<T> right)
        => new XOrSpecification<T>(left, right);

    /// <summary>
    /// Creates a specification that negates the supplied specification.
    /// </summary>
    public static ISpecification<T> Not<T>(
        this ISpecification<T> inner)
        => new NotSpecification<T>(inner);

    /// <summary>
    /// Creates a specification that is satisfied when the selected property
    /// is greater than the supplied comparator.
    /// </summary>
    public static ISpecification<TProjection> GreaterThan<TProjection, TProperty>(
        this Expression<Func<TProjection, TProperty?>> property,
        TProperty comparator)
        where TProperty : struct, IComparable<TProperty>
    {
        return new GreaterThanSpecification<TProjection, TProperty>(
            property,
            comparator);
    }

    /// <summary>
    /// Creates a specification that is satisfied when the selected property
    /// is greater than or equal to the supplied comparator.
    /// </summary>
    public static ISpecification<TProjection> GreaterThanOrEqualTo<TProjection, TProperty>(
        this Expression<Func<TProjection, TProperty?>> property,
        TProperty comparator)
        where TProperty : struct, IComparable<TProperty>
    {
        return new GreaterThanOrEqualSpecification<TProjection, TProperty>(
            property,
            comparator);
    }

    /// <summary>
    /// Creates a specification that is satisfied when the selected property
    /// is less than the supplied comparator.
    /// </summary>
    public static ISpecification<TProjection> LessThan<TProjection, TProperty>(
        this Expression<Func<TProjection, TProperty?>> property,
        TProperty comparator)
        where TProperty : struct, IComparable<TProperty>
    {
        return new LessThanSpecification<TProjection, TProperty>(
            property,
            comparator);
    }

    /// <summary>
    /// Creates a specification that is satisfied when the selected property
    /// is less than or equal to the supplied comparator.
    /// </summary>
    public static ISpecification<TProjection> LessThanOrEqualTo<TProjection, TProperty>(
        this Expression<Func<TProjection, TProperty?>> property,
        TProperty comparator)
        where TProperty : struct, IComparable<TProperty>
    {
        return new LessThanOrEqualSpecification<TProjection, TProperty>(
            property,
            comparator);
    }
}
