using DotNet.Testcontainers.Containers;

namespace DfE.Core.Libraries.IntegrationTests.Containers.Registry;

public interface IContainerFactory
{
    Task<IContainer> CreateAsync(string key, CancellationToken cancellationToken);
}
