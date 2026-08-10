using DotNet.Testcontainers.Containers;

namespace DfE.Core.Libraries.IntegrationTests.Abstractions.Containers;

public sealed record ContainerRegistration
{
    public ContainerRegistration(
        string key,
        Func<IContainerRegistry, CancellationToken, Task<IContainer>> create)
    {
        Key = key;
        Create = create;
    }

    public string Key { get; }
    public Func<IContainerRegistry, CancellationToken, Task<IContainer>> Create { get; }
}
