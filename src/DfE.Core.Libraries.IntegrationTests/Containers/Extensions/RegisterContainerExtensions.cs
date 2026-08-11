using DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Options;
using DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Registry;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Extensions;

public static class RegisterContainerExtensions
{
    public static IServiceCollection AddContainerRegistry(this IServiceCollection services)
    {
        services.TryAddSingleton<IContainerRegistry, ContainerRegistry>();
        return services;
    }

    public static IServiceCollection AddContainer(
        this IServiceCollection services,
        string key,
        IConfiguration configuration,
        Func<IServiceProvider, IEnumerable<IContainerBuilderHandler<ContainerBuilder>>>? handlersFactory = null)
    {
        services
            .AddOptions<ContainerOptions>(key)
            .Bind(configuration.GetRequiredSection(nameof(ContainerOptions)))
            .ValidateOnStart();

        services.TryAddEnumerable(
            ServiceDescriptor.Singleton<
                IValidateOptions<ContainerOptions>,
                ContainerOptionsValidator>());

        services.AddContainerRegistry();

        services.AddSingleton<IContainerRegistration>(
            (sp) =>
            {
                Func<IContainerRegistry, CancellationToken, Task<ContainerBuilder>> createBuilderContext =
                async (registry, ct) =>
                {
                    ContainerOptions options =
                        sp.GetRequiredService<IOptionsMonitor<ContainerOptions>>()
                            .Get(key);

                    ContainerBuilder builder =
                        new ContainerBuilder(options.Image)
                            .WithContainerOptions<
                                ContainerBuilder,
                                IContainer,
                                IContainerConfiguration>(options);

                    builder =
                        await builder
                            .WithContainerNetworksAsync<
                                ContainerBuilder,
                                IContainer,
                                IContainerConfiguration>(
                                    options.Networks,
                                    registry);

                    return builder;
                };

                return new ContainerRegistration<ContainerBuilder>(
                    key,
                    createBuilderContext,
                    static builder => builder.Build(),
                    handlersFactory?.Invoke(sp) ?? []);
            });

        return services;
    }
}
