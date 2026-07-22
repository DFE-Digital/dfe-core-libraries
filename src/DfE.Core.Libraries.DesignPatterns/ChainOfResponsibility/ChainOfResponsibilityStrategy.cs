namespace DfE.Core.Libraries.DesignPatterns.ChainOfResponsibility;

public sealed class ChainOfResponsibilityStrategy<TIn> : IExecutionStrategy<TIn, IChainOfResponsibilityEvaluationHandler<TIn>>
{
    public async ValueTask ExecuteAsync(
        TIn input,
        IHandlerChain<TIn, IChainOfResponsibilityEvaluationHandler<TIn>> chain,
        CancellationToken ctx = default)
    {
        foreach (IChainOfResponsibilityEvaluationHandler<TIn> item in chain.Handlers)
        {
            if (!item.CanHandle(input))
            {
                continue;
            }

            await item.HandleAsync(input);
            return;
        }

        throw new InvalidOperationException("No handlers able to handle");
    }
}
