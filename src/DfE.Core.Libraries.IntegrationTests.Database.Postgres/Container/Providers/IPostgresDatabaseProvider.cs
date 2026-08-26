using DfE.Core.Libraries.IntegrationTests.Database.Abstractions;

namespace DfE.Core.Libraries.IntegrationTests.Database.Postgres.Container.Providers;

public interface IPostgresDatabaseProvider
{
    Task<IDatabase> GetDatabaseAsync(string key, CancellationToken cancellationToken = default);
}
