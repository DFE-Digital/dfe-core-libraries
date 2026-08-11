using DotNet.Testcontainers.Containers;

namespace DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Registry;

public interface IContainerFactory
{
    Task<IContainer> CreateAsync(string key, CancellationToken cancellationToken);
}
