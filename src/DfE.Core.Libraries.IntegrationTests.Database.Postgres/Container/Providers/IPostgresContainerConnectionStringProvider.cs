namespace DfE.Core.Libraries.IntegrationTests.Database.Postgres.Container.Provider;

public interface IPostgresContainerConnectionStringProvider
{
    public string GetConnectionString(string containerKey = "postgres");
}
