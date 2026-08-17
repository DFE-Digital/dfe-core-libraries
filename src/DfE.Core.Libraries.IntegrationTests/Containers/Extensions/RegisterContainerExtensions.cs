using DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Options.Container;
using DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Registry;
using DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Registry.BuilderHandler;
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
        services.TryAddScoped<IContainerRegistry, ContainerRegistry>();
        services.TryAddScoped<IContainerNetworkRegistry, ContainerNetworkRegistry>();

        return services;
    }

    public static IServiceCollection AddContainer(
        this IServiceCollection services,
        string key,
        IConfiguration configuration)
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

        services.AddScoped<DefaultContainerFactory>();

        services.AddScoped<ContainerFactoryRegistration>(
            sp => new(
                    key,
                    sp.GetRequiredService<DefaultContainerFactory>()));

        // Default if no registration from client for HandlerRegistry...
        services.TryAddScoped<Dictionary<string, Func<IReadOnlyCollection<IConfigureContainerBuilderHandler<ContainerBuilder>>>>>((sp) => []);

        return services;
    }
}
