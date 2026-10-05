using DotNet.Testcontainers.Networks;

namespace DfE.Core.Libraries.IntegrationTests.Containers.Registry;

public interface IContainerNetworkRegistry : IAsyncDisposable
{
    Task<INetwork> GetOrCreateNetworkAsync(string key);
}
