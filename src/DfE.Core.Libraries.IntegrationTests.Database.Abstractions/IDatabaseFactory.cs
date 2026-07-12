namespace DfE.Core.Libraries.IntegrationTests.Database.Abstractions;

public interface IDatabaseFactory
{
    Task<IDatabase> CreateAsync(CancellationToken ctx = default);
}
