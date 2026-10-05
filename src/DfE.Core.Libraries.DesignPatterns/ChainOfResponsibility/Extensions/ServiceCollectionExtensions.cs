using Microsoft.Extensions.DependencyInjection;

namespace DfE.Core.Libraries.DesignPatterns.ChainOfResponsibility.Extensions;


public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddChainedHandlers<TIn>(
        this IServiceCollection services,
        Action<IChainedEvaluationHandlerBuilder<TIn>> configure)
    {
        ChainedHandlerBuilder<TIn> builder = new();

        configure(builder);

        IReadOnlyList<ServiceDescriptor> registrations = builder.Registrations;

        foreach (ServiceDescriptor descriptor in registrations)
        {
            services.Add(descriptor);
        }

        services.AddScoped<IEvaluator<TIn>>(sp =>
        {
            // Resolve Handler concrete types
            List<BaseEvaluationHandler<TIn>> handlers =
                [.. registrations.Select((descriptor)
                        => (BaseEvaluationHandler<TIn>)sp.GetRequiredService(descriptor.ServiceType))];

            if (handlers.Count == 0)
            {
                throw new InvalidOperationException(
                    $"No handlers registered for {typeof(TIn).Name}.");
            }

            for (int index = 0; index < handlers.Count - 1; index++)
            {
                handlers[index].SetNext(handlers[index + 1]);
            }

            return new ChainOfResponsibilityEvaluator<TIn>(
                handlers[0]);
        });

        return services;
    }
}
