namespace DfE.Core.Libraries.IntegrationTests.Database.Postgres.Container.Providers;

public interface IPostgresContainerConnectionStringProvider
{
    public string GetConnectionString(string containerKey = "postgres");
}
