using DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Registry;
using Microsoft.Extensions.DependencyInjection;

namespace DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Extensions;

public static class ContainerBuilderHandlerExtensions
{
    public static IServiceCollection AddContainerBuilderHandler<TBuilder>(
        this IServiceCollection services,
        string key,
        IContainerBuilderHandler<TBuilder> handler) where TBuilder : class
    {
        services.AddSingleton(
            new ContainerBuilderHandlerRegistration<TBuilder>(
                key,
                handler));

        return services;
    }
}
