using System.Linq.Expressions;

namespace DfE.Core.Libraries.DesignPatterns.Specification;

public interface ISpecification<TInput>
{
    Expression<Func<TInput, bool>> ToExpression();
    bool IsSatisfiedBy(TInput input);
}
