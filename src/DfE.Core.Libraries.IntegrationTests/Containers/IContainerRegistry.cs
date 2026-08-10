using DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Options;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;

namespace DfE.Core.Libraries.IntegrationTests.Abstractions.Containers;

public interface IContainerRegistry
{
    public void Register(string key, Func<IContainerRegistry, CancellationToken, Task<IContainer>> create);
    Task<IContainer> GetOrCreateContainerAsync(string key, CancellationToken cancellationToken = default);
    Task<INetwork> GetOrCreateNetworkAsync(string key);
}
