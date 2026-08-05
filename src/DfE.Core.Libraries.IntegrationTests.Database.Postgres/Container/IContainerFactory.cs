using DotNet.Testcontainers.Containers;

namespace DfE.Core.Libraries.IntegrationTests.Database.Postgres.Container;

public interface IContainerFactory
{
    Task<IContainer> Create();
}
