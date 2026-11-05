namespace DfE.Core.Libraries.DesignPatterns.ChainOfResponsibility;

internal sealed class ChainOfResponsibilityEvaluator<TIn> : IEvaluator<TIn>
{
    private readonly BaseEvaluationHandler<TIn> _rootHandler;

    public ChainOfResponsibilityEvaluator(BaseEvaluationHandler<TIn> rootHandler)
    {
        _rootHandler = rootHandler ?? throw new ArgumentNullException(nameof(rootHandler));
    }

    public ValueTask EvaluateAsync(TIn request, CancellationToken cancellationToken = default)
    {
        return _rootHandler.HandleAsync(request, cancellationToken);
    }
}
