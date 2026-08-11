namespace DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Registry;

public interface IContainerFactoryRegistry
{
    IContainerFactory GetFactory(string key);
}
