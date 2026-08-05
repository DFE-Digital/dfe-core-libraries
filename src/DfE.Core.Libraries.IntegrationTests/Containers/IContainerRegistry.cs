using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;

namespace DfE.Core.Libraries.IntegrationTests.Abstractions.Containers;

public interface IContainerRegistry
{
    Task<INetwork> GetOrCreateNetworkAsync(string name);

    Task RegisterContainerAsync(
        string name,
        IContainer container);

    bool TryGetContainer(
        string name,
        out IContainer? container);
}
