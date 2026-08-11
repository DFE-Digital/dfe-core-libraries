namespace DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Options.Networks;

public sealed class ContainerNetwork
{
    public string? Name { get; set; }
    public IList<string> Aliases { get; set; } = [];
}
