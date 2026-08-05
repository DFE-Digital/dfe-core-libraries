namespace DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Options;

public sealed class ContainerNetworkOptions
{
    public IEnumerable<ContainerNetwork> Networks { get; set; } = [];
}

public sealed class ContainerNetwork
{
    public string? Name { get; set; }
    public IList<string> Aliases { get; set; } = [];
}
