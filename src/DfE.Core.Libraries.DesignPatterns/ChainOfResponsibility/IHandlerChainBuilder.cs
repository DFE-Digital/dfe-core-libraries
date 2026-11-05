namespace DfE.Core.Libraries.DesignPatterns.ChainOfResponsibility;

public interface IHandlerChainBuilder<TInput, THandler>
    where THandler : IEvaluationHandler<TInput>
{
    IHandlerChainBuilder<TInput, THandler> ChainNext(THandler handler);
    IHandlerChain<TInput, THandler> Build();
}
