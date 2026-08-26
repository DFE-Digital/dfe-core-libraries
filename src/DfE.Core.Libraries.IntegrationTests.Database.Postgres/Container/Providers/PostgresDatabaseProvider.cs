using DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Registry;
using DfE.Core.Libraries.IntegrationTests.Database.Abstractions;
using DfE.Core.Libraries.IntegrationTests.Database.Postgres.Container.Options;
using DotNet.Testcontainers.Containers;
using Microsoft.Extensions.Options;

namespace DfE.Core.Libraries.IntegrationTests.Database.Postgres.Container.Providers;

internal sealed class PostgresDatabaseProvider : IPostgresDatabaseProvider
{
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
}
