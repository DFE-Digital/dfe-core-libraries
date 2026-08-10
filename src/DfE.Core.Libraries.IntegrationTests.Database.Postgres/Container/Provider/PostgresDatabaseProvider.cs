using DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Registry;
using DfE.Core.Libraries.IntegrationTests.Database.Abstractions;
using DfE.Core.Libraries.IntegrationTests.Database.Postgres.Container.Options;
using DotNet.Testcontainers.Containers;
using Microsoft.Extensions.Options;

namespace DfE.Core.Libraries.IntegrationTests.Database.Postgres.Container.Provider;

internal sealed class PostgresDatabaseProvider
    : IPostgresDatabaseProvider
{
    private readonly IContainerRegistry _containerRegistry;
    private readonly IOptionsMonitor<PostgresDatabaseOptions> _dbOptions;

    public PostgresDatabaseProvider(
        IContainerRegistry containerRegistry,
        IOptionsMonitor<PostgresDatabaseOptions> dbOptions)
    {
        _containerRegistry = containerRegistry;
        _dbOptions = dbOptions;
    }

    public async Task<IDatabase> GetDatabaseAsync(
        string key,
        CancellationToken cancellationToken = default)
    {
        IContainer container =
            await _containerRegistry.GetOrCreateContainerAsync(
                key,
                cancellationToken);

        PostgresDatabaseOptions options = _dbOptions.Get(key);

        PostgresContainerDatabase database = new(options, new(container));

        await database.StartAsync(cancellationToken);

        return database;
    }
}

