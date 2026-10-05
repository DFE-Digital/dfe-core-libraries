using DfE.Core.Libraries.IntegrationTests.Containers.Options.Container;

namespace DfE.Core.Libraries.IntegrationTests.Database.Postgres.Container.Options;

public sealed class PostgresContainerOptions
{
    public ContainerOptions? Container { get; set; }
    public PostgresDatabaseOptions? Database { get; set; }
}
