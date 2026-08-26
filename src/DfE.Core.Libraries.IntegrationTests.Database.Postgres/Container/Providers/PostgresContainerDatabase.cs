using DfE.Core.Libraries.IntegrationTests.Database.Abstractions;
using DfE.Core.Libraries.IntegrationTests.Database.Postgres.Container.Options;
using DotNet.Testcontainers.Containers;
using Npgsql;

namespace DfE.Core.Libraries.IntegrationTests.Database.Postgres.Container.Providers;

internal sealed class PostgresContainerDatabase : IDatabase
{
    public const int PostgresPort = 5432;

    private readonly PostgresDatabaseOptions _databaseOptions;
    private readonly Lazy<IContainer> _container;
    private readonly SemaphoreSlim _startLock = new(1, 1);

    private bool _started;

    public PostgresContainerDatabase(
        PostgresDatabaseOptions databaseOptions,
        Lazy<IContainer> container)
    {
        ArgumentNullException.ThrowIfNull(databaseOptions);
        ArgumentNullException.ThrowIfNull(container);

        _databaseOptions = databaseOptions;
        _container = container;
    }

    public async ValueTask DisposeAsync()
    {
        if (_container is not null)
        {
            await _container.Value.DisposeAsync();
        }

        _startLock.Dispose();
    }

    public async Task StartAsync(CancellationToken ctx = default)
    {
        if (_started)
        {
            return;
        }

        await _startLock.WaitAsync(ctx);

        try
        {
            if (_started)
            {
                return;
            }

            await _container.Value.StartAsync(ctx);

            _started = true;
        }
        finally
        {
            _startLock.Release();
        }
    }

    public async Task ExecuteAsync(
        string sql,
        CancellationToken ctx = default)
    {
        string connectionString = BuildConnectionString();

        await using NpgsqlConnection connection =
            new(connectionString);

        await connection.OpenAsync(ctx);

        await using NpgsqlCommand command =
            new(sql, connection);

        await command.ExecuteNonQueryAsync(ctx);
    }

    private string BuildConnectionString()
    {
        EnsureDatabaseStarted();

        return new NpgsqlConnectionStringBuilder
        {
            Host = _container.Value.Hostname,
            Port = GetPublicContainerPort(_container.Value),
            Database = _databaseOptions.Name,
            Username = _databaseOptions.Username,
            Password = _databaseOptions.Password
        }.ConnectionString;
    }

    private static ushort GetPublicContainerPort(
        IContainer container)
    {
        return container.GetMappedPublicPort(PostgresPort);
    }

    private void EnsureDatabaseStarted()
    {
        if (!_started)
        {
            throw new InvalidOperationException(
                "Database has not been started");
        }
    }
}
