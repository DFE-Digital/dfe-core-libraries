using System.Linq.Expressions;
using DfE.Core.Libraries.DesignPatterns.Specification;

namespace DfE.Core.Libraries.DesignPatterns.UnitTests.Specification.TestDoubles;

internal static class SpecificationTestDoubles
{
    internal static ISpecification<T> Fake<T>(bool result)
        => new FakeSpecification<T>(() => result);

    internal static ISpecification<T> Fake<T>(Func<bool> resultFactory)
        => new FakeSpecification<T>(resultFactory);

    private sealed class FakeSpecification<T> : ISpecification<T>
    {
        private readonly Func<bool> _result;

        public FakeSpecification(Func<bool> resultFactory)
            => _result = resultFactory ?? throw new ArgumentNullException(nameof(resultFactory));

        public bool IsSatisfiedBy(T input) => _result();

        public Expression<Func<T, bool>> ToExpression()
        {
            // Evaluate once so the expression captures a constant, not a method call.
            bool value = _result();
            return _ => value;
        }
    }
}
