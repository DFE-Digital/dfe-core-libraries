using DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Extensions;
using DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Registry;
using DfE.Core.Libraries.IntegrationTests.Database.Postgres.Container.Options;
using DfE.Core.Libraries.IntegrationTests.Database.Postgres.Container.Provider;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace DfE.Core.Libraries.IntegrationTests.Database.Postgres.Container.Extensions;

public static class RegisterPostgresServiceCollectionExtensions
{
    public static IServiceCollection AddPostgres(
        this IServiceCollection services,
        IConfiguration configuration,
        string key = "postgres")
    {
        // Shared ContainerRegistry activation
        services.AddContainerRegistry();

        services
            .AddOptions<PostgresContainerOptions>(key)
            .Bind(configuration.GetRequiredSection(nameof(PostgresContainerOptions)))
            .ValidateOnStart();

        services.TryAddEnumerable(
            ServiceDescriptor.Singleton<
                IValidateOptions<PostgresContainerOptions>,
                PostgresContainerOptionsValidator>());

        services.AddSingleton<ContainerFactoryRegistration>(
            sp =>
            {
                return new
                    ContainerFactoryRegistration(
                        key,
                        sp.GetRequiredService<PostgresContainerFactory>());
            });

        services.AddScoped<IPostgresDatabaseProvider, PostgresDatabaseProvider>();

        return services;

    }
}
