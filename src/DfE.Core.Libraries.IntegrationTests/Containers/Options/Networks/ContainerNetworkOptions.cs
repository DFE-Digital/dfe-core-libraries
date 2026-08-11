namespace DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Options.Networks;

public sealed class ContainerNetworkOptions
{
    public IEnumerable<ContainerNetwork> Networks { get; set; } = [];
}
