namespace DfE.Core.Libraries.IntegrationTests.Database.Abstractions;

public interface IDatabase : IAsyncDisposable
{
    DatabaseEndpoint GetDatabaseEndpoint();
    Task StartAsync(CancellationToken ctx = default);
    Task ExecuteAsync(string sql, CancellationToken ctx = default);
}
