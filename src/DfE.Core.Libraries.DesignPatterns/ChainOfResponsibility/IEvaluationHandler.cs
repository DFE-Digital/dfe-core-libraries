namespace DfE.Core.Libraries.DesignPatterns.ChainOfResponsibility;

public interface IEvaluationHandler<in TIn>
{
    ValueTask HandleAsync(TIn input, CancellationToken ctx = default);
}
