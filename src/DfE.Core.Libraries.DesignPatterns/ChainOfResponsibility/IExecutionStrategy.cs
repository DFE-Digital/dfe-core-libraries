namespace DfE.Core.Libraries.DesignPatterns.ChainOfResponsibility;

public interface IExecutionStrategy<TIn, THandler> where THandler : IEvaluationHandler<TIn>
{
    ValueTask ExecuteAsync(
        TIn input,
        IHandlerChain<TIn, THandler> chain,
        CancellationToken token = default
    );
}
