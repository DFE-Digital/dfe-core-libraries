namespace DfE.Core.Libraries.DesignPatterns.ChainOfResponsibility;

public interface IEvaluator<in TRequest>
{
    ValueTask EvaluateAsync(
        TRequest request,
        CancellationToken cancellationToken = default);
}
