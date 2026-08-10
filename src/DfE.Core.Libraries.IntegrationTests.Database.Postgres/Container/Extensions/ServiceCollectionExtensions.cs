using DfE.Core.Libraries.IntegrationTests.Abstractions.Containers;
using DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Extensions;
using DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Options.Extensions;
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

        // Shared ContainerRegistry activation
        services.AddContainerRegistry();

        PostgresContainerOptions options =
            configuration
                .GetRequiredSection(nameof(PostgresContainerOptions))
                .Get<PostgresContainerOptions>() ??
                    throw new ArgumentException($"{nameof(PostgresContainerOptions)} does not exist in configuration");

        services.TryAddSingleton<IValidateOptions<PostgresContainerOptions>, PostgresContainerOptionsValidator>();

        services.AddSingleton<ContainerRegistration>(
            (sp) =>
            {
                sp.GetRequiredService<IValidateOptions<PostgresContainerOptions>>()
                    .Validate(key, options)
                    .ThrowIfFailed<PostgresContainerOptions>(key);

                ContainerRegistration registration = new(
                    key,
                    async (registry, ct) =>
                    {
                        PostgreSqlBuilder builder =
                            new PostgreSqlBuilder(options.Container!.Image)
                                .WithDatabase(options.Database!.Name)
                                .WithUsername(options.Database.Username)
                                .WithPassword(options.Database.Password)
                                .WithContainerOptions<PostgreSqlBuilder, PostgreSqlContainer, PostgreSqlConfiguration>(options.Container);

                        builder =
                            await builder
                                .WithContainerNetworksAsync<PostgreSqlBuilder, PostgreSqlContainer, PostgreSqlConfiguration>(options.Container.Networks, registry);

                        return builder.Build();
                    });

                return registration;
            });


        // Named option for DatabaseOptions used runtime connection string in provider
        services.AddOptions<PostgresDatabaseOptions>(key)
            .Configure(opt =>
            {
                opt.Name = options.Database!.Name;
                opt.Username = options.Database.Username;
                opt.Password = options.Database.Password;
            });

        services.AddScoped<IPostgresDatabaseProvider, PostgresDatabaseProvider>();

        return services;
    }
}
