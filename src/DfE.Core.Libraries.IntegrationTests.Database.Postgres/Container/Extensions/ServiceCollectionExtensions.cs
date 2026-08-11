using DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Extensions;
using DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Registry;
using DfE.Core.Libraries.IntegrationTests.Database.Postgres.Container.Options;
using DfE.Core.Libraries.IntegrationTests.Database.Postgres.Container.Provider;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Testcontainers.PostgreSql;

namespace DfE.Core.Libraries.IntegrationTests.Database.Postgres.Container.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPostgres(
        this IServiceCollection services,
        IConfiguration configuration,
        string key = "postgres")
    {

        services
            .AddOptions<PostgresContainerOptions>(key)
            .Bind(configuration.GetRequiredSection(nameof(PostgresContainerOptions)))
            .ValidateOnStart();

        services.TryAddEnumerable(
            ServiceDescriptor.Singleton<
                IValidateOptions<PostgresContainerOptions>,
                PostgresContainerOptionsValidator>());

        // Shared ContainerRegistry activation
        services.AddContainerRegistry();

        services.AddSingleton<IContainerRegistration>(
            (sp) =>
            {
                PostgresContainerOptions options =
                    sp.GetRequiredService<IOptionsMonitor<PostgresContainerOptions>>()
                        .Get(key);

                return new ContainerRegistration<PostgreSqlBuilder>(
                    key,
                    async (registry, ct) =>
                    {
                        PostgreSqlBuilder builder =
                            new PostgreSqlBuilder(options.Container!.Image)
                                .WithDatabase(options.Database!.Name)
                                .WithUsername(options.Database.Username)
                                .WithPassword(options.Database.Password)
                                .WithContainerOptions<
                                    PostgreSqlBuilder,
                                    PostgreSqlContainer,
                                    PostgreSqlConfiguration>(
                                        options.Container);

                        builder =
                            await builder
                                .WithContainerNetworksAsync<
                                    PostgreSqlBuilder,
                                    PostgreSqlContainer,
                                    PostgreSqlConfiguration>(
                                        options.Container.Networks,
                                        registry);

                        return builder;
                    },
                    static builder => builder.Build(),
                    []);
            });

        services.AddScoped<IPostgresDatabaseProvider, PostgresDatabaseProvider>();

        return services;

    }
}
