using DfE.Core.Libraries.IntegrationTests.Abstractions.Containers;
using DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Extensions;
using DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Options;
using DotNet.Testcontainers.Containers;
using Testcontainers.PostgreSql;

namespace DfE.Core.Libraries.IntegrationTests.Database.Postgres.Container;

internal sealed class PostgresContainerFactory : IContainerFactory
{
    private readonly PostgresDatabaseOptions _dbOptions;
    private readonly ContainerOptions _containerOptions;
    private readonly IContainerRegistry _containerRegistry;

    public PostgresContainerFactory(
        PostgresDatabaseOptions dbOptions,
        ContainerOptions containerOptions,
        IContainerRegistry containerRegistry)
    {
        _dbOptions = dbOptions;
        _containerOptions = containerOptions;
        _containerRegistry = containerRegistry;
    }

    public async Task<IContainer> Create()
    {
        IEnumerable<PortMapping> portMappings = _containerOptions.PortMappings ?? [];

        if (!portMappings.Any(t => t.ContainerPort == PostgreSqlBuilder.PostgreSqlPort))
        {
            portMappings =
            [
                ..portMappings,
                new PortMapping
                {
                    PublicPort = null,
                    ContainerPort = PostgreSqlBuilder.PostgreSqlPort
                }
            ];
        }

        // Important builder is immuteable so each configuration will create a new instance with configuration applied
        PostgreSqlBuilder builder =
            new PostgreSqlBuilder(_containerOptions.Image)
                .WithDatabase(_dbOptions.Database)
                .WithUsername(_dbOptions.Username)
                .WithPassword(_dbOptions.Password)
                .WithExposedPorts<PostgreSqlBuilder, PostgreSqlContainer, PostgreSqlConfiguration>(portMappings)
                .WithStartupCommands<PostgreSqlBuilder, PostgreSqlContainer, PostgreSqlConfiguration>(_containerOptions.StartupArguments)
                // Add files that need to be copied into the container before it starts e.g. .sql files to be applied at startup
                .WithMountedResources<PostgreSqlBuilder, PostgreSqlContainer, PostgreSqlConfiguration>(_containerOptions.CopyResourcesIntoContainerBeforeInit)
                // forces fresh container state - no mounted volume reuse
                .WithCleanUp(true);

        if (_containerOptions.Networks != null && _containerOptions.Networks.Any())
        {
            builder = await builder.WithNetworksAsync<PostgreSqlBuilder, PostgreSqlContainer, PostgreSqlConfiguration>(_containerOptions.Networks, _containerRegistry);
        }

        if (!string.IsNullOrWhiteSpace(_containerOptions.ContainerName))
        {
            builder = builder.WithName(_containerOptions.ContainerName);
        }

        // container-container networking
        if (!string.IsNullOrEmpty(_containerOptions.HostName))
        {
            builder = builder.WithNetworkAliases(_containerOptions.HostName);
        }

        return builder.Build();
    }
}
