using DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Extensions;
using DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Registry;
using DfE.Core.Libraries.IntegrationTests.Database.Postgres.Container.Options;
using DotNet.Testcontainers.Containers;
using Microsoft.Extensions.Options;
using Testcontainers.PostgreSql;

namespace DfE.Core.Libraries.IntegrationTests.Database.Postgres.Container;

internal sealed class PostgresContainerFactory : IContainerFactory
{
    private readonly IOptionsMonitor<PostgresContainerOptions> _options;
    private readonly IContainerNetworkRegistry _networkRegistry;

    public PostgresContainerFactory(
        IOptionsMonitor<PostgresContainerOptions> options,
        IContainerNetworkRegistry networkRegistry)
    {
        _options = options;
        _networkRegistry = networkRegistry;
    }

    public async Task<IContainer> CreateAsync(string key, CancellationToken cancellationToken)
    {
        PostgresContainerOptions containerOptions = _options.Get(key);

        PostgreSqlBuilder builder =
            new PostgreSqlBuilder(containerOptions.Container!.Image)
                .WithDatabase(containerOptions.Database!.Name)
                .WithUsername(containerOptions.Database.Username)
                .WithPassword(containerOptions.Database.Password)
                .WithContainerOptions<
                    PostgreSqlBuilder,
                    PostgreSqlContainer,
                    PostgreSqlConfiguration>(containerOptions.Container);

        builder =
            await builder
                .WithContainerNetworksAsync<
                    PostgreSqlBuilder,
                    PostgreSqlContainer,
                    PostgreSqlConfiguration>(
                        containerOptions.Container.Networks,
                        _networkRegistry);

        return builder.Build();
    }
}
