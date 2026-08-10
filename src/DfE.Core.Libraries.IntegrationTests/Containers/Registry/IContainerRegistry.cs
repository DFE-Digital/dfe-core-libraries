using DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Options;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;

namespace DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Registry;

public interface IContainerRegistry
{
    Task<IContainer> GetOrCreateContainerAsync(string key, CancellationToken cancellationToken = default);
    Task<INetwork> GetOrCreateNetworkAsync(string key);
}
