using DotNet.Testcontainers.Containers;

namespace DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Registry;

public interface IContainerRegistration
{
    string Key { get; }

    Task<IContainer> CreateAsync(
        IContainerRegistry registry,
        CancellationToken cancellationToken);
}
