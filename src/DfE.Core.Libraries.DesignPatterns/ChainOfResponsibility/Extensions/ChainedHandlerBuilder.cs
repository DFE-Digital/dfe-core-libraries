using Microsoft.Extensions.DependencyInjection;

namespace DfE.Core.Libraries.DesignPatterns.ChainOfResponsibility.Extensions;

internal sealed class ChainedHandlerBuilder<TIn> : IChainedHandlerBuilder<TIn>
{
    private readonly List<HandlerRegistration> _registrations = [];

    public IReadOnlyList<HandlerRegistration> Registrations => _registrations;

    public IChainedHandlerBuilder<TIn> AddScoped<THandler>()
        where THandler : BaseEvaluationHandler<TIn>
    {
        _registrations.Add(
            new HandlerRegistration(
                typeof(THandler),
                ServiceLifetime.Scoped));

        return this;
    }

    public IChainedHandlerBuilder<TIn> AddTransient<THandler>()
        where THandler : BaseEvaluationHandler<TIn>
    {
        _registrations.Add(
            new HandlerRegistration(
                typeof(THandler),
                ServiceLifetime.Transient));

        return this;
    }

    public IChainedHandlerBuilder<TIn> AddSingleton<THandler>()
        where THandler : BaseEvaluationHandler<TIn>
    {
        _registrations.Add(
            new HandlerRegistration(
                typeof(THandler),
                ServiceLifetime.Singleton));

        return this;
    }
}

internal sealed record HandlerRegistration
{
    public HandlerRegistration(Type handlerType, ServiceLifetime lifetime)
    {
        HandlerType = handlerType ?? throw new ArgumentNullException(nameof(handlerType));
        Lifetime = lifetime;
    }

    public Type HandlerType { get; }
    public ServiceLifetime Lifetime { get; }
}
