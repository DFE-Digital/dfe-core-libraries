using DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Options;
using DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Options.Extensions;
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
        IConfiguration configuration)
    {
        ContainerOptions options =
            configuration
            .GetRequiredSection(nameof(configuration))
            .Get<ContainerOptions>() ?? throw new ArgumentException($"{nameof(ContainerOptions)} does not exist in configuration");

        services.AddSingleton<IValidateOptions<ContainerOptions>, ContainerOptionsValidator>();

        services.AddContainerRegistry();

        services.AddSingleton<IContainerRegistration>(
            (sp) =>
            {
                sp.GetRequiredService<IValidateOptions<ContainerOptions>>()
                    .Validate(key, options)
                    .ThrowIfFailed<ValidateOptionsResult>(key);

                IReadOnlyCollection<IContainerBuilderHandler<ContainerBuilder>> handlers = GetHandlersFor(key, sp);

                Func<IContainerRegistry, CancellationToken, Task<ContainerBuilderContext<ContainerBuilder>>> createBuilderContext =
                    async (registry, ct) =>
                    {
                        ContainerBuilder builder =
                            new ContainerBuilder(options.Image)
                                .WithContainerOptions<ContainerBuilder, IContainer, IContainerConfiguration>(options);

                        builder =
                            await builder
                                .WithContainerNetworksAsync<ContainerBuilder, IContainer, IContainerConfiguration>(options.Networks, registry);

                        return new ContainerBuilderContext<ContainerBuilder>(
                            builder,
                            (builder) => builder.Build());
                    };

                return new ContainerRegistration<ContainerBuilder>(
                    key,
                    createBuilderContext,
                    handlers);
            });

        return services;
    }

    private static IReadOnlyCollection<IContainerBuilderHandler<ContainerBuilder>> GetHandlersFor(string key, IServiceProvider provider)
    {
        IReadOnlyCollection<IContainerBuilderHandler<ContainerBuilder>> handlers =
            provider.GetServices<ContainerBuilderHandlerRegistration<ContainerBuilder>>()
                .Where((builderHandlerRegistration) => builderHandlerRegistration.Key.Equals(key, StringComparison.OrdinalIgnoreCase))
                .Select(x => x.Handler)
                .ToArray();

        return handlers;
    }
}
