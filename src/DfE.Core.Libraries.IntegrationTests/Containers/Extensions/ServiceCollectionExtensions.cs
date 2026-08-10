using DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Options;
using DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Options.Extensions;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddContainerRegistry(this IServiceCollection services)
    {
        services.TryAddSingleton<IContainerRegistry, ContainerRegistry>();
        return services;
    }

    public static IServiceCollection AddContainer(
        this IServiceCollection services,
        IConfiguration configuration,
        string key)
    {
        ContainerOptions options =
            configuration
            .GetRequiredSection(nameof(configuration))
            .Get<ContainerOptions>() ?? throw new ArgumentException($"{nameof(ContainerOptions)} does not exist in configuration");

        services.AddSingleton<IValidateOptions<ContainerOptions>, ContainerOptionsValidator>();

        services.AddSingleton(
            (sp) =>
            {
                sp.GetRequiredService<IValidateOptions<ContainerOptions>>()
                    .Validate(key, options)
                    .ThrowIfFailed<ValidateOptionsResult>(key);

                ContainerRegistration registration = new(key, async (registry, ct) =>
                {
                    ContainerBuilder builder =
                        new ContainerBuilder(options.Image)
                            .WithContainerOptions<ContainerBuilder, IContainer, IContainerConfiguration>(options);

                    builder =
                        await builder
                            .WithContainerNetworksAsync<ContainerBuilder, IContainer, IContainerConfiguration>(options.Networks, registry);

                    return builder.Build();
                });

                return registration;
            });

        services.AddContainerRegistry();

        return services;
    }
}
