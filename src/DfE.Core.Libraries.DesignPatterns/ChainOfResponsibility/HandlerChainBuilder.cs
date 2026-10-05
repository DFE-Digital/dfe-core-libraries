namespace DfE.Core.Libraries.DesignPatterns.ChainOfResponsibility;

public sealed class HandlerChainBuilder<THandlerInput, THandler> : IHandlerChainBuilder<THandlerInput, THandler>
    where THandler : class, IEvaluationHandler<THandlerInput>
{
    private readonly List<THandler> _items;
    public HandlerChainBuilder()
    {
        _items = [];
    }

    public IHandlerChainBuilder<THandlerInput, THandler> ChainNext(THandler handler)
    {
        if (handler == null)
        {
            throw new ArgumentNullException(nameof(handler));
        }
        _items.Add(handler);
        return this;
    }

    public IHandlerChain<THandlerInput, THandler> Build() => new HandlerChain<THandlerInput, THandler>(_items);
    public static HandlerChainBuilder<THandlerInput, THandler> Create() => new();
    public static HandlerChainBuilder<THandlerInput, THandler> Create(IEnumerable<THandler> handlers)
    {
        HandlerChainBuilder<THandlerInput, THandler> builder = new();

        handlers?.ToList().ForEach(handler => builder.ChainNext(handler));

        return builder;
    }
}
