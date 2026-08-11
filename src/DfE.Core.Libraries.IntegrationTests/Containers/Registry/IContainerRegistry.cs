using DotNet.Testcontainers.Containers;

namespace DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Registry;

public interface IContainerRegistry
{
    Task<IContainer> GetOrCreateContainerAsync(string key, CancellationToken cancellationToken = default);
}
