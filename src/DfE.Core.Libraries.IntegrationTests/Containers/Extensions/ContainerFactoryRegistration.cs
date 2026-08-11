using DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Registry;

namespace DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Extensions;

public sealed record ContainerFactoryRegistration
{
    public ContainerFactoryRegistration(string key, IContainerFactory factory)
    {
        Key = key;
        Factory = factory;
    }

    public string Key { get; }
    public IContainerFactory Factory { get; }
}
