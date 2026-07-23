using Microsoft.Extensions.DependencyInjection;

namespace DfE.Core.Libraries.DesignPatterns.ChainOfResponsibility.Extensions;


public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddChainedHandlers<TRequest>(
        this IServiceCollection services)
    {
        services.AddScoped<IEvaluator<TRequest>>(sp =>
        {
            // Resolve registered handlers through DI order
            List<BaseEvaluationHandler<TRequest>> handlers =
                [.. sp.GetServices<BaseEvaluationHandler<TRequest>>()];

            if (handlers.Count == 0)
            {
                throw new InvalidOperationException(
                    $"No handlers registered for '{typeof(TRequest).Name}'.");
            }

            for (int index = 0; index < handlers.Count - 1; index++)
            {
                handlers[index].SetNext(handlers[index + 1]);
            }

            BaseEvaluationHandler<TRequest> rootHandler = handlers[0];

            return new ChainOfResponsibilityEvaluator<TRequest>(
                rootHandler);
        });

        return services;
    }
}
