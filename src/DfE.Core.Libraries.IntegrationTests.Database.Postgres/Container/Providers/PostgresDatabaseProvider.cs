using DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Options.Container;
using DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Registry;
using DfE.Core.Libraries.IntegrationTests.Database.Abstractions;
using DfE.Core.Libraries.IntegrationTests.Database.Postgres.Container.Options;
using DotNet.Testcontainers.Containers;
using Microsoft.Extensions.Options;
using Npgsql;

namespace DfE.Core.Libraries.IntegrationTests.Database.Postgres.Container.Providers;

internal sealed class PostgresDatabaseProvider : IPostgresDatabaseProvider
{
    private const int PostgresPort = 5432;
    private readonly IContainerRegistry _containerRegistry;
    private readonly IOptionsMonitor<PostgresContainerOptions> _options;

    public PostgresDatabaseProvider(
        IContainerRegistry containerRegistry,
        IOptionsMonitor<PostgresContainerOptions> options)
    {
        _containerRegistry = containerRegistry;
        _options = options;
    }

    public async Task<IDatabase> GetDatabaseAsync(
        string key,
        CancellationToken cancellationToken = default)
    {
        IContainer container =
            await _containerRegistry.GetOrCreateContainerAsync(
                key,
                cancellationToken);

        PostgresContainerOptions options = _options.Get(key);

        PostgresContainerDatabase database = new(
            options.Database!,
            new(container));

        await database.StartAsync(cancellationToken);

        return database;
    }

    public async Task<string> GetConnectionStringAsync(
        string key,
        string? networkName = null,
        CancellationToken cancellationToken = default)
    {
        IContainer container =
            await _containerRegistry.GetOrCreateContainerAsync(
                key,
                cancellationToken);

        PostgresContainerOptions options = _options.Get(key);

        NpgsqlConnectionStringBuilder builder = new()
        {
            Database = options.Database!.Name,
            Username = options.Database.Username,
            Password = options.Database.Password
        };

        if (string.IsNullOrWhiteSpace(networkName))
        {
            builder.Host = container.Hostname;
            builder.Port = container.GetMappedPublicPort(PostgresPort);

            return builder.ConnectionString;
        }

        ContainerNetworkAttachment network =
            options.Container?.Networks.SingleOrDefault(x => x.Key == networkName) ??
                throw new ArgumentException($"Network '{networkName}' is not configured.", nameof(networkName));

        builder.Host =
            network.Aliases.FirstOrDefault() ??
                throw new InvalidOperationException($"No alias configured for network '{network.Key}'.");

        builder.Port = PostgresPort;

        return builder.ConnectionString;
    }
}
