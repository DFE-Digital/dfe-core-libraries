namespace DfE.Core.Libraries.DesignPatterns.Specification.Extensions;

public static class SpecificationExtensions
{
    public static ISpecification<T> And<T>(this ISpecification<T> left, ISpecification<T> right)
        => new AndSpecification<T>(left, right);

    public static ISpecification<T> Or<T>(this ISpecification<T> left, ISpecification<T> right)
        => new OrSpecification<T>(left, right);

    public static ISpecification<T> Xor<T>(this ISpecification<T> left, ISpecification<T> right)
        => new XOrSpecification<T>(left, right);

    public static ISpecification<T> Not<T>(this ISpecification<T> inner)
        => new NotSpecification<T>(inner);
}
