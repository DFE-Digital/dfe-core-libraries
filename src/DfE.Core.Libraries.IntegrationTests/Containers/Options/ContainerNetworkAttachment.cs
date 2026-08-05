namespace DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Options;

public sealed class ContainerNetworkAttachment
{
    public string Key { get; set; } = string.Empty;
    public IList<string> Aliases { get; set; } = [];
}

