namespace DfE.Core.Libraries.DesignPatterns.ChainOfResponsibility;

public interface IHandlerChain<TRequest, out THandler>
    where THandler : IEvaluationHandler<TRequest>
{
    IReadOnlyList<THandler> Handlers { get; }
}
