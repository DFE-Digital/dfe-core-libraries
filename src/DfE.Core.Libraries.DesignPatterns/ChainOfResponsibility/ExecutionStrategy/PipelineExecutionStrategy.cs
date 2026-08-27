namespace DfE.Core.Libraries.DesignPatterns.ChainOfResponsibility;

internal sealed class PipelineExecutionStrategy<TIn> : IExecutionStrategy<TIn, IEvaluationHandler<TIn>>
{
    public async ValueTask ExecuteAsync(TIn input, IHandlerChain<TIn, IEvaluationHandler<TIn>> chain, CancellationToken token = default)
    {
        foreach (IEvaluationHandler<TIn> item in chain.Handlers)
        {
            await item.HandleAsync(input);
        }
    }
}
