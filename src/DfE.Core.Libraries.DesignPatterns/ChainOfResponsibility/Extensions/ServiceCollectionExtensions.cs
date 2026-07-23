using Microsoft.Extensions.DependencyInjection;

namespace DfE.Core.Libraries.DesignPatterns.ChainOfResponsibility.Extensions;


public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddChainedHandlers<TIn>(
        this IServiceCollection services,
        Action<IChainedHandlerBuilder<TIn>> configure)
    {
        ChainedHandlerBuilder<TIn> builder = new();

        configure(builder);

        foreach (HandlerRegistration registration in builder.Registrations)
        {
            services.Add(
                new ServiceDescriptor(
                    typeof(BaseEvaluationHandler<TIn>),
                    registration.HandlerType,
                    registration.Lifetime));
        }

        services.AddScoped<IEvaluator<TIn>>((sp) =>
        {
            List<BaseEvaluationHandler<TIn>> handlers =
                [.. sp.GetServices<BaseEvaluationHandler<TIn>>()];

            if (handlers.Count == 0)
            {
                throw new InvalidOperationException(
                    $"No handlers were registered for {typeof(TIn).Name}.");
            }

            for (int index = 0; index < handlers.Count - 1; index++)
            {
                handlers[index].SetNext(handlers[index + 1]);
            }

            BaseEvaluationHandler<TIn> rootHandler = handlers[0];

            return new ChainOfResponsibilityEvaluator<TIn>(rootHandler);
        });

        return services;
    }
}
