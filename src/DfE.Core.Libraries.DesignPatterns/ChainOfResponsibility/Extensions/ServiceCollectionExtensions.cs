using Microsoft.Extensions.DependencyInjection;

namespace DfE.Core.Libraries.DesignPatterns.ChainOfResponsibility.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHandlerChain<THandlerInput, THandler>(
        this IServiceCollection services,
        Action<IServiceProvider, IHandlerChainBuilder<THandlerInput, THandler>> configure)
        where THandler : class, IEvaluationHandler<THandlerInput>
    {
        services.AddScoped<IHandlerChain<THandlerInput, THandler>>((sp) =>
        {
            HandlerChainBuilder<THandlerInput, THandler> builder = HandlerChainBuilder<THandlerInput, THandler>.Create();

            configure(sp, builder);

            return builder.Build();
        });

        return services;
    }
}
