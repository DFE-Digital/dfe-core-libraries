using DfE.Core.Libraries.DesignPatterns.ChainOfResponsibility.Options;

namespace DfE.Core.Libraries.DesignPatterns.ChainOfResponsibility;

public interface IEvaluator<in TIn>
{
    ValueTask EvaluateAsync(TIn input, EvaluationOptions? options = null, CancellationToken ctx = default);
}
