using DotNet.Testcontainers.Networks;

namespace DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Registry;

public interface IContainerNetworkRegistry : IAsyncDisposable
{
    Task<INetwork> GetOrCreateNetworkAsync(string key);
}
