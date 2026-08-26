using DfE.Core.Libraries.IntegrationTests.Database.Abstractions;

namespace DfE.Core.Libraries.IntegrationTests.Database.Postgres.Container.Provider;

public interface IPostgresDatabaseProvider
{
    Task<IDatabase> GetDatabaseAsync(string key, CancellationToken cancellationToken = default);
}
