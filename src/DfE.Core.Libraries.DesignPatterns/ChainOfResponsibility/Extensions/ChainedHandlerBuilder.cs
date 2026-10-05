using Microsoft.Extensions.DependencyInjection;

namespace DfE.Core.Libraries.DesignPatterns.ChainOfResponsibility.Extensions;

internal sealed class ChainedHandlerBuilder<TIn> : IChainedEvaluationHandlerBuilder<TIn>
{
    private readonly List<ServiceDescriptor> _registrations = [];

    public IReadOnlyList<ServiceDescriptor> Registrations => _registrations;

    public IChainedEvaluationHandlerBuilder<TIn> AddScoped<THandler>()
        where THandler : BaseEvaluationHandler<TIn>
    {
        _registrations.Add(
            new ServiceDescriptor(
                typeof(THandler),
                typeof(THandler),
                ServiceLifetime.Scoped));

        return this;
    }

    public IChainedEvaluationHandlerBuilder<TIn> AddScoped<THandler>(
        Func<IServiceProvider, THandler> factory)
        where THandler : BaseEvaluationHandler<TIn>
    {
        _registrations.Add(
            ServiceDescriptor.Scoped(
                typeof(THandler),
                sp => factory(sp)));

        return this;
    }


    public IChainedEvaluationHandlerBuilder<TIn> AddTransient<THandler>()
        where THandler : BaseEvaluationHandler<TIn>
    {
        _registrations.Add(
            new ServiceDescriptor(
                typeof(THandler),
                typeof(THandler),
                ServiceLifetime.Transient));

        return this;
    }

    public IChainedEvaluationHandlerBuilder<TIn> AddTransient<THandler>(
    Func<IServiceProvider, THandler> factory)
    where THandler : BaseEvaluationHandler<TIn>
    {
        _registrations.Add(
            ServiceDescriptor.Transient(
                typeof(THandler),
                sp => factory(sp)));

        return this;
    }

    public IChainedEvaluationHandlerBuilder<TIn> AddSingleton<THandler>()
        where THandler : BaseEvaluationHandler<TIn>
    {
        _registrations.Add(
            new ServiceDescriptor(
                typeof(THandler),
                ServiceLifetime.Singleton));

        return this;
    }

    public IChainedEvaluationHandlerBuilder<TIn> AddSingleton<THandler>(
        Func<IServiceProvider, THandler> factory)
        where THandler : BaseEvaluationHandler<TIn>
    {
        _registrations.Add(
            new ServiceDescriptor(
                typeof(THandler),
                typeof(THandler),
                ServiceLifetime.Singleton));

        return this;
    }
}
