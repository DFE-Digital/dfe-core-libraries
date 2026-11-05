namespace DfE.Core.Libraries.DesignPatterns.ChainOfResponsibility;

internal sealed class HandlerChain<TContext, THandler> : IHandlerChain<TContext, THandler>
    where THandler : IEvaluationHandler<TContext>
{
    public HandlerChain(IEnumerable<THandler> handlers)
    {
        if (handlers == null)
        {
            throw new ArgumentNullException(nameof(handlers));
        }

        if (!handlers.Any())
        {
            throw new ArgumentException("No handlers registered");
        }

        Handlers = handlers.ToList().AsReadOnly();
    }

    public IReadOnlyList<THandler> Handlers { get; }
}
