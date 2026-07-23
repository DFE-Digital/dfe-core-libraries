namespace DfE.Core.Libraries.DesignPatterns.ChainOfResponsibility;


public abstract class BaseEvaluationHandler<TRequest>
{
    private BaseEvaluationHandler<TRequest>? _next;

    public BaseEvaluationHandler<TRequest> ChainNext(BaseEvaluationHandler<TRequest> handler)
    {
        _next = handler ?? throw new ArgumentNullException(nameof(handler));
        return this;
    }

    public async ValueTask HandleAsync(TRequest request, CancellationToken cancellationToken = default)
    {
        if (CanHandle(request))
        {

            await HandleCoreAsync(request, cancellationToken);
            return;
        }

        if (_next is null)
        {
            throw new InvalidOperationException("No handler was able to process the request.");
        }

        await _next.HandleAsync(request, cancellationToken);

    }

    public abstract bool CanHandle(TRequest request);

    protected abstract ValueTask HandleCoreAsync(TRequest request, CancellationToken cancellationToken = default);
}
