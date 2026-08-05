namespace DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Options;

public sealed class ContainerNetwork
{
    public string Name { get; set; } = string.Empty;
    public IList<string> Aliases { get; set; } = [];
}

