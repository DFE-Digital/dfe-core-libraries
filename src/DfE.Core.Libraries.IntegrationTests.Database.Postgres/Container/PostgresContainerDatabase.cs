using DfE.Core.Libraries.IntegrationTests.Abstractions;
using DfE.Core.Libraries.IntegrationTests.Database.Abstractions;
using DotNet.Testcontainers.Containers;
using Npgsql;

namespace DfE.Core.Libraries.IntegrationTests.Database.Postgres.Container;

internal sealed class PostgresContainerDatabase : IDatabase
{
    public const int PostgresPort = 5432;

    private readonly Lazy<IContainer> _container;
    private DatabaseEndpoint? _endpoint;
    private readonly ContainerOptions _containerOptions;
    private readonly PostgresDatabaseOptions _databaseOptions;
    private readonly SemaphoreSlim _startLock = new(1, 1);
    private bool _started;

    public PostgresContainerDatabase(
        ContainerOptions containerOptions,
        PostgresDatabaseOptions databaseOptions,
        IContainerFactory containerFactory)
    {
        ArgumentNullException.ThrowIfNull(containerOptions);
        ArgumentNullException.ThrowIfNull(containerFactory);
        ArgumentNullException.ThrowIfNull(databaseOptions);

        _containerOptions = containerOptions;
        _databaseOptions = databaseOptions;
        _container = new Lazy<IContainer>(containerFactory.Create);
    }

    public async ValueTask DisposeAsync()
    {
        if (_container != null)
        {
            await _container.Value.DisposeAsync();
        }
    }

    public DatabaseEndpoint GetDatabaseEndpoint()
    {
        EnsureDatabaseStarted();
        return _endpoint!;
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

            IContainer container = _container.Value;

            _endpoint = new(
                host: container.Hostname, port: GetPublicContainerPort(container));

            _started = true;
        }
        finally
        {
            _startLock.Release();
        }
    }

    public async Task ExecuteAsync(string sql, CancellationToken ctx = default)
    {
        string connectionString = BuildConnectionString();
        await using NpgsqlConnection connection = new(connectionString);
        await connection.OpenAsync(ctx);

        await using NpgsqlCommand command = new(sql, connection);
        await command.ExecuteNonQueryAsync(ctx);
    }

    private string BuildConnectionString()
    {
        EnsureDatabaseStarted();

        return
            $"Host={_endpoint!.Host};" +
            $"Port={_endpoint.Port};" +
            $"Database={_databaseOptions.Database};" +
            $"Username={_databaseOptions.Username};" +
            $"Password={_databaseOptions.Password};";
    }

    private static ushort GetPublicContainerPort(IContainer container) => container.GetMappedPublicPort(PostgresPort);

    private void EnsureDatabaseStarted()
    {
        if (!_started || _endpoint is null)
        {
            throw new InvalidOperationException("Database has not been started");
        }
    }
}
