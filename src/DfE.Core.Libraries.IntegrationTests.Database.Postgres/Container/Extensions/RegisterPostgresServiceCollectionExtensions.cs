using DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Extensions;
using DfE.Core.Libraries.IntegrationTests.Database.Postgres.Container.Options;
using DfE.Core.Libraries.IntegrationTests.Database.Postgres.Container.Providers;
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
        // Shared for IValidator<ContainerOptions>
        services.AddContainerOptionsValidation();

        // PostgresOptions and validator
        services
            .AddOptions<PostgresContainerOptions>(key)
            .Bind(configuration.GetRequiredSection(nameof(PostgresContainerOptions)))
            .ValidateOnStart();

        services.TryAddEnumerable(
            ServiceDescriptor.Singleton<
                IValidateOptions<PostgresContainerOptions>,
                PostgresContainerOptionsValidator>());

        services.AddScoped<PostgresContainerFactory>();

        services.AddScoped<ContainerFactoryRegistration>(
            (sp) => new
                    ContainerFactoryRegistration(
                        key,
                        sp.GetRequiredService<PostgresContainerFactory>()));

        services.AddScoped<IPostgresDatabaseProvider, PostgresDatabaseProvider>();

        return services;
    }
}
