using Npgsql;

namespace DfE.Core.Libraries.IntegrationTests.Database.Postgres.Container.Providers;

public sealed record DatabaseConnection(
    string Host,
    int Port,
    string Database,
    string Username,
    string Password)
{
    public string GetConnectionString()
    {
        return new NpgsqlConnectionStringBuilder
        {
            Host = Host,
            Port = Port,
            Database = Database,
            Username = Username,
            Password = Password
        }.ConnectionString;
    }
}
