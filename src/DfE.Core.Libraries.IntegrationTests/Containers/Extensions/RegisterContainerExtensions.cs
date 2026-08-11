using DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Options.Container;
using DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Registry;
using DotNet.Testcontainers.Builders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Extensions;

public static class RegisterContainerExtensions
{
    public static IServiceCollection AddContainerRegistry(
        this IServiceCollection services)
    {
        services.AddSingleton<DefaultContainerFactory>();
        services.AddSingleton<IContainerFactoryRegistry, ContainerFactoryRegistry>();
        services.TryAddSingleton<IContainerRegistry, ContainerRegistry>();
        services.TryAddSingleton<IContainerNetworkRegistry, ContainerNetworkRegistry>();

        return services;
    }

    public static IServiceCollection AddContainer(
        this IServiceCollection services,
        string key,
        IConfiguration configuration,
        Action<IServiceCollection>? configureHandlers = null)
    {
        services.AddContainerRegistry();

        services
            .AddOptions<ContainerOptions>(key)
            .Bind(configuration.GetRequiredSection(nameof(ContainerOptions)))
            .ValidateOnStart();

        services.TryAddEnumerable(
            ServiceDescriptor.Singleton<
                IValidateOptions<ContainerOptions>,
                ContainerOptionsValidator>());

        configureHandlers?.Invoke(services);

        services.AddSingleton<IContainerFactory>(
            sp =>
            {
                IEnumerable<IContainerBuilderHandler<ContainerBuilder>> handlers =
                    sp.GetServices<IContainerBuilderHandler<ContainerBuilder>>();

                return new DefaultContainerFactory(
                    sp.GetRequiredService<IOptionsMonitor<ContainerOptions>>(),
                    sp.GetRequiredService<IContainerNetworkRegistry>(),
                    handlers.ToArray());
            });

        return services;
    }
}
