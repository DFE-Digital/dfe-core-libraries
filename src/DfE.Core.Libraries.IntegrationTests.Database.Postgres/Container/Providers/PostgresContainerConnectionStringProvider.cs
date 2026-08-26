using DfE.Core.Libraries.IntegrationTests.Database.Postgres.Container.Options;
using Microsoft.Extensions.Options;
using Npgsql;

namespace DfE.Core.Libraries.IntegrationTests.Database.Postgres.Container.Providers;

internal sealed class PostgresContainerConnectionStringProvider : IPostgresContainerConnectionStringProvider
{
    private readonly IOptionsMonitor<PostgresContainerOptions> _options;

    public PostgresContainerConnectionStringProvider(IOptionsMonitor<PostgresContainerOptions> options)
    {
        _options = options;
    }

    public string GetConnectionString(string containerKey = "postgres")
    {
        if (string.IsNullOrWhiteSpace(containerKey))
        {
            throw new ArgumentException("Container name cannot be null or whitespace");
        }

        PostgresContainerOptions dbOptions = _options.Get(containerKey);

        string containerNetworkAlias = dbOptions.Container?.Networks?.FirstOrDefault()?.Aliases[0] ?? "localhost";

        NpgsqlConnectionStringBuilder connectionStringBuilder = new()
        {
            Host = containerNetworkAlias,
            Port = 5432,
            Database = dbOptions.Database!.Name,
            Username = dbOptions.Database.Username,
            Password = dbOptions.Database.Password
        };

        return connectionStringBuilder.ConnectionString;
    }
}
