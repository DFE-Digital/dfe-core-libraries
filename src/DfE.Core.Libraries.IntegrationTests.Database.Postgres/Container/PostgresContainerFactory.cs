using DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Extensions;
using DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Registry;
using DfE.Core.Libraries.IntegrationTests.Database.Postgres.Container.Options;
using DotNet.Testcontainers.Containers;
using Microsoft.Extensions.Options;
using Testcontainers.PostgreSql;

namespace DfE.Core.Libraries.IntegrationTests.Database.Postgres.Container;

internal sealed class PostgresContainerFactory : IContainerFactory
{
    private readonly PostgresContainerOptions _options;
    private readonly IContainerNetworkRegistry _networkRegistry;

    public PostgresContainerFactory(
        PostgresContainerOptions options,
        IContainerNetworkRegistry networkRegistry)
    {
        _options = options;
        _networkRegistry = networkRegistry;
    }

    public async Task<IContainer> CreateAsync(string key, CancellationToken cancellationToken)
    {
        PostgreSqlBuilder builder =
            new PostgreSqlBuilder(_options.Container!.Image)
                .WithDatabase(_options.Database!.Name)
                .WithUsername(_options.Database.Username)
                .WithPassword(_options.Database.Password)
                .WithContainerOptions<
                    PostgreSqlBuilder,
                    PostgreSqlContainer,
                    PostgreSqlConfiguration>(_options.Container);

        builder =
            await builder
                .WithContainerNetworksAsync<
                    PostgreSqlBuilder,
                    PostgreSqlContainer,
                    PostgreSqlConfiguration>(
                        _options.Container.Networks,
                        _networkRegistry);

        return builder.Build();
    }
}
