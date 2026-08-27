using DfE.Core.Libraries.DesignPatterns.ChainOfResponsibility.ExecutionStrategy;
using DfE.Core.Libraries.DesignPatterns.ChainOfResponsibility.Options;

namespace DfE.Core.Libraries.DesignPatterns.ChainOfResponsibility;

// NOTE TODO Separate evaluators for different execution modes ChainOfResponsibilityEvaluator - replaces the strategy

public sealed class Evaluator<TRequest, THandler> : IEvaluator<TRequest> where THandler : IEvaluationHandler<TRequest>
{
    private readonly IExecutionStrategy<TRequest, THandler> _executionStrategy;
    private readonly IHandlerChain<TRequest, THandler> _handlerChain;

    public Evaluator(
        IHandlerChain<TRequest, THandler> handlerChain, IExecutionStrategy<TRequest, THandler> executionStrategy)
    {
        _handlerChain = handlerChain ?? throw new ArgumentNullException(nameof(handlerChain));
        _executionStrategy = executionStrategy ?? throw new ArgumentNullException(nameof(executionStrategy));
    }

    public async ValueTask EvaluateAsync(
        TRequest input, EvaluationOptions? options = null, CancellationToken ctx = default)
    {
        options ??= new();

        await _executionStrategy.ExecuteAsync(input, _handlerChain, ctx);
    }
}
